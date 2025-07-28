using System.ComponentModel.DataAnnotations.Schema;

namespace InformationSystem.Domain.Models
{
    public class Income
    {
        public Guid IncomeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; }
        
        
        public Guid BalanceAnalysisId { get; set; }

        [ForeignKey("BalanceAnalysisId")]
        public BalanceAnalysis BalanceAnalysis { get; set; }
        
        public Guid TradePointEconomicsId { get; set; }
        public TradePointEconomics TradePointEconomics { get; set; }
        public ICollection<FinishedGoodsWarehouse> FinishedGoodsWarehouses { get; set; }
        public ICollection<DebtReceivable> DebtReceivables { get; set; }
    }
}
