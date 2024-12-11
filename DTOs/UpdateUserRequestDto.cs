namespace Taskly.DTOs;

public class UpdateUserRequestDto
{
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string TitleJob { get; set; }
}