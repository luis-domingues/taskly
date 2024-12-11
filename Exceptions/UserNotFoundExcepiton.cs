namespace Taskly.Exceptions;

public class UserNotFoundExcepiton : Exception
{
    public UserNotFoundExcepiton(string message) : base(message) {  }
}