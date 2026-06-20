namespace PRN232.LMS.Services.Models.Business;

public class SemesterBusiness
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CourseBusiness>? Courses { get; set; }
}
