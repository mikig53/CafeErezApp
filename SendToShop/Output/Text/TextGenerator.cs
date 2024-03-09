using BetModels.Interfaces;
using BetModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;
using Form = BetModels.Models.Form;

namespace SendToShop.Output.Text;
public class TextGenerator : ITextService
{
    private readonly IFileSystemService _fileSystemService;
    public TextGenerator(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;

    }
    public async Task SaveAsTextAsync(Form form)
    {
        var outputDirectory = _fileSystemService.GetAppDataDirectory();
        var textFilename = Path.Combine(outputDirectory, "forms.txt");

        try
        {
            DateTime lastWriteTime = File.GetLastWriteTime(textFilename);
            File.WriteAllText(textFilename, string.Empty);
            // Compare the last write time with the current date
            if (lastWriteTime.Date < DateTime.Today)
            {
                // The file is from a previous day, so clear it
                File.WriteAllText(textFilename, string.Empty);
            }
            using (var writer = new StreamWriter(textFilename, true))
            {

                // Write the form data to the CSV file
                DateTime now = DateTime.Now;
                string formattedDate = now.ToString("dd/MM/yyyy HH:mm");
                writer.WriteLine($"{formattedDate},{form.BetAmount},{form.TelephoneNumber},{form.LastName},{form.FirstName}");
                // Add a delimiter between forms
                writer.WriteLine("===USER DETAILS DELIMITER===");

                foreach (var bet in form.Bets)
                {
                    string formattedLine = $"{bet.BetValue},{bet.Ratio},{bet.DescriptionGame},({bet.GameNumber}),{bet.Single},{bet.SingleAmount}, {bet.Info}";
                    writer.WriteLine(formattedLine);
                }

                // Add a delimiter between forms
                writer.WriteLine("===FORM DELIMITER===");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task<List<Form>> ReadAsTextAsync()
    {
        var outputDirectory = _fileSystemService.GetAppDataDirectory();
        var csvFilename = Path.Combine(outputDirectory, "forms.txt");
        List<BetModels.Models.Form> forms = new List<Form>();

        try
        {
            using (var reader = new StreamReader(csvFilename))
            {
                string line;
                Form currentForm = new Form();
                string details = "user";

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line == "===USER DETAILS DELIMITER===")
                    {
                        
                        details = "bets";
                        continue;
                    }

                    if (line == "===FORM DELIMITER===")
                    {
                        forms.Add(currentForm);
                        currentForm = new Form();
                        details = "user";
                        continue;
                    }
                    
                   
                    switch (details)
                    {

                        case "user":
                            string[] formsDetails = line.Split(',');
                            currentForm.ReceivedDate = formsDetails[0];
                            currentForm.BetAmount = Decimal.Parse(formsDetails[1]);
                            currentForm.FirstName = formsDetails[4];
                            currentForm.LastName = formsDetails[3];
                            currentForm.TelephoneNumber = formsDetails[2];
                            break;

                        case "bets":
                            string[] values = line.Split(',');
                            Bet bet = new Bet();
                            bet.BetValue = values[0];
                            bet.Ratio = values[1];
                            bet.DescriptionGame = values[2];
                            bet.Info = values[6];
                            Boolean.TryParse(values[4], out bool singleBool);
                            bet.Single = singleBool;
                            if (int.TryParse(values[5], out int singleInt))
                                bet.SingleAmount = singleInt;
                            else bet.SingleAmount = 0;
                            // Remove surrounding parentheses from GameNumber using regular expression
                            string gameNumber = values[3];
                            gameNumber = Regex.Replace(gameNumber, @"^\(|\)$", "");
                            bet.GameNumber = gameNumber;
                            currentForm.Bets.Add(bet);
                            break;
                        case "add":
                            forms.Add(currentForm);
                            details = "bets";
                            break;

                        default:
                           break;
                    }

                }

            }
                      
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return forms;
    }

    public async Task DeleteOldFormsAsync()
    {
        var outputDirectory = _fileSystemService.GetAppDataDirectory();
        var csvFilename = Path.Combine(outputDirectory, "forms.txt");

        // Get the creation date of the file
        var creationDate = await Task.Run(() => File.GetCreationTime(csvFilename));

        // Check if the file was not created today
        if (creationDate.Date != DateTime.Today)
        {
            // Delete the file
            await Task.Run(() => File.Delete(csvFilename));
        }
    }
}
