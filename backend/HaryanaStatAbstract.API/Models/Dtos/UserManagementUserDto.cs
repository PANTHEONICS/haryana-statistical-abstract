namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// User DTO for User Management Module
    /// </summary>
    public class UserManagementUserDto
    {
        public int UserID { get; set; }
        public string LoginID { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserMobileNo { get; set; } = string.Empty;
        public string? UserEmailID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public DateTime? CurrentLoginDateTime { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
