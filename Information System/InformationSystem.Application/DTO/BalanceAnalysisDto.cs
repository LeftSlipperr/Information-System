using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class BalanceAnalysisDto
    {
        public Guid BalanceId { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal ProfitOrLoss { get; set; }
        public DateTime Period { get; set; }
    }
}
