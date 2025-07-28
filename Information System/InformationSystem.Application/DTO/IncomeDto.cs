using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class IncomeDto
    {
        public Guid IncomeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; }
    }
}
