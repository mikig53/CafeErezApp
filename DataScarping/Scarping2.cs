using BetModels.Interfaces;
using BetModels.Models;

using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace Scarp;
public class Scarping2 : IScarp
{
    public List<TelesportCategory> debtList = new List<TelesportCategory>();
    static int[] count = new int[15];
    public async Task<List<TelesportCategory>> ScarpAsync()
    {
        // Initialize HTMLDocument object with URL
        HtmlDocument document = new HtmlDocument();
       // string url = ("https://m.telesport.co.il/winnerboard.aspx");
        string url = ("https://www.telesport.co.il/%D7%90%D7%96%D7%95%D7%A8%20%D7%95%D7%95%D7%99%D7%A0%D7%A8");
    
        var web = new HtmlWeb();
        HtmlDocument doc = await web.LoadFromWebAsync(url);

        // Read the text contents of the HTML format
 //       var text = doc.DocumentNode.InnerHtml;

        var winnerTable = doc.DocumentNode.SelectNodes("//table[@class=\"winnerTable\"]//tr");


        TelesportCategory? debts = null;
        foreach (var item in winnerTable)
        {
            if (item.Attributes["class"]?.Value == "winnerHeaderTr")
            {
                // Select the img element using XPath relative to the current tr
                HtmlNode imgNode = item.SelectSingleNode(".//img[@class=\"winnerCountryFlagImg\"]");
                string? srcValue = default;

                if (imgNode != null)
                {
                    // Get the value of the src attribute
                    srcValue = imgNode.GetAttributeValue("src", "");
                }
                string nameValue = item.SelectSingleNode(".//h3 [@class=\"winnerCountryName\"]").InnerText;
                if (nameValue.EndsWith("'")) // from xyz. to .xyz
                {
                    nameValue = "'" + nameValue.TrimEnd('\'');
                }
                MatchCollection matches99 = Regex.Matches(nameValue, @"(.+)\s([A-z]+)\s(.+)"); // Winner סל
                if (matches99.Count > 0)
                {
                    nameValue = matches99[0].Groups[3].Value + " " + matches99[0].Groups[2].Value + " " + matches99[0].Groups[1].Value;

                }
                 matches99 = Regex.Matches(nameValue, @"(.+)\s([A-Z]+)$"); // NBA
                if (matches99.Count > 0)
                {
                    nameValue = matches99[0].Groups[2].Value + " - " + matches99[0].Groups[1].Value.Trim('-');

                }
                debts = new TelesportCategory
                {
                    ImgFlag = srcValue, // image flag country
                    Name = nameValue, // Title
                  
                    Tags = new List<TelesportBet>()
                };
                // in case the category not contains any item
                if (debtList.Count > 0)
                {
                    var lastItem = debtList[debtList.Count - 1];
                    if (lastItem.Tags.Count == 0)
                    {
                        debtList.Remove(lastItem);
                    }
                }
               
                debtList.Add(debts);
            }
            else if (item.Attributes["id"]?.Value.Contains("winnerRowBet") == true)
            {
                // game number
                var tdElement = item.SelectSingleNode(".//td[@class='tdWinId']");
                var spanElement = tdElement.SelectSingleNode("span");

                var textValue = tdElement.LastChild.InnerText.Trim(); // game number in the form xyz.
                var spanValue = spanElement?.InnerText.Trim(); // number day of week (d)
                if (textValue.EndsWith(".")) // from xyz. to .xyz
                {
                    textValue = "." + textValue.TrimEnd('.');
                }
                var newValue = textValue + " " + spanValue;

                // Description
                var htmlEment = item.SelectSingleNode(".//td[@class=\"th_td_WinnerteamsAndBetType\"]").InnerHtml;
                string[] desc1desc2 = new string[3];
                desc1desc2 = ExtractTextFromNode(htmlEment);
                // Extract team1, team2
               
                
                // Score
                var htmlValue = item.SelectSingleNode(".//td[@class=\"tdWinScore\"]").InnerText;
                string[] scoreValue = new string[2]; 
                //if (htmlValue.Length >4) 
                 scoreValue = ExtractTextFromNode(htmlValue);
                // in case no bet for X
                string data1X2 = item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_X\"]")?.InnerText ?? string.Empty;
                var debt = new TelesportBet
                {
                    Number = newValue, // bet number in the form
                    Status = item.SelectSingleNode(".//td[@class=\"th_td_WinnerStatus tdWinnerStatus\"]").InnerText, // finished/current game time/start game time /canceled
                    Description = desc1desc2[0], // team1 
                    Description2 = desc1desc2[1], // team2
                    Note = desc1desc2[2], // remarks
                    Type = item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_SD td_WinnerBet_SD\"]").InnerText, // single,duble
                    WinBet = "", // who won
                    Active = !item.SelectSingleNode(".//td[@class=\"th_td_WinnerActive\"]").InnerHtml.Contains("blinkingbullet_off"), // blinkingbullet=> live
                    Bet1 = new ButtonBet
                    {
                        Data1X2 = item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_1\"]")?.InnerText ?? string.Empty,  // Initialize  with source data
                        IsWon = Regex.IsMatch(item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_1\"]").InnerHtml, @"wr_winner"),
                        IsClicked = false // Initialize with false
                    }, // bate rate for 1
                    BetX= new ButtonBet
                    {
                        Data1X2 = data1X2,
                        IsWon = !string.IsNullOrEmpty(data1X2) && Regex.IsMatch(item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_X\"]").InnerHtml, @"wr_winner"),
                        IsClicked = false // Initialize with false
                    },
                    Bet2 = new ButtonBet
                    {
                        Data1X2 = item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_2\"]")?.InnerText ?? string.Empty,  // Initialize with source data
                        IsWon = Regex.IsMatch(item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_2\"]").InnerHtml, @"wr_winner"),
                        IsClicked = false // Initialize with false
                    },
                    //BetX = item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_X\"]")?.InnerText ?? string.Empty, // bate rate for x
                    //Bet2 = item.SelectSingleNode(".//td[@class=\"th_td_WinnerBet_2\"]").InnerText, // bate rate for x
                    Score = scoreValue[0], // score of the game

                };
               
                // no data for bet
                if (debt.Bet1.Data1X2 == "-")
                    debt.Bet1.Data1X2 = "";
                if (debt.Bet2.Data1X2 == "-")
                    debt.Bet2.Data1X2 = "";
                if (debt.BetX.Data1X2 == "-")
                    debt.BetX.Data1X2 = "";
                if (Regex.IsMatch(debt.Status, @"\d{2}:\d{2}") && !debt.Active && !debt.Status.Contains("נסגר"))// not yet started, can bet this game
                    debt.EligibleBet = true;
                if (!(debt.Bet1.Data1X2 == "" && debt.Bet1.Data1X2 == "" && debt.Bet1.Data1X2 == ""))
                   debts.Tags.Add(debt);
            }
            else
            {
            }
        }
        
        return debtList;
    }

    public static string[] ExtractTextFromNode(string nodeElement)
    {
        string[] desc1desc2 = new string[3];

       
        if (!nodeElement.Contains("<span") && (!nodeElement.Contains("(")))// text1 - text2
        {
            // convert score from xx - yy to yy - xx (right to left problem)
            MatchCollection matches4 = Regex.Matches(nodeElement, @"(\d+)\s-\s(\d+)");
            if (matches4.Count > 0)
            {
                count[1]++;

                desc1desc2[0] = matches4[0].Groups[2].Value.Trim() + " - " + matches4[0].Groups[1].Value.Trim();
                return desc1desc2;
            }
            else // text1 - text2 or score as single number
            {
                count[2]++;
                if (nodeElement.Contains("-"))
                {
                    MatchCollection matches11 = Regex.Matches(nodeElement, @"(.+)\s-\s(.+)");// 
                    if (matches11.Count > 0)
                    {
                        desc1desc2[0] = matches11[0].Groups[1].Value; // team1
                        if (desc1desc2[0].EndsWith("'")) // from xyz. to .xyz
                        {
                            desc1desc2[0] = "'" + desc1desc2[0].TrimEnd('\'');
                        }
                        desc1desc2[1] = matches11[0].Groups[2].Value; // team2
                        if (desc1desc2[1].EndsWith("'")) // from xyz. to .xyz
                        {
                            desc1desc2[1] = "'" + desc1desc2[1].TrimEnd('\'');
                        }
                    }
                     
                }
                else
                {
                    desc1desc2[0] = nodeElement;
                    if (desc1desc2[0].EndsWith("'")) // דגוקוביץ'
                    {
                        desc1desc2[0] = "'" + desc1desc2[0].TrimEnd('\'');
                    }
                    desc1desc2[1] = "";
                }
                
                return desc1desc2;
            }
                
        }
        if (!nodeElement.Contains("<span"))
        {
            // text (+n) => (+n) text
            MatchCollection matches1 = Regex.Matches(nodeElement, @"(.+)-\s(.+)\s(\(.+\))$");// 

            if (matches1.Count > 0)
            {
                count[3]++;
                desc1desc2[0] = matches1[0].Groups[1].Value; // team1
                if (desc1desc2[0].EndsWith("'")) 
                {
                    desc1desc2[0] = "'" + desc1desc2[0].TrimEnd('\'');
                }
                string newVal = default;
                if (matches1[0].Groups[2].Value.EndsWith("'"))
                {
                     newVal  = "'" + matches1[0].Groups[2].Value.TrimEnd('\'');
                    desc1desc2[1] = matches1[0].Groups[3].Value + " " + newVal; // team2
                }
                else
                {
                    desc1desc2[1] = matches1[0].Groups[3].Value + " " + matches1[0].Groups[2].Value; // team2
                }
                
                return desc1desc2;
            }
            // text1 (+n) - text2 => (+n) text2 - text1

            MatchCollection matches55 = Regex.Matches(nodeElement, @"(.+)(\(.+\))\s-\s(.+)");
            if (matches55.Count > 0)
            {
                count[4]++;
                string newVal = default;
                if (matches55[0].Groups[1].Value.EndsWith("'")) // from xyz. to .xyz
                {
                    newVal = "'" + matches55[0].Groups[1].Value.TrimEnd('\'');
                    desc1desc2[0] = matches55[0].Groups[2].Value + " " + newVal; // team1
                }
                else
                {
                    desc1desc2[0] = matches55[0].Groups[2].Value + " " + matches55[0].Groups[1].Value; // team1

                }
                desc1desc2[1] = matches55[0].Groups[3].Value; // team2
                if (desc1desc2[1].EndsWith("'")) // from xyz. to .xyz
                {
                    desc1desc2[1] = "'" + desc1desc2[1].TrimEnd('\'');
                }
                return desc1desc2;
            }
        }
        // text (+n) => (+n) text
        MatchCollection matches6 = Regex.Matches(nodeElement, @"<span.+>(.+)</span>(.+)-\s(.+)\s(\(.+\))$");//<span class="spanWinnerBetType">רבע ראשון </span>בוסניה-הרצגובינה - פולין (‎+1)

        if (matches6.Count > 0)
        {
            count[5]++;
            string newVal = default;
            if (matches6[0].Groups[3].Value.EndsWith("'")) //
            {
                newVal = "'" + matches6[0].Groups[3].Value.TrimEnd('\'');
                desc1desc2[1] = matches6[0].Groups[4].Value + " " + newVal; // team2
            }
            else
            {
                desc1desc2[1] = matches6[0].Groups[4].Value + " " + matches6[0].Groups[3].Value; // team2
            }

            desc1desc2[0] =  matches6[0].Groups[2].Value  ; // team1
            if (desc1desc2[0].EndsWith("'")) 
            {
                desc1desc2[0] = "'" + desc1desc2[0].TrimEnd('\'');
            }
            desc1desc2[2] = matches6[0].Groups[1].Value;
            return desc1desc2;
        }
        MatchCollection matches5 = Regex.Matches(nodeElement, @"<span.+>(.+)</span>(.+)(\(.+\))\s-\s(.+)");//<span class="spanWinnerBetType">יתרון משחקונים </span>אלכסנדר זברב (‎+4.5) - נובאק דג'וקוביץ
        if (matches5.Count > 0)
        {
            count[6]++;
            string newVal = default;
            if (matches5[0].Groups[2].Value.EndsWith("'")) //
            {
                newVal = "'" + matches5[0].Groups[2].Value.TrimEnd('\'');
                desc1desc2[0] = matches5[0].Groups[3].Value + " " + newVal; // team1
            }
            else
            {
                desc1desc2[0] = matches5[0].Groups[3].Value + " " + matches5[0].Groups[2].Value; // team1
            }
            
            desc1desc2[1] = matches5[0].Groups[4].Value; // team2
            if (desc1desc2[1].EndsWith("'")) // from xyz. to .xyz
            {
                desc1desc2[1] = "'" + desc1desc2[1].TrimEnd('\'');
            }
            desc1desc2[2] = matches5[0].Groups[1].Value;
            return desc1desc2;
        }
        MatchCollection matches99 = Regex.Matches(nodeElement, @"<span.+>(.+)</span>(.+)\s-\s(.+)");//<span class="spanWinnerBetType">מחצית ראשונה </span>נאשוויל - אינטר מיאמי
        if (matches99.Count > 0)
        {
            count[7]++;
            desc1desc2[0] = matches99[0].Groups[2].Value; // team1
            if (desc1desc2[0].EndsWith("'")) // from xyz. to .xyz
            {
                desc1desc2[0] = "'" + desc1desc2[0].TrimEnd('\'');
            }
            desc1desc2[1] = matches99[0].Groups[3].Value; // team2
            if (desc1desc2[1].EndsWith("'")) // from xyz. to .xyz
            {
                desc1desc2[1] = "'" + desc1desc2[1].TrimEnd('\'');
            }
            desc1desc2[2] = matches99[0].Groups[1].Value;
            MatchCollection matches89 = Regex.Matches(desc1desc2[2], @"(.+)\s([A-Z]+\s.+)"); // NBA
            if (matches89.Count > 0)
            {
                desc1desc2[2] = matches89[0].Groups[2].Value + " " + matches89[0].Groups[1].Value;

            }
            return desc1desc2;

        }
        MatchCollection matches88 = Regex.Matches(nodeElement, @"<span.+>(.+)</span>(\w+\s\w+)");//<span class="spanWinnerBetType">מעל/מתחת שערים (1)&nbsp;</span>רוברט לבנדובסקי
        if (matches88.Count > 0)
        {
            count[8]++;
            string newVal = default;
            if (matches88[0].Groups[2].Value.EndsWith("'")) //
            {
                newVal = "'" + matches88[0].Groups[2].Value.TrimEnd('\'');
                desc1desc2[0] = matches88[0].Groups[3].Value + " " + newVal; // team1
            }
            else
            {
                desc1desc2[0] = matches88[0].Groups[3].Value + " " + matches88[0].Groups[2].Value; // team1
            }
            
            desc1desc2[1] = "";
            desc1desc2[2] = matches88[0].Groups[1].Value;
            MatchCollection matches89 = Regex.Matches(desc1desc2[2], @"(.+)\s([A-Z]+)"); // NBA
            if (matches89.Count > 0)
            {
                desc1desc2[2] = matches89[0].Groups[2].Value + " " + matches89[0].Groups[1].Value;
               
            }
            return desc1desc2;

        }
        
        

        //(+n) text => (+n) text only one team
        MatchCollection matches61 = Regex.Matches(nodeElement, @"<span.+>(.+)</span>(.+^-)$"); //<span class="spanWinnerBetType">מעל/מתחת שערים (1)&nbsp;</span>רוברט לבנדובסקי

        if (matches61.Count > 0)
        {
            count[9]++;
            desc1desc2[0] = matches61[0].Groups[2].Value; // team1
            if (desc1desc2[0].EndsWith("'")) // from xyz. to .xyz
            {
                desc1desc2[0] = "'" + desc1desc2[0].TrimEnd('\'');
            }
            desc1desc2[2] = matches61[0].Groups[1].Value;
            return desc1desc2;
        }
        

        
        
        
        // text1 - text2 (+n)=> (+n) text2 - text1
        MatchCollection matches60 = Regex.Matches(nodeElement, @"(.+)\s([A-Z]+)\s-\s(.+)\s(\(.+\))$");//
        if (matches60.Count > 0)
        {
            count[10]++;
            string newVal = default;
            if (matches60[0].Groups[1].Value.EndsWith("'")) //
            {
                newVal = "'" + matches60[0].Groups[1].Value.TrimEnd('\'');
                desc1desc2[0] = matches60[0].Groups[2].Value + " " + newVal; // team1
            }
            else
            {
                desc1desc2[0] = matches60[0].Groups[2].Value + " " + matches60[0].Groups[1].Value; // team1
            }
            
            if (matches60[0].Groups[3].Value.EndsWith("'")) //
            {
                newVal = "'" + matches60[0].Groups[3].Value.TrimEnd('\'');
                desc1desc2[1] = matches60[0].Groups[4].Value + " " + newVal; // team2
            }
            else
            {
                desc1desc2[1] = matches60[0].Groups[4].Value + " " + matches60[0].Groups[3].Value; // team2
            }
            
            return desc1desc2;
        }
      
        // NBA
        MatchCollection matches7 = Regex.Matches(nodeElement, @"(.+)\s([A-Z]+\s\(.+\))\s(.+)-(.+)");//
        if (matches7.Count > 0)
        {
            count[11]++;
            string newVal = default;
            if (matches7[0].Groups[1].Value.EndsWith("'")) //
            {
                newVal = "'" + matches7[0].Groups[1].Value.TrimEnd('\'');
                desc1desc2[0] = matches7[0].Groups[3].Value + " " + matches7[0].Groups[2].Value + " " + newVal; // team1
            }
            else
            {
                desc1desc2[0] = matches7[0].Groups[3].Value + " " + matches7[0].Groups[2].Value + " " + matches7[0].Groups[1].Value; // team1
            }
            
            return desc1desc2;
        }


        MatchCollection matches8 = Regex.Matches(nodeElement, @"(.+)\s([A-Z]+)\s-(.+)");//
        if (matches8.Count > 0)
        {
            count[12]++;
            string newVal = default;
            if (matches8[0].Groups[1].Value.EndsWith("'")) //
            {
                newVal = "'" + matches8[0].Groups[1].Value.TrimEnd('\'');
                desc1desc2[0] = matches8[0].Groups[2].Value + " " + newVal; // team1
            }
            else
            {
                desc1desc2[0] = matches8[0].Groups[2].Value + " " + matches8[0].Groups[1].Value; // team1
            }
            
         
            desc1desc2[1] = matches8[0].Groups[3].Value ; // team2
            if (desc1desc2[1].EndsWith("'")) //
            {
                desc1desc2[1] = "'" + desc1desc2[1].TrimEnd('\'');
            }
            
            return desc1desc2;
            
        }

       
        

        count[13]++;
        desc1desc2[0] = nodeElement;
        return desc1desc2;
     }

    

}


