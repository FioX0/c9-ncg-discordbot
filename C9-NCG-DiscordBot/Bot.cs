using C9_NCG_DiscordBot.Commands;
using C9_NCG_DiscordBot.TipSystem;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextModule Commands { get; private set; }

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
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(1)
            }
            );

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefix = new string(configJson.Prefix),
                EnableDms = true,
                EnableMentionPrefix = false,
                IgnoreExtraArguments = false,
            };


            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<QueryCommands>();
            Commands.RegisterCommands<TipCommands>();

            await Client.ConnectAsync();
            Tips.PaymentProcessor(Client);
            await Task.Delay(-1);
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
