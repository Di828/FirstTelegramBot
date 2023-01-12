using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstTelegramBot.Models
{
    public class CurrencyParseData
    {
        public string Currency { get; set; } = "";
        public string Message { get; set; } = "";
        public bool IsParsed { get; set; }        
    }
}
