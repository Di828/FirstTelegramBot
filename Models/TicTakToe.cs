using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstTelegramBot.Models
{
    public class TicTakToe
    {

        public string[,] PlayingField =
        {
            {"   ", "   ", "   " },
            {"   ", "   ", "   " },
            {"   ", "   ", "   " }
        };
        public string[,] PlayingFieldPresentation =
        {
            {"   "," ","    A"," ","   B   "," ","C"," "},
            {"   ","  -------","—","-","-","-","----"," "},
            {" 1 ","| ","   "," | ","   "," | ","   "," |"},
            {"   ","  -------","—","-","-","-","----"," "},
            {" 2 ","| ","   "," | ","   "," | ","   "," |"},
            {"   ","  -------","—","-","-","-","----"," "},
            {" 3 ","| ","   "," | ","   "," | ","   "," |"},
            {"   ","  -------","—","-","-","-","----"," "},
        };
        public string? Role { get; set; }
        public string? OpponentRole { get; set; }        
    }
}
