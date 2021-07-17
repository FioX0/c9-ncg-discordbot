using C9_NCG_DiscordBot.Handlers.Step;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using DSharpPlus.Interactivity;
using System.Text;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.Handlers.Steps
{
    public class IntStep : DialogueStepBase
    {
        private readonly int? _minValue;
        private readonly int? _maxValue;

        private IDialogueStep _nextStep;

        public IntStep(
            string content,
            IDialogueStep nextStep,
            int? minValue = null,
            int? maxValue = null) : base(content)
        {
            _nextStep = nextStep;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public Action<int> OnValidResult { get; set; } = delegate { };

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
                Description = $"{user.Mention}, {_content}",
                Color = DiscordColor.Blue,
                ThumbnailUrl = client.CurrentUser.AvatarUrl
            };
            embedBuilder.AddField("To Stop the Dialogue", "Use the +cancel command");

            //if(_minValue.HasValue)
            //{
            //    embedBuilder.AddField("Min Value:", $"{_minValue.Value} characters");
            //}

            //if (_maxValue.HasValue)
            //{
            //    embedBuilder.AddField("Max Value:", $"{_maxValue.Value} characters");
            //}

            var interactivity = client.GetInteractivityModule();

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

                if(!int.TryParse(messageResult.Message.Content, out int inputValue))
                {
                    if (messageResult.Message.Content.Contains("+setprofile") || messageResult.Message.Content.Contains("+ncg") || messageResult.Message.Content.Contains("+ncgprofile") || messageResult.Message.Content.Contains("+tip*") || messageResult.Message.Content.Contains("+tipbalance*") || messageResult.Message.Content.Contains("+tipredeem*"))
                        continue;
                    else
                    {
                        await TryAgain(channel, $"Your input is not a valid input").ConfigureAwait(false);
                        continue;
                    }
                }

                if(_minValue.HasValue)
                {
                    if(inputValue < _minValue.Value)
                    {
                        await TryAgain(channel, $"Your Input value {inputValue} is smaller than {_minValue.Value}").ConfigureAwait(false);
                        continue;
                    }
                }


                if (_maxValue.HasValue)
                {
                    if (inputValue > _maxValue.Value)
                    {
                        await TryAgain(channel, $"Your Input value {inputValue} is larger than {_maxValue.Value}").ConfigureAwait(false);
                        continue;
                    }
                }

                OnValidResult(inputValue);

                return false;
            }
        }

    }
}
