using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// Data transfer object for updating user information
    /// </summary>
    public class UpdateUserDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        public bool? IsActive { get; set; }
    }
}
