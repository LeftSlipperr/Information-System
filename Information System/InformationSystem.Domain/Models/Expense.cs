namespace InformationSystem.Domain.Models
{
    public class Expense
    {
        public Guid ExpenseId { get; set; }
        public string ExpenseName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }


        public Guid BalanceAnalysisId { get; set; }

        public BalanceAnalysis BalanceAnalysis { get; set; }
        public Salary Salary { get; set; }
        public TradePointEconomics TradePointEconomics { get; set; }
        public ICollection<RawMaterialWarehouse> RawMaterialWarehouses { get; set; }
        public ICollection<DebtPayable> DebtPayables { get; set; }
    }
}
