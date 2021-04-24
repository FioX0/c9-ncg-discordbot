using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.Handlers.Step
{
    public abstract class DialogueStepBase : IDialogueStep
    {
        protected readonly string _content;

        public DialogueStepBase(string content)
        {
            _content = content;
        }

        public Action<DiscordMessage> OnMessageAdded { get; set; } = delegate { };

        public abstract IDialogueStep NextStep { get; }

        public abstract Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user);

    }
}
