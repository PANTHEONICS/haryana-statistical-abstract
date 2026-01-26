namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// DTO for Menu information
    /// </summary>
    public class MenuDto
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuPath { get; set; } = string.Empty;
        public string? MenuIcon { get; set; }
        public int? ParentMenuID { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdminOnly { get; set; }
        public string? MenuDescription { get; set; }
        public List<MenuDto>? ChildMenus { get; set; }
    }
}
