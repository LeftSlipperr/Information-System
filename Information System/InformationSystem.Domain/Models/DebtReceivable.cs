namespace InformationSystem.Domain.Models
{
    public class DebtReceivable
    {
        public Guid DebtReceivableId { get; set; }
        public string DebtorName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public Guid IncomeId { get; set; }

        public Income Income { get; set; }

    }
}