using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Mapping;

public class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        // Entity → Business
        CreateMap<Student, StudentBusiness>().PreserveReferences();
        CreateMap<Course, CourseBusiness>().PreserveReferences();
        CreateMap<Enrollment, EnrollmentBusiness>().PreserveReferences();
        CreateMap<Semester, SemesterBusiness>().PreserveReferences();
        CreateMap<Subject, SubjectBusiness>().PreserveReferences();

        // Business → Entity (for create/update)
        CreateMap<StudentBusiness, Student>()
            .ForMember(d => d.StudentId, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore());

        CreateMap<CourseBusiness, Course>()
            .ForMember(d => d.CourseId, o => o.Ignore())
            .ForMember(d => d.Semester, o => o.Ignore())
            .ForMember(d => d.Subject, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore());

        CreateMap<EnrollmentBusiness, Enrollment>()
            .ForMember(d => d.EnrollmentId, o => o.Ignore())
            .ForMember(d => d.Student, o => o.Ignore())
            .ForMember(d => d.Course, o => o.Ignore());

        CreateMap<SemesterBusiness, Semester>()
            .ForMember(d => d.SemesterId, o => o.Ignore())
            .ForMember(d => d.Courses, o => o.Ignore());

        CreateMap<SubjectBusiness, Subject>()
            .ForMember(d => d.SubjectId, o => o.Ignore())
            .ForMember(d => d.Courses, o => o.Ignore());

        // User
        CreateMap<User, UserBusiness>()
            .ForMember(d => d.Password, o => o.Ignore());
    }
}
