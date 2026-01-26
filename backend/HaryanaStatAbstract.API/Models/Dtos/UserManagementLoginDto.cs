using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// Login DTO for User Management Module
    /// </summary>
    public class UserManagementLoginDto
    {
        [Required]
        [MaxLength(50)]
        public string LoginID { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
