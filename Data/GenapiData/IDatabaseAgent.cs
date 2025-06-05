using genapi_api.Data.GenapiData.Entities;

namespace genapi_api.Data.GenapiData
{
    public interface IDatabaseAgent
    {
        // User Entity methods
        Task<User?> GetUserByUsername(string username);
        Task<User?> GetUserById(int id);
        bool UserExists(string username, string email);

        // Organization Entity methods
        Task<Organization?> GetOrganizationById(int id);
        Task<bool> OrganizationExists(string name);

        // Shared methods
        Task<bool> AddEntity(object entity);
        Task<bool> UpdateEntity(object entity);
    }
}
