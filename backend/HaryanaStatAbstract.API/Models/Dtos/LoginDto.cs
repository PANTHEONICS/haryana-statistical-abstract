using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// Data transfer object for user login
    /// </summary>
    public class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
