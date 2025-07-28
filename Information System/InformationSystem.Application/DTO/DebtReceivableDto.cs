using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class DebtReceivableDto
    {
        public Guid DebtReceivableId { get; set; }
        public string DebtorName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}
