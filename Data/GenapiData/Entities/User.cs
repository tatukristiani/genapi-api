using System.ComponentModel.DataAnnotations;

namespace genapi_api.Data.GenapiData.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
        public ICollection<Organization> OrganizationsAsEditor { get; set; } = new List<Organization>();
    }
}
