namespace InformationSystem.Domain.Models
{
    public class Incident
    {
        public Guid IncidentId { get; set; }
        public string IncidentName { get; set; }
        public DateTime IncidentDate { get; set; }
        public string IncidentDescription { get; set; }
    }
}