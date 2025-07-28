using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class RawMaterialWarehouseDto
    {
        public Guid MaterialId { get; set; }
        public string MaterialName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchaseCost { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
