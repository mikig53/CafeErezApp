using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetModels.Models;
public  class ButtonBet
{
    public string Data1X2 { get; set; } // 1|X|2
    public bool IsWon { get; set; } // is bet successed
    public bool IsClicked { get; set; } // If clicked for bet or cancel the bet
}
