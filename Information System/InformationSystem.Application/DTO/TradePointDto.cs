using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class TradePointDto
    {
        public Guid TradePointId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
