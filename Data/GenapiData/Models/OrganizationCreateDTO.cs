using System.ComponentModel.DataAnnotations;

namespace genapi_api.Data.GenapiData.Models
{
    public class OrganizationCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<string> Editors { get; set; } = new List<string>();

        public ICollection<string> Users { get; set; } = new List<string>();
    }
}
