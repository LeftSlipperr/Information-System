using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Application.DTO
{
    internal class LogDto
    {
        public Guid LogId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
