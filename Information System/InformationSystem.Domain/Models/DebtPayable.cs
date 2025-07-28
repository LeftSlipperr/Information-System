namespace InformationSystem.Domain.Models;
public class DebtPayable
{
    public Guid DebtPayableId { get; set; }
    public string CreditorName { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public Guid ExpenseId { get; set; }

    public Expense Expense { get; set; }
}