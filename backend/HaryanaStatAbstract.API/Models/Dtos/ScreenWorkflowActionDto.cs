using System.ComponentModel.DataAnnotations;

namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// DTO for executing screen-level workflow actions
    /// </summary>
    public class ScreenWorkflowActionDto
    {
        [Required]
        [MaxLength(50)]
        public string ScreenCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Remarks are mandatory for rejections
        /// </summary>
        public string? Remarks { get; set; }
    }
}
