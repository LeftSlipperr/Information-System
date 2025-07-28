namespace InformationSystem.Domain.Models;

public class Salary
{
    public Guid SalaryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }

    public Worker Worker { get; set; }
    
    public Guid ExpenseId { get; set; }

    public Expense Expense { get; set; }
}