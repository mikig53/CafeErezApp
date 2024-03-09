using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetModels.Interfaces;
using BetModels.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace SendToShop.SignalR;
public class SignalRFormSender : IFormSender
{
    private readonly HubConnection _hubConnection;

    public SignalRFormSender()
    {
        
        // Build the connection
        _hubConnection = new HubConnectionBuilder()
          .WithUrl(" https://cafeerez.btunnel.co.in/formhub") // btunnel
      //  .WithUrl("https://xfgrdwh.localto.net.localto.net/formhub") // localtonet
         //  .WithUrl("https://b4tqt25z-5011.uks1.devtunnels.ms/formhub") // visual studio
      .WithUrl("https://prepared-bug-sure.ngrok-free.app/formhub")  // ngrok
             .WithAutomaticReconnect()
              .ConfigureLogging(logging =>
              {
                  logging.AddConsole();
                  logging.SetMinimumLevel(LogLevel.Trace);
              })
            .Build();
        _hubConnection.Reconnecting += error =>
        {
            Console.WriteLine($"Connection lost due to error: {error.ToString()}");
            return Task.CompletedTask;
        };
    }

    public async Task SendForm(Form form)
    {
        // Ensure the connection is started
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await _hubConnection.StartAsync();
                

                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;

            }
            
            
        }
        if (_hubConnection.State == HubConnectionState.Connected) 
        // Call the SendForm method on the server
             await _hubConnection.SendAsync("SendForm", form);
    }
}
