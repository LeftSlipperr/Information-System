namespace InformationSystem.Domain.Models 
{
    public class BalanceAnalysis
    {
        public Guid BalanceId { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal ProfitOrLoss { get; set; }
        public DateTime Period { get; set; }

        public ICollection<Income> Incomes { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
