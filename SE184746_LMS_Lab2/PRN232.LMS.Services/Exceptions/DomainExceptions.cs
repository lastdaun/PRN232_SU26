namespace PRN232.LMS.Services.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message) : base(message) { }
}

public class ResourceValidationException : Exception
{
    public ResourceValidationException(string message) : base(message) { }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message = "Invalid username or password.") : base(message) { }
}
