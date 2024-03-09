using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetModels.Models;
public class TelesportBet
{
    public string Number { get; set; }// bet number in the form
    public bool Active { get; set; } // game is active yes/no 
    public string Status { get; set; }// finished/current game time/will start game time
    public string Description { get; set; } // Home team
    public string Type { get; set; }// S-single, D-duble
    public ButtonBet Bet1 { get; set; }// bate rate for 1
    public ButtonBet BetX { get; set; }// bate rate for X
    public ButtonBet Bet2 { get; set; }// bate rate for 2
    public string Score { get; set; }// score of the game
    public string WinBet { get; set; } // 2|x|1
    public bool EligibleBet { get; set; } // can bet 
    public string Description2 { get; set; } // Guest team
    public string Note { get; set; } // Additional details
    
}
