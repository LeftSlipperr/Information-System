namespace InformationSystem.Domain.Models
{

    public class FinishedGoodsWarehouse
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal PotentialRevenue { get; set; }
        public DateTime LastUpdated { get; set; }
        public Guid IncomeId { get; set; }

        public Income Income { get; set; }
    }
}