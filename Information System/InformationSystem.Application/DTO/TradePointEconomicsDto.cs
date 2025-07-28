using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class TradePointEconomicsDto
    {
        public Guid TradePointEconomicsId { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Profit { get; set; }
        public DateTime Period { get; set; }
    }
}
