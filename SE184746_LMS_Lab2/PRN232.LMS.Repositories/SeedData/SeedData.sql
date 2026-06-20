-- PRN232 LMS seed data (run once when database is empty)
IF EXISTS (SELECT 1 FROM dbo.Semesters)
    RETURN;

SET NOCOUNT ON;

INSERT INTO dbo.Semesters (SemesterName, StartDate, EndDate)
VALUES
    (N'Spring 2024', '2024-01-01', '2024-04-30'),
    (N'Summer 2024', '2024-05-01', '2024-08-31'),
    (N'Fall 2024',   '2024-09-01', '2024-12-31'),
    (N'Spring 2025', '2025-01-01', '2025-04-30'),
    (N'Summer 2025', '2025-05-01', '2025-08-31');

INSERT INTO dbo.Subjects (SubjectCode, SubjectName, Credit)
VALUES
    ('PRN232', N'Advanced Cross-Platform Application Programming With .NET', 3),
    ('SWT301', N'Software Testing', 3),
    ('SWP391', N'Application Development Project', 3),
    ('DBI202', N'Database Systems', 3),
    ('PRO192', N'Object-Oriented Programming', 3),
    ('CSD201', N'Data Structures and Algorithms', 3),
    ('PRJ301', N'Java Web Application Development', 3),
    ('FER202', N'Front-End Web Development', 3),
    ('MAS291', N'Statistics and Probability', 3),
    ('WED201', N'Web Design', 3);

;WITH nums AS (
    SELECT TOP (50) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i
    FROM sys.all_objects
)
INSERT INTO dbo.Students (FullName, Email, DateOfBirth)
SELECT
    CONCAT(N'Student ', RIGHT(CONCAT('0', CAST(i AS varchar(2))), 2)),
    CONCAT('student', RIGHT(CONCAT('0', CAST(i AS varchar(2))), 2), '@fpt.edu.vn'),
    DATEFROMPARTS(2002 + (i % 5), (i % 12) + 1, (i % 28) + 1)
FROM nums;

;WITH nums AS (
    SELECT TOP (20) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i
    FROM sys.all_objects
)
INSERT INTO dbo.Courses (CourseName, SemesterId, SubjectId)
SELECT
    CONCAT(s.SubjectCode, N' - Class ', RIGHT(CONCAT('0', CAST(n.i AS varchar(2))), 2)),
    ((n.i - 1) % 5) + 1,
    ((n.i - 1) % 10) + 1
FROM nums AS n
INNER JOIN dbo.Subjects AS s ON s.SubjectId = ((n.i - 1) % 10) + 1;

;WITH nums AS (
    SELECT TOP (500) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects AS a
    CROSS JOIN sys.all_objects AS b
),
statuses AS (
    SELECT 1 AS idx, 'Active' AS StatusName UNION ALL
    SELECT 2, 'Completed' UNION ALL
    SELECT 3, 'Dropped' UNION ALL
    SELECT 4, 'Pending'
)
INSERT INTO dbo.Enrollments (StudentId, CourseId, EnrollDate, Status)
SELECT
    1 + (ABS(CHECKSUM(NEWID())) % 50),
    1 + (ABS(CHECKSUM(NEWID())) % 20),
    DATEADD(DAY, -(1 + (ABS(CHECKSUM(NEWID())) % 365)), SYSUTCDATETIME()),
    (SELECT StatusName FROM statuses WHERE idx = 1 + (ABS(CHECKSUM(NEWID())) % 4))
FROM nums;
