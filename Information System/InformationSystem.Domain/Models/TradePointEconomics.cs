namespace InformationSystem.Domain.Models
{
    public class TradePointEconomics
    {
        public Guid TradePointEconomicsId { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Profit { get; set; }
        public DateTime Period { get; set; }
        public Guid TradePointId { get; set; }

        public TradePoint TradePoint { get; set; }

        public Guid ExpenseId { get; set; }

        public Expense Expense { get; set; }

        public Guid IncomeId { get; set; }

        public Income Income { get; set; }
    }
}