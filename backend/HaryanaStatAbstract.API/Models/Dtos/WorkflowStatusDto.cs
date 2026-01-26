namespace HaryanaStatAbstract.API.Models.Dtos
{
    /// <summary>
    /// DTO for Workflow Status
    /// </summary>
    public class WorkflowStatusDto
    {
        public int StatusID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public string? VisualStageKey { get; set; }
    }
}
