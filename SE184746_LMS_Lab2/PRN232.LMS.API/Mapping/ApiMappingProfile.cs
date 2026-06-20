using AutoMapper;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.API.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        // ── Request → Business ──────────────────────────────────────────
        CreateMap<CreateStudentRequest, StudentBusiness>()
            .ForMember(d => d.StudentId, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore())
            .ForMember(d => d.StudentCode, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.StudentCode) ? null : s.StudentCode.Trim().ToUpper()));

        CreateMap<UpdateStudentRequest, StudentBusiness>()
            .ForMember(d => d.StudentId, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore())
            .ForMember(d => d.StudentCode, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.StudentCode) ? null : s.StudentCode.Trim().ToUpper()));

        CreateMap<CreateCourseRequest, CourseBusiness>()
            .ForMember(d => d.CourseId, o => o.Ignore())
            .ForMember(d => d.Semester, o => o.Ignore())
            .ForMember(d => d.Subject, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore());

        CreateMap<UpdateCourseRequest, CourseBusiness>()
            .ForMember(d => d.CourseId, o => o.Ignore())
            .ForMember(d => d.Semester, o => o.Ignore())
            .ForMember(d => d.Subject, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore());

        CreateMap<CreateSemesterRequest, SemesterBusiness>()
            .ForMember(d => d.SemesterId, o => o.Ignore())
            .ForMember(d => d.Courses, o => o.Ignore());

        CreateMap<UpdateSemesterRequest, SemesterBusiness>()
            .ForMember(d => d.SemesterId, o => o.Ignore())
            .ForMember(d => d.Courses, o => o.Ignore());

        CreateMap<CreateSubjectRequest, SubjectBusiness>()
            .ForMember(d => d.SubjectId, o => o.Ignore())
            .ForMember(d => d.Courses, o => o.Ignore());

        CreateMap<UpdateSubjectRequest, SubjectBusiness>()
            .ForMember(d => d.SubjectId, o => o.Ignore())
            .ForMember(d => d.Courses, o => o.Ignore());

        CreateMap<CreateEnrollmentRequest, EnrollmentBusiness>()
            .ForMember(d => d.EnrollmentId, o => o.Ignore())
            .ForMember(d => d.Student, o => o.Ignore())
            .ForMember(d => d.Course, o => o.Ignore());

        CreateMap<UpdateEnrollmentRequest, EnrollmentBusiness>()
            .ForMember(d => d.EnrollmentId, o => o.Ignore())
            .ForMember(d => d.Student, o => o.Ignore())
            .ForMember(d => d.Course, o => o.Ignore());

        // ── Business → Response ─────────────────────────────────────────
        CreateMap<StudentBusiness, StudentResponse>().PreserveReferences();
        CreateMap<CourseBusiness, CourseResponse>().PreserveReferences();
        CreateMap<EnrollmentBusiness, EnrollmentResponse>().PreserveReferences();
        CreateMap<SemesterBusiness, SemesterResponse>().PreserveReferences();
        CreateMap<SubjectBusiness, SubjectResponse>().PreserveReferences();

        // Token
        CreateMap<TokenBusiness, TokenResponse>();

        // Student V2
        CreateMap<CreateStudentRequestV2, StudentBusiness>()
            .ForMember(d => d.StudentId, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore())
            .ForMember(d => d.StudentCode, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.StudentCode) ? null : s.StudentCode.Trim().ToUpper()));

        CreateMap<UpdateStudentRequestV2, StudentBusiness>()
            .ForMember(d => d.StudentId, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore())
            .ForMember(d => d.StudentCode, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.StudentCode) ? null : s.StudentCode.Trim().ToUpper()));

        CreateMap<StudentBusiness, StudentResponseV2>().PreserveReferences();

        // User
        CreateMap<CreateUserRequest, UserBusiness>()
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.PasswordHash, o => o.Ignore());

        CreateMap<UpdateUserRequest, UserBusiness>()
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.PasswordHash, o => o.Ignore());

        CreateMap<UserBusiness, UserResponse>();
    }
}
