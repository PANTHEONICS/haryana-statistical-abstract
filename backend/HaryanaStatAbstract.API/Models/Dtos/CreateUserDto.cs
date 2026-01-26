using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// DTO for creating a new user
    /// </summary>
    public class CreateUserDto
    {
        [Required]
        [MaxLength(50)]
        public string LoginID { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")]
        public string UserMobileNo { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(100)]
        public string? UserEmailID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public int RoleID { get; set; }

        public int? DepartmentID { get; set; }

        public int? SecurityQuestionID { get; set; }

        [MaxLength(100)]
        public string? SecurityQuestionAnswer { get; set; }
    }
}
