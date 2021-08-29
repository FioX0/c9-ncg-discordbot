using C9_NCG_DiscordBot.Commands;
using C9_NCG_DiscordBot.TipSystem;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using DSharpPlus.Interactivity.Extensions;

namespace C9_NCG_DiscordBot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Information,
            };

            Client = new DiscordClient(config);

            this.Client.Ready += this.OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(1)
            }
            );

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] {configJson.Prefix},
                EnableDms = true,
                EnableMentionPrefix = false,
                IgnoreExtraArguments = false,
                EnableDefaultHelp = false,
            };


            Commands = Client.UseCommandsNext(commandsConfig);

            this.Commands.RegisterCommands<QueryCommands>();
            this.Commands.RegisterCommands<TipCommands>();
            //this.Commands.RegisterCommands<HelpMe>();
            this.Commands.RegisterCommands<ReportCommands>();

            DSharpPlus.Entities.DiscordActivity activity = new DSharpPlus.Entities.DiscordActivity();
            activity.Name = "Loading";

            await Client.ConnectAsync(activity);

            Console.WriteLine("Running NCGBot V1.0.7.2");

            new Thread(() =>{extras.BackupDBAsync();}).Start();
            //new Thread(() => {Tips.PaymentProcessor(Client);}).Start();
            //extras.RunChain();
            //new Thread(() => { extras.DailyBlockReport(Client); }).Start();
            new Thread(() => { extras.Miningwhitelist(Client); }).Start();
            new Thread(() => { extras.UpdateStatusAsync(Client); }).Start();

            //extras.ShopDataAsync(Client);

            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            Console.WriteLine("Client Ready");
            return Task.CompletedTask;
        }
    }
}
