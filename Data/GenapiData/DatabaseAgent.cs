using genapi_api.Data.GenapiData.Entities;
using Microsoft.EntityFrameworkCore;

namespace genapi_api.Data.GenapiData
{
    public class DatabaseAgent : IDatabaseAgent
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseAgent> _logger;

        public DatabaseAgent(ApplicationDbContext context, ILogger<DatabaseAgent> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ILogger<DatabaseAgent> Logger { get; }

        public async Task<bool> AddEntity(object entity)
        {
            _context.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<ApiKey>> GetApiKeys()
        {
            return await _context.ApiKeys.ToListAsync();
        }

        public async Task<ApiKeyUsage?> GetApiKeyUsage(Guid apiKeyId)
        {
            return await _context.ApiKeyUsages
                .FirstOrDefaultAsync(u => u.ApiKeyId == apiKeyId && u.Date == DateTime.UtcNow.Date);
        }

        public async Task<Organization?> GetOrganizationById(Guid id)
        {
            return await _context.Organizations
                .Include(o => o.Users)
                .Include(o => o.Editors)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users
                .Include(o => o.Organizations)
                .Include(o => o.OrganizationsAsEditor)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users
                .Include(o => o.Organizations)
                .Include(o => o.OrganizationsAsEditor)
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<ApiKey?> GetValidApiKey(Guid apiKeyValue)
        {
            return await _context.ApiKeys
                .FirstOrDefaultAsync(k => k.Key == apiKeyValue && k.IsActive && (k.ExpiresAt == null || k.ExpiresAt > DateTime.UtcNow));
        }

        public async Task<bool> OrganizationExists(string name)
        {
            if (_context.Organizations == null) return false;

            return await _context.Organizations.AnyAsync(e => e.Name == name);
        }

        public async Task<bool> UpdateApiKeyUsageCount(ApiKeyUsage usage)
        {
            _context.Update(usage);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateEntity(object entity)
        {
            _context.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool UserExists(string username, string email)
        {
            return (_context.Users?.Any(e => e.Username == username || e.Email == email)).GetValueOrDefault();
        }
    }
}
