using System.ComponentModel.DataAnnotations;

namespace genapi_api.Data.GenapiData.Entities
{
    public class Organization
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<User> Editors { get; set; } = new List<User>();

        public ICollection<User> Users { get; set; } = new List<User>();

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
