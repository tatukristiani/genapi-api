using genapi_api.CodeGeneration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;

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
        public IActionResult CreateInterface()
        {
            Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Request received");
            return CreateInitialInterface() ? Created() : BadRequest();
        }

        private bool CreateInitialInterface()
        {
            try
            {
                JObject configurations = new JObject
                {
                    ["connectionString"] = "Server=your-azure-server.database.windows.net;Database=YourDatabaseName;User Id=YourUsername;Password=YourPassword;TrustServerCertificate=True;",
                    ["models"] = new JArray
                    {
                        new JObject
                        {
                            ["name"] = "Product",
                            ["properties"] = new JArray
                            {
                                new JObject
                                {
                                    ["type"] = "int",
                                    ["name"] = "Id",
                                    ["nullable"] = false,
                                    ["pk"] = true,
                                    ["fk"] = false,
                                    ["fkReference"] = "",
                                    ["maxLength"] = null,
                                    ["decimalIntegerPart"] = null,
                                    ["decimalFractionPart"] = null
                                },
                                 new JObject
                                {
                                    ["type"] = "string",
                                    ["name"] = "Name",
                                    ["nullable"] = false,
                                    ["pk"] = false,
                                    ["fk"] = false,
                                    ["fkReference"] = "",
                                    ["maxLength"] = 100,
                                    ["decimalIntegerPart"] = null,
                                    ["decimalFractionPart"] = null
                                },
                                 new JObject
                                {
                                    ["type"] = "decimal",
                                    ["name"] = "Price",
                                    ["nullable"] = false,
                                    ["pk"] = false,
                                    ["fk"] = false,
                                    ["fkReference"] = "",
                                    ["maxLength"] = null,
                                    ["decimalIntegerPart"] = 10,
                                    ["decimalFractionPart"] = 2
                                }
                            }
                        },
                        new JObject
                        {
                            ["name"] = "Order",
                            ["properties"] = new JArray
                            {
                                new JObject
                                {
                                    ["type"] = "int",
                                    ["name"] = "Id",
                                    ["nullable"] = false,
                                    ["pk"] = true,
                                    ["fk"] = false,
                                    ["fkReference"] = "",
                                    ["maxLength"] = null,
                                    ["decimalIntegerPart"] = null,
                                    ["decimalFractionPart"] = null
                                },
                                new JObject
                                {
                                    ["type"] = "datetime",
                                    ["name"] = "OrderDate",
                                    ["nullable"] = false,
                                    ["pk"] = false,
                                    ["fk"] = false,
                                    ["fkReference"] = "",
                                    ["maxLength"] = null,
                                    ["decimalIntegerPart"] = null,
                                    ["decimalFractionPart"] = null
                                },
                                new JObject
                                {
                                    ["type"] = "int",
                                    ["name"] = "ProductId",
                                    ["nullable"] = false,
                                    ["pk"] = false,
                                    ["fk"] = true,
                                    ["fkReference"] = "Products",
                                    ["maxLength"] = null,
                                    ["decimalIntegerPart"] = null,
                                    ["decimalFractionPart"] = null
                                },
                                new JObject
                                {
                                    ["type"] = "int",
                                    ["name"] = "Quantity",
                                    ["nullable"] = false,
                                    ["pk"] = false,
                                    ["fk"] = false,
                                    ["fkReference"] = "",
                                    ["maxLength"] = null,
                                    ["decimalIntegerPart"] = null,
                                    ["decimalFractionPart"] = null
                                }
                            }
                        }
                    }
                };

                Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Initial API creation started...");

                // Define some required information
                string projectName = $"Genapi_{Guid.NewGuid()}".Replace("-", "_");
                string generatedAppsPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedApps");
                string projectPath = Path.Combine(generatedAppsPath, projectName);
                string gitRemoteUrl = "https://github.com/tatukristiani/testrepo.git"; // Replace this to a dynamic value later!!!

                Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Project name is {projectName}");
                Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Project path is {projectPath}");

                // Step 1: Create a new directory for the project & delete if already exists
                if (Directory.Exists(projectPath))
                {
                    Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Directory {projectPath} EXISTS!");
                    Directory.Delete(projectPath, true);
                    Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Directory {projectPath} DELETED.");
                }
                Directory.CreateDirectory(projectPath);
                Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Directory {projectPath} CREATED.");

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
                ModelCodeGenerator.GenerateModels(configurations.Value<JArray>("models"), modelsPath, projectName);

                // Step 8: Create DbContext

                // Step 9: Initialize Git
                /*
                GithubService.InitializeGitRepo(projectPath);

                // Step 10: Push to Git
                return GithubService.PushToGitHub(projectPath, gitRemoteUrl, _config["GITHUB_USERNAME"], _config["GITHUB_TOKEN"]); // Replace credentials to use values from the request instead of .env
            */
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"{HttpContext.GetEndpoint()?.DisplayName}: Failure during generation. Message: {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
        }
    }
}
