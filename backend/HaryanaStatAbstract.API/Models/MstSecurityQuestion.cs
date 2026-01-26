using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Master table for Security Questions
    /// </summary>
    [Table("Mst_SecurityQuestions")]
    public class MstSecurityQuestion
    {
        [Key]
        [Column("SecurityQuestionID")]
        public int SecurityQuestionID { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("QuestionText")]
        public string QuestionText { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<MasterUser> Users { get; set; } = new List<MasterUser>();
    }
}
