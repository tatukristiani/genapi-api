namespace genapi_api.Data.GenapiData.Models
{
    public class UserDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<string> Organizations { get; set; } = new List<string>();
        public ICollection<string> OrganizationsAsEditor { get; set; } = new List<string>();
    }
}
