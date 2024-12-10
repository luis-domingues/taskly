namespace Taskly.Exceptions;

public class IncorrectPassword: Exception
{
    public IncorrectPassword(string message) : base(message) {  }
}