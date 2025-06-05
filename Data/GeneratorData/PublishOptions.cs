namespace genapi_api.Data.GeneratorData
{
    public class PublishOptions(string projectPath, string gitHubRemoteUrl, string gitHubUsername, string gitHubPersonalAccessToken)
    {
        public string ProjectPath { get; set; } = projectPath;
        public string GitHubRemoteUrl { get; set; } = gitHubRemoteUrl;
        public string GitHubUsername { get; set; } = gitHubUsername;
        public string GitHubPersonalAccessToken { get; set; } = gitHubPersonalAccessToken;
    }
}
