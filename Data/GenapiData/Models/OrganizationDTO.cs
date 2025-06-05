namespace genapi_api.Data.GenapiData.Models
{
    public class OrganizationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<string> Editors { get; set; } = new List<string>();

        public ICollection<string> Users { get; set; } = new List<string>();
        public DateTime Created { get; set; }
    }
}
