using genapi_api.Data;
using genapi_api.Services.Builders;
using genapi_api.Services.Factories;
using genapi_api.VersionControl;

namespace genapi_api.Services
{
    public class GeneratorService
    {
        public void Generate(string dbType, CodeGenerationOptions options)
        {
            IApiGeneratorFactory factory = dbType.ToLower() switch
            {
                "azure" => new AzureSqlApiGeneratorFactory(options),
                _ => throw new ArgumentException("Unsupported database type")
            };

            var builder = new ApiBuilder(factory);
            builder.Build();
        }

        public void Publish(PublishOptions options)
        {
            GithubService.InitializeGitRepo(options.ProjectPath);
            GithubService.PushToGitHub(options.ProjectPath, options.GitHubRemoteUrl, options.GitHubUsername, options.GitHubPersonalAccessToken);
        }
    }
}
