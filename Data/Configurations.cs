namespace genapi_api.Data
{
    public class Configurations
    {
        public required string ApiName { get; set; }
        public required string GitHubRepository { get; set; }
        public required string GitHubUser { get; set; }
        public required string GitHubPersonalAccessToken { get; set; }
        public required List<Resource> Resources { get; set; }

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
