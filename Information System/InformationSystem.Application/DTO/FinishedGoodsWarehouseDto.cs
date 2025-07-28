using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class FinishedGoodsWarehouseDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PotentialRevenue { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
