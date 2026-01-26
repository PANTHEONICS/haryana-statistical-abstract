namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// Data transfer object for role information
    /// </summary>
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
