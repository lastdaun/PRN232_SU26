using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Mappings;

public static class BusinessToResponseMapper
{
    public static StudentResponse ToStudentResponse(StudentBusiness student)
        => new()
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth,
            Enrollments = student.Enrollments?.Select(ToEnrollmentResponse).ToList()
        };

    public static EnrollmentResponse ToEnrollmentResponse(EnrollmentBusiness enrollment)
        => new()
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status,
            Student = enrollment.Student is null ? null : ToStudentResponse(enrollment.Student),
            Course = enrollment.Course is null ? null : ToCourseResponse(enrollment.Course)
        };

    public static CourseResponse ToCourseResponse(CourseBusiness course)
        => new()
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId,
            SubjectId = course.SubjectId,
            Semester = course.Semester is null ? null : ToSemesterResponse(course.Semester),
            Subject = course.Subject is null ? null : ToSubjectResponse(course.Subject),
            Enrollments = course.Enrollments?.Select(ToEnrollmentResponse).ToList()
        };

    public static SemesterResponse ToSemesterResponse(SemesterBusiness semester)
        => new()
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            Courses = semester.Courses?.Select(ToCourseResponse).ToList()
        };

    public static SubjectResponse ToSubjectResponse(SubjectBusiness subject)
        => new()
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit,
            Courses = subject.Courses?.Select(ToCourseResponse).ToList()
        };

    public static Dictionary<string, object?> ShapeStudentFields(StudentResponse model, IReadOnlyCollection<string> fields)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in fields)
        {
            var key = field.Trim();
            if (key.Length == 0)
            {
                continue;
            }

            switch (key.ToLowerInvariant())
            {
                case "studentid":
                    dict["studentId"] = model.StudentId;
                    break;
                case "fullname":
                    dict["fullName"] = model.FullName;
                    break;
                case "email":
                    dict["email"] = model.Email;
                    break;
                case "dateofbirth":
                    dict["dateOfBirth"] = model.DateOfBirth;
                    break;
                case "enrollments":
                    dict["enrollments"] = model.Enrollments;
                    break;
            }
        }

        return dict;
    }

    public static Dictionary<string, object?> ShapeEnrollmentFields(EnrollmentResponse model, IReadOnlyCollection<string> fields)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in fields)
        {
            var key = field.Trim();
            if (key.Length == 0)
            {
                continue;
            }

            switch (key.ToLowerInvariant())
            {
                case "enrollmentid":
                    dict["enrollmentId"] = model.EnrollmentId;
                    break;
                case "studentid":
                    dict["studentId"] = model.StudentId;
                    break;
                case "courseid":
                    dict["courseId"] = model.CourseId;
                    break;
                case "enrolldate":
                    dict["enrollDate"] = model.EnrollDate;
                    break;
                case "status":
                    dict["status"] = model.Status;
                    break;
                case "student":
                    dict["student"] = model.Student;
                    break;
                case "course":
                    dict["course"] = model.Course;
                    break;
            }
        }

        return dict;
    }

    public static Dictionary<string, object?> ShapeCourseFields(CourseResponse model, IReadOnlyCollection<string> fields)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in fields)
        {
            var key = field.Trim();
            if (key.Length == 0)
            {
                continue;
            }

            switch (key.ToLowerInvariant())
            {
                case "courseid":
                    dict["courseId"] = model.CourseId;
                    break;
                case "coursename":
                    dict["courseName"] = model.CourseName;
                    break;
                case "semesterid":
                    dict["semesterId"] = model.SemesterId;
                    break;
                case "subjectid":
                    dict["subjectId"] = model.SubjectId;
                    break;
                case "semester":
                    dict["semester"] = model.Semester;
                    break;
                case "subject":
                    dict["subject"] = model.Subject;
                    break;
                case "enrollments":
                    dict["enrollments"] = model.Enrollments;
                    break;
            }
        }

        return dict;
    }

    public static Dictionary<string, object?> ShapeSemesterFields(SemesterResponse model, IReadOnlyCollection<string> fields)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in fields)
        {
            var key = field.Trim();
            if (key.Length == 0)
            {
                continue;
            }

            switch (key.ToLowerInvariant())
            {
                case "semesterid":
                    dict["semesterId"] = model.SemesterId;
                    break;
                case "semestername":
                    dict["semesterName"] = model.SemesterName;
                    break;
                case "startdate":
                    dict["startDate"] = model.StartDate;
                    break;
                case "enddate":
                    dict["endDate"] = model.EndDate;
                    break;
                case "courses":
                    dict["courses"] = model.Courses;
                    break;
            }
        }

        return dict;
    }

    public static Dictionary<string, object?> ShapeSubjectFields(SubjectResponse model, IReadOnlyCollection<string> fields)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in fields)
        {
            var key = field.Trim();
            if (key.Length == 0)
            {
                continue;
            }

            switch (key.ToLowerInvariant())
            {
                case "subjectid":
                    dict["subjectId"] = model.SubjectId;
                    break;
                case "subjectcode":
                    dict["subjectCode"] = model.SubjectCode;
                    break;
                case "subjectname":
                    dict["subjectName"] = model.SubjectName;
                    break;
                case "credit":
                    dict["credit"] = model.Credit;
                    break;
                case "courses":
                    dict["courses"] = model.Courses;
                    break;
            }
        }

        return dict;
    }
}
