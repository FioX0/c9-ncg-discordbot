using C9_NCG_DiscordBot.Handlers.Step;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.Handlers.Steps
{
    public class TextStep : DialogueStepBase
    {
        private IDialogueStep _nextStep;
        private readonly int? _minLength;
        private readonly int? _maxLength;

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

        public override Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            throw new NotImplementedException();
        }

    }
}
