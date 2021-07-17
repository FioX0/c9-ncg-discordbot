using C9_NCG_DiscordBot.Handlers.Step;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.Handlers
{
    public class DialogueHandler
    {
        private readonly DiscordClient _client;
        private readonly DiscordChannel _channel;
        private readonly DiscordUser _user;
        private  IDialogueStep _currentStep;

        public DialogueHandler(DiscordClient client, DiscordChannel channel, DiscordUser user, IDialogueStep startingstep)
        {
            _client = client;
            _channel = channel;
            _user = user;
            _currentStep = startingstep;
        }

        public static List<DiscordMessage> messages = new List<DiscordMessage>();

        public async Task<bool> ProcessDialogue()
        {
            while(_currentStep != null)
            {
                _currentStep.OnMessageAdded += (message) => messages.Add(message);

                bool canceled = await _currentStep.ProcessStep(_client, _channel, _user).ConfigureAwait(false);

                if(canceled)
                {
                    await DeleteMessages().ConfigureAwait(false);

                    var cancelEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Help no longer required"
                    };
                    await _channel.SendMessageAsync(embed: cancelEmbed).ConfigureAwait(false);
                    return false;
                }

                _currentStep = _currentStep.NextStep;
            }
            await DeleteMessages().ConfigureAwait(false);
            return true;
        }

        private async Task DeleteMessages()
        {
            if(_channel.IsPrivate) { return; }

            foreach(var message in messages)
            {
                await message.DeleteAsync().ConfigureAwait(false);
            }
            messages.Clear();
        }
    }
}
