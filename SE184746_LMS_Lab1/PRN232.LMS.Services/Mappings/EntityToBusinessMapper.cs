using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Models.Business;

namespace PRN232.LMS.Services.Mappings;

public static class EntityToBusinessMapper
{
    public static StudentBusiness ToStudentBusiness(Student student, IReadOnlyCollection<string> expand)
    {
        var expandSet = QueryStringHelper.ToExpandSet(expand);

        IReadOnlyList<EnrollmentBusiness>? enrollments = null;
        if (expandSet.Contains("enrollments") || expandSet.Any(x => x.StartsWith("enrollments.")))
        {
            enrollments = student.Enrollments
                .Select(e => ToEnrollmentForStudent(e, expandSet))
                .ToList();
        }

        return new StudentBusiness
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth,
            Enrollments = enrollments
        };
    }

    public static EnrollmentBusiness ToEnrollmentBusiness(Enrollment enrollment, IReadOnlyCollection<string> expand)
    {
        var expandSet = QueryStringHelper.ToExpandSet(expand);

        StudentBusiness? student = null;
        if (expandSet.Contains("student"))
        {
            student = ToStudentBusiness(enrollment.Student, Array.Empty<string>());
        }

        CourseBusiness? course = null;
        if (expandSet.Contains("course")
            || expandSet.Contains("course.semester")
            || expandSet.Contains("course.subject"))
        {
            course = ToCourseBusiness(enrollment.Course, expandSet);
        }

        return new EnrollmentBusiness
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status,
            Student = student,
            Course = course
        };
    }

    public static CourseBusiness ToCourseBusiness(Course course, IReadOnlyCollection<string> expand)
    {
        var expandSet = expand is HashSet<string> set ? set : QueryStringHelper.ToExpandSet(expand);

        SemesterBusiness? semester = null;
        if (ShouldIncludeSemester(expandSet) && course.Semester is not null)
        {
            semester = ToSemesterBusiness(course.Semester, Array.Empty<string>());
        }

        SubjectBusiness? subject = null;
        if (ShouldIncludeSubject(expandSet) && course.Subject is not null)
        {
            subject = ToSubjectBusiness(course.Subject, Array.Empty<string>());
        }

        IReadOnlyList<EnrollmentBusiness>? enrollments = null;
        if (expandSet.Contains("enrollments") || expandSet.Any(x => x.StartsWith("enrollments.")))
        {
            enrollments = course.Enrollments
                .Select(e => ToEnrollmentForCourse(e, expandSet))
                .ToList();
        }

        return new CourseBusiness
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId,
            SubjectId = course.SubjectId,
            Semester = semester,
            Subject = subject,
            Enrollments = enrollments
        };
    }

    public static SemesterBusiness ToSemesterBusiness(Semester semester, IReadOnlyCollection<string> expand)
    {
        var expandSet = QueryStringHelper.ToExpandSet(expand);

        IReadOnlyList<CourseBusiness>? courses = null;
        if (expandSet.Contains("courses") || expandSet.Any(x => x.StartsWith("courses.")))
        {
            courses = semester.Courses
                .Select(c => ToCourseForSemester(c, expandSet))
                .ToList();
        }

        return new SemesterBusiness
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            Courses = courses
        };
    }

    public static SubjectBusiness ToSubjectBusiness(Subject subject, IReadOnlyCollection<string> expand)
    {
        var expandSet = QueryStringHelper.ToExpandSet(expand);

        IReadOnlyList<CourseBusiness>? courses = null;
        if (expandSet.Contains("courses") || expandSet.Any(x => x.StartsWith("courses.")))
        {
            courses = subject.Courses
                .Select(c => ToCourseForSubject(c, expandSet))
                .ToList();
        }

        return new SubjectBusiness
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit,
            Courses = courses
        };
    }

    private static EnrollmentBusiness ToEnrollmentForStudent(Enrollment enrollment, HashSet<string> expandSet)
    {
        CourseBusiness? course = null;
        if (expandSet.Contains("enrollments.course")
            || expandSet.Contains("enrollments.course.semester")
            || expandSet.Contains("enrollments.course.subject"))
        {
            course = ToCourseBusiness(enrollment.Course, expandSet);
        }

        return new EnrollmentBusiness
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status,
            Student = null,
            Course = course
        };
    }

    private static EnrollmentBusiness ToEnrollmentForCourse(Enrollment enrollment, HashSet<string> expandSet)
    {
        StudentBusiness? student = null;
        if (expandSet.Contains("enrollments.student"))
        {
            student = ToStudentBusiness(enrollment.Student, Array.Empty<string>());
        }

        return new EnrollmentBusiness
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status,
            Student = student,
            Course = null
        };
    }

    private static CourseBusiness ToCourseForSemester(Course course, HashSet<string> expandSet)
    {
        SubjectBusiness? subject = null;
        if (expandSet.Contains("courses.subject") && course.Subject is not null)
        {
            subject = ToSubjectBusiness(course.Subject, Array.Empty<string>());
        }

        return new CourseBusiness
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId,
            SubjectId = course.SubjectId,
            Semester = null,
            Subject = subject,
            Enrollments = null
        };
    }

    private static CourseBusiness ToCourseForSubject(Course course, HashSet<string> expandSet)
    {
        SemesterBusiness? semester = null;
        if (expandSet.Contains("courses.semester") && course.Semester is not null)
        {
            semester = ToSemesterBusiness(course.Semester, Array.Empty<string>());
        }

        return new CourseBusiness
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId,
            SubjectId = course.SubjectId,
            Semester = semester,
            Subject = null,
            Enrollments = null
        };
    }

    private static bool ShouldIncludeSemester(HashSet<string> expandSet)
        => expandSet.Contains("semester")
           || expandSet.Contains("course.semester")
           || expandSet.Contains("enrollments.course.semester");

    private static bool ShouldIncludeSubject(HashSet<string> expandSet)
        => expandSet.Contains("subject")
           || expandSet.Contains("course.subject")
           || expandSet.Contains("enrollments.course.subject");

}
