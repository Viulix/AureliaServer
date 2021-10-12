using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Aurelia
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
        
        
        public async Task MainAsync()
        {
            // Collect all of the services we will need to begin.
            
            var services = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>();

            // Put all of the collected services into their reusable "box".
            var serviceProvider = services.BuildServiceProvider();

            var _commandService = serviceProvider.GetRequiredService<CommandService>();
            // In the Singleton pattern, an object is not created until it is first needed. 
            // Here we will ask the provider to give us all of the services, which will in turn create them.
            var _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            serviceProvider.GetRequiredService<CommandService>();
            serviceProvider.GetRequiredService<CommandHandler>();
            
            _client.Log += Log;

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);

            await _client.LoginAsync(TokenType.Bot, "");
            
            await _client.StartAsync();

            await _client.SetGameAsync("with you!", null);
            
            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
