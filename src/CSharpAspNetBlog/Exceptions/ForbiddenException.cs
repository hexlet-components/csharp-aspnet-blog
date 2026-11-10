namespace CSharpAspNetBlog.Exceptions;

public class ForbiddenException(string message) : Exception(message);
