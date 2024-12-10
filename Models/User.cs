using System.ComponentModel.DataAnnotations;

namespace Taskly.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength]
    public string FullName { get; set; }
    [Required]
    [MinLength(6)]
    [MaxLength(30)]
    public string Username { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(6)]
    [MaxLength(256)]
    public string Password { get; set; }
    [Required]
    [MaxLength(30)]
    public string TitleJob { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
}