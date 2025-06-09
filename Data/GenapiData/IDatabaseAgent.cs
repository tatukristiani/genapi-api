using genapi_api.Data.GenapiData.Entities;

namespace genapi_api.Data.GenapiData
{
    public interface IDatabaseAgent
    {
        // User Entity methods
        Task<User?> GetUserByUsername(string username);
        Task<User?> GetUserById(Guid id);
        bool UserExists(string username, string email);

        // Organization Entity methods
        Task<Organization?> GetOrganizationById(Guid id);
        Task<bool> OrganizationExists(string name);

        // ApiKey entity methods
        Task<ApiKey?> GetValidApiKey(Guid apiKeyValue);
        Task<ICollection<ApiKey>> GetApiKeys();

        // ApiKeyUsage entity methods
        Task<ApiKeyUsage?> GetApiKeyUsage(Guid apiKeyId);

        // Shared methods
        Task<bool> AddEntity(object entity);
        Task<bool> UpdateEntity(object entity);
    }
}
