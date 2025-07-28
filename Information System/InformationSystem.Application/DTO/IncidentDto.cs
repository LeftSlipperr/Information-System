using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class IncidentDto
    {
        public Guid IncidentId { get; set; }
        public string IncidentName { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
