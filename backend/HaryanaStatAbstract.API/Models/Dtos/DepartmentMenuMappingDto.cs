namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// DTO for Department Menu Mapping
    /// </summary>
    public class DepartmentMenuMappingDto
    {
        public int MappingID { get; set; }
        public int DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int MenuID { get; set; }
        public string? MenuName { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for assigning menus to department
    /// </summary>
    public class AssignMenusToDepartmentDto
    {
        public int DepartmentID { get; set; }
        public List<int> MenuIDs { get; set; } = new List<int>();
    }
}
