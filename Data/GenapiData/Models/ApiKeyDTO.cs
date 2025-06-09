using System.ComponentModel.DataAnnotations;

namespace genapi_api.Data.GenapiData.Entities
{
    public class ApiKeyDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public string? Description { get; set; }

        [Required]
        public int DailyLimit { get; set; }
    }
}
