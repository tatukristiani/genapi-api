namespace genapi_api.Data
{
    public class Configurations
    {
        public string GitHubRepository { get; set; } = String.Empty;
        public string GitHubUser { get; set; } = String.Empty;
        public string GitHubPersonalAccessToken { get; set; } = String.Empty;
        public List<Resource> Resources { get; set; }

        // TODO: Add validation
        public ValidationResult Validate()
        {
            return new ValidationResult(true, String.Empty);
        }

        public class ValidationResult(bool success, string error)
        {
            public bool Success { get; set; } = success;
            public string Error { get; set; } = error;
        }
    }
}
