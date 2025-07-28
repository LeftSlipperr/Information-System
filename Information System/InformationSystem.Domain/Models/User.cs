namespace InformationSystem.Domain.Models;
public class User
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }

    public ICollection<Log> Logs { get; set; }
}