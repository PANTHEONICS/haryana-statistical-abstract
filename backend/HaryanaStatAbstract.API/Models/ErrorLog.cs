using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaryanaStatAbstract.API.Models
{
    /// <summary>
    /// Error Log Entity - Stores runtime errors and exceptions
    /// </summary>
    [Table("Error_Logs")]
    public class ErrorLog
    {
        [Key]
        [Column("ErrorLogID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ErrorLogID { get; set; }

        [Required]
        [Column("ErrorLevel")]
        [MaxLength(50)]
        public string ErrorLevel { get; set; } = string.Empty; // Error, Warning, Information, Critical, Debug

        [Required]
        [Column("ErrorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        [Column("ExceptionType")]
        [MaxLength(200)]
        public string? ExceptionType { get; set; }

        [Column("StackTrace")]
        public string? StackTrace { get; set; }

        [Column("InnerException")]
        public string? InnerException { get; set; }

        [Column("Source")]
        [MaxLength(500)]
        public string? Source { get; set; } // Controller, Service, Middleware, etc.

        [Column("MethodName")]
        [MaxLength(200)]
        public string? MethodName { get; set; }

        [Column("RequestPath")]
        [MaxLength(500)]
        public string? RequestPath { get; set; }

        [Column("RequestMethod")]
        [MaxLength(10)]
        public string? RequestMethod { get; set; } // GET, POST, PUT, DELETE

        [Column("UserID")]
        public int? UserID { get; set; }

        [Column("UserLoginID")]
        [MaxLength(50)]
        public string? UserLoginID { get; set; }

        [Column("IPAddress")]
        [MaxLength(50)]
        public string? IPAddress { get; set; }

        [Column("RequestHeaders")]
        public string? RequestHeaders { get; set; } // JSON string

        [Column("RequestBody")]
        public string? RequestBody { get; set; }

        [Column("QueryString")]
        public string? QueryString { get; set; }

        [Column("AdditionalData")]
        public string? AdditionalData { get; set; } // JSON string for additional context

        [Required]
        [Column("IsResolved")]
        public bool IsResolved { get; set; } = false;

        [Column("ResolvedBy")]
        public int? ResolvedBy { get; set; }

        [Column("ResolvedAt")]
        public DateTime? ResolvedAt { get; set; }

        [Column("ResolutionNotes")]
        public string? ResolutionNotes { get; set; }

        [Required]
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual MasterUser? User { get; set; }

        [ForeignKey("ResolvedBy")]
        public virtual MasterUser? ResolvedByUser { get; set; }
    }
}
