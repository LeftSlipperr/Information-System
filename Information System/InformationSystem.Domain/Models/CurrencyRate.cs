using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CurrencyRate
    {
        public string Flag { get; set; }   // URL или путь к изображению
        public string Name { get; set; }   // Название валюты
        public decimal Rate { get; set; }  // Курс
    }


}
