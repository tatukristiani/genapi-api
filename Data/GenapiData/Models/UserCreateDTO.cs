using System.ComponentModel.DataAnnotations;

namespace genapi_api.Data.GenapiData.Models
{
    public class UserCreateDTO
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
