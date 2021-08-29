using C9_NCG_DiscordBot.Handlers.Step;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using DSharpPlus.Interactivity;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;

namespace C9_NCG_DiscordBot.Handlers.Steps
{
    public class TextStep : DialogueStepBase
    {
        private readonly int? _minLength;
        private readonly int? _maxLength;

        private IDialogueStep _nextStep;

        public TextStep(
            string content,
            IDialogueStep nextStep,
            int? minLength = null,
            int? maxLength = null) : base(content)
        {
            _nextStep = nextStep;
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public Action<string> OnValidResult { get; set; } = delegate { };

        public override IDialogueStep NextStep => _nextStep;

        public void SetNextStep(IDialogueStep nextStep)
        {
            _nextStep = nextStep;
        }

        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = $"Help System",
                Description = $"{_content}",
                Color = DiscordColor.Blue,
            };
            embedBuilder.AddField("To return to previous menu", "Use the +back command.");
            embedBuilder.AddField("To Stop the Dialogue", "Use the +cancel command.");

            //if(_minLength.HasValue)
            //{
            //    embedBuilder.AddField("Min Lenght:", $"{_minLength.Value} characters");
            //}

            //if (_maxLength.HasValue)
            //{
            //    embedBuilder.AddField("Min Lenght:", $"{_maxLength.Value} characters");
            //}

            var interactivity = client.GetInteractivity();

            while(true)
            {
                var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);

                OnMessageAdded(embed);

                var messageResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == channel.Id && x.Author.Id == user.Id).ConfigureAwait(false);

                OnMessageAdded(messageResult.Message);

                if(messageResult.Message.Content.Equals("+cancel", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if(_minLength.HasValue)
                {
                    if(messageResult.Message.Content.Length < _minLength.Value)
                    {                     
                            await TryAgain(channel, $"Your Input is not valid\n\n use **+back** to go to the main menu or **+cancel** to exit the help menu. L").ConfigureAwait(false);
                            continue;
                    }
                }


                if (_maxLength.HasValue)
                {
                    if (messageResult.Message.Content.Length > _maxLength.Value)
                    {
                        if (messageResult.Message.Content.Contains("+setprofile") || messageResult.Message.Content.Contains("+ncg") || messageResult.Message.Content.Contains("+ncgprofile") || messageResult.Message.Content.Contains("+tip") || messageResult.Message.Content.Contains("+tipbalance") || messageResult.Message.Content.Contains("+tipredeem"))
                            continue;
                        else
                        {
                            await TryAgain(channel, $"Your Input is not valid\n\n use **+back** to go to the main menu or **+cancel** to exit the help menu. H").ConfigureAwait(false);
                            continue;
                        }
                    }
                }

                OnValidResult(messageResult.Message.Content);

                return false;
            }
        }

    }
}
