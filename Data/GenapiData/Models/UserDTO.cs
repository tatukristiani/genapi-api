namespace genapi_api.Data.GenapiData.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<string> Organizations { get; set; } = new List<string>();
        public ICollection<string> OrganizationsAsEditor { get; set; } = new List<string>();
    }
}
