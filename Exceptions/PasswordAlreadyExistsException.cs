namespace Taskly.Exceptions;

public class PasswordAlreadyExistsException : Exception
{ 
    public PasswordAlreadyExistsException(string message) : base(message) {  }
}