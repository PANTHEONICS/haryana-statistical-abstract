namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// Login Response DTO for User Management Module
    /// </summary>
    public class UserManagementLoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserManagementUserDto User { get; set; } = null!;
    }
}
