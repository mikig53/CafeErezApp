using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorClassLibraryModels.Models;
public class BetList
{
    public string GameNo { get; set; }
    public decimal BetRatio { get; set; }
    public string Bet1X2 { get; set; }
    public string Game { get; set; }
    public string Game2 { get; set; }
    public string Type { get; set; }
    public int ClickedButton { get; set; }
    public string Info { get; set; }
}
