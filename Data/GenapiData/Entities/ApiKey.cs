using System.ComponentModel.DataAnnotations;

namespace genapi_api.Data.GenapiData.Entities
{
    public class ApiKey
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public Guid Key { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Description { get; set; }

        [Required]
        public int DailyLimit { get; set; }
    }
}
