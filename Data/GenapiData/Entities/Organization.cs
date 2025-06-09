using System.ComponentModel.DataAnnotations;

namespace genapi_api.Data.GenapiData.Entities
{
    public class Organization
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<User> Editors { get; set; } = new List<User>();

        public ICollection<User> Users { get; set; } = new List<User>();

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
