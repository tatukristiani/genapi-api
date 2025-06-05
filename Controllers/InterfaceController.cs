using genapi_api.Data.GeneratorData;
using genapi_api.Services.GeneratorServices;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static genapi_api.Data.GeneratorData.Configurations;

namespace genapi_api.Controllers
{
    [ApiController]
    [Route("api/v1/interfaces")]
    public class InterfaceController : ControllerBase
    {
        private readonly ILogger<InterfaceController> _logger;
        private static IConfiguration _config;

        public InterfaceController(ILogger<InterfaceController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult CreateInterface([FromBody] Configurations configurations)
        {
            Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Request received");
            try
            {
                Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Generating API...");
                Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Validating configurations...");

                // Validate configurations
                ValidationResult validationResult = configurations.Validate();
                if (!validationResult.Success) BadRequest($"Invalid configuration. Errors: {validationResult.Error}");

                // Define some required information
                string projectName = $"Genapi_{Guid.NewGuid()}".Replace("-", "_");
                string generatedAppsPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedApps");
                string projectPath = Path.Combine(generatedAppsPath, projectName);
                string gitRemoteUrl = configurations.GitHubRepository;

                // Create project directory
                if (Directory.Exists(projectPath))
                {
                    Directory.Delete(projectPath, true);
                }
                Directory.CreateDirectory(projectPath);

                // Generate code via service
                GeneratorService service = new GeneratorService();
                service.Generate("azure", new CodeGenerationOptions(projectPath, projectName, generatedAppsPath, configurations));
                service.Publish(new PublishOptions(projectPath, gitRemoteUrl, configurations.GitHubUser, configurations.GitHubPersonalAccessToken));
                return Created();
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while generating code. Error: {ex.Message}. Stack: {ex.StackTrace}");
                return BadRequest();
            }
        }
    }
}
