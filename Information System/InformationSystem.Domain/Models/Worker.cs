namespace InformationSystem.Domain.Models;
public class Worker
{
    public Guid WorkerId { get; set; }
    public string Position { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string ThirdName { get; set; }
    
    public Guid SalaryId { get; set; }

    public Salary Salary { get; set; }
}