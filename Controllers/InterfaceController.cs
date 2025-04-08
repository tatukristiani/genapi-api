using genapi_api.CodeGeneration;
using genapi_api.CodeGenerators;
using genapi_api.Data;
using genapi_api.VersionControl;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static genapi_api.Data.Configurations;

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
        public IActionResult CreateInterface([FromBody] Configurations configurations)
        {
            Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Request received");
            return CreateInitialInterface(configurations) ? Created() : BadRequest();
        }

        private bool CreateInitialInterface(Configurations configurations)
        {
            Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Generating API...");
            try
            {
                Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Validating configurations...");
                // Validate configurations
                ValidationResult validationResult = configurations.Validate();
                if (!validationResult.Success) BadRequest($"Invalid configuration. Errors: {validationResult.Error}");

                // Define some required information
                string projectName = $"Genapi_{Guid.NewGuid()}".Replace("-", "_");
                string generatedAppsPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedApps");
                string projectPath = Path.Combine(generatedAppsPath, projectName);
                string gitRemoteUrl = configurations.GitHubRepository;

                // Step 1: Create a new directory for the project & delete if already exists
                if (Directory.Exists(projectPath))
                {
                    Directory.Delete(projectPath, true);
                }
                Directory.CreateDirectory(projectPath);

                // Step 2: Create a new .NET Web API project
                ProjectCodeGenerator.GenerateDefaultWebAPI(projectName, generatedAppsPath);

                // Step 3. Add required Azure SQL Database packages to csproj file
                ProjectCodeGenerator.AddAzureSqlPackagesToProjectFile(projectPath, projectName);

                // Step 4. Update Program.cs
                ProjectCodeGenerator.GenerateDefaultProgramCS(projectPath, projectName);

                // Step 5: Add a .gitignore file
                ProjectCodeGenerator.GenerateGitignore(projectPath);

                // Step 6: Create Directory for Models
                string modelsPath = Path.Combine(projectPath, "Models");
                Directory.CreateDirectory(modelsPath);

                // Step 7: Create Model classes
                ModelCodeGenerator.GenerateModels(configurations.Resources, modelsPath, projectName);

                // Step 8: Create Directory for Data + DbContext class
                string dataPath = Path.Combine(projectPath, "Data");
                Directory.CreateDirectory(dataPath);
                ProjectCodeGenerator.GenerateApplicationDbContext(configurations.Resources, projectName, dataPath);

                // Step 9: Create Directory for Controllers & Controller classes
                string controllerPath = Path.Combine(projectPath, "Controllers");
                Directory.CreateDirectory(controllerPath);
                ControllerCodeGenerator.GenerateControllers(configurations.Resources.Select(r => r.Name).ToList(), projectName, controllerPath);

                // Step 10: Initialize Git
                GithubService.InitializeGitRepo(projectPath);

                // Step 11: Push to Git
                return GithubService.PushToGitHub(projectPath, gitRemoteUrl, configurations.GitHubUser, configurations.GitHubPersonalAccessToken);
            }
            catch (Exception ex)
            {
                Log.Error($"{HttpContext.GetEndpoint()?.DisplayName}: Failure during generation. Message: {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
        }
    }
}
