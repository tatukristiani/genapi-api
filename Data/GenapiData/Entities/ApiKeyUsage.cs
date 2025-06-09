using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace genapi_api.Data.GenapiData.Entities
{
    public class ApiKeyUsage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ApiKeyId { get; set; }

        [ForeignKey(nameof(ApiKeyId))]
        public ApiKey ApiKey { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int RequestCount { get; set; } = 0;
    }
}
