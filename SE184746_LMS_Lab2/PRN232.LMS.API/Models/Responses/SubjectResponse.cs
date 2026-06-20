namespace PRN232.LMS.API.Models.Responses;

public class SubjectResponse
{
    public int? SubjectId { get; set; }
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public int? Credit { get; set; }
    public List<CourseResponse>? Courses { get; set; }
}
