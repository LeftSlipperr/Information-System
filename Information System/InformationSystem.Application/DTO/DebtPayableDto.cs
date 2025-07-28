using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class DebtPayableDto
    {
        public Guid DebtPayableId { get; set; }
        public string CreditorName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}
