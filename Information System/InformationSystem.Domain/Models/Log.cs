namespace InformationSystem.Domain.Models
{
    public class Log
    {
        public Guid LogId { get; set; } = Guid.NewGuid();
        public string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}