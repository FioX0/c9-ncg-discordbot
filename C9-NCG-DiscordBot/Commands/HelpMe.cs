using DSharpPlus.CommandsNext;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using C9_NCG_DiscordBot.Handlers;
using C9_NCG_DiscordBot.Handlers.Steps;
using System;

namespace C9_NCG_DiscordBot.Commands
{
    public class HelpMe : BaseCommandModule
    {

        [Command("helpme")]
        [Description("HelpMe will provide help with anything you might need.")]
        public async Task Dialogue(CommandContext ctx)
        {
            //var inputStep = new TextStep
            var inputStep = new IntStep("I will try to help you as much as I can.\nPlease advise, **by telling me which NUMBER**, the command you need help with.\n\n**NCG System**\n\n:one: +ping\n:two: +setprofile\n:three: +ncg\n:four: +ncgprofile\n\n**Tip System**\n\n:five: +tip\n:six: +tipbalance\n:seven: +tipredeem",null, 1, 7);
            var ping = new TextStep("**Ping Command**\n\nThe Ping command allows you to verify that the bot is online and waiting for commands.\n\n**How to use**\n\n +ping\n", null, 4, 7);
            var setprofile = new TextStep("**SetProfile Command**\n\nThe setprofile command will create a profile against your discord id.\nYou can create how many you want!\nThis will allow you to use the +ncgprofile command.\n\n**How to use**\n\nTo use this command you need 2 things:\n-A **name** for your profile,these are case SENSITIVE.\n-Your **PUBLIC Address/Public Key** visible in your settings menu.\n\n**Example**:\n+setprofile Guide 0xa49d64c31A2594e8Fb452238C9a03beFD1119963\n\nYou can then call the profile with +ncgprofile command.", null, 4, 7);
            var ncg = new TextStep("**NCG Command**\n\nThe ncg command allows you check the ncg of an address.\nIf you will be checking this adddress frequently look into +setprofile\n\n**How to use**\nTo use this command you need 1 thing.\n\n-Your PUBLIC Address/Public Key visible in your settings menu.\n\nExample:\n\n+ncg 0xa49d64c31A2594e8Fb452238C9a03beFD1119963.", null, 4, 7);
            var ncgprofile = new TextStep("**NCGProfile Command**\n\nThis command allows you to check NCG against created profiles against your discord.\nIf you haven't set a profile yet have a look at the +setprofile command.\n\n**How to use**\n\nThere are two ways to use this command.\n-You can ask for me to return **ALL profiles** by not specifying a profile **name** or tell me a specific profile to check.\n\n**Example**\n+ncgprofile Guide\nOR to return ALL profile\n+ncgprofile\n\n", null, 4, 7);
            var tip = new TextStep("**Tip Command**\n\nThis command allows you to tip another user.\nThis requires you to have enough tips in your balance to action.\n\n**How to use**\n\nTo use this command you need 2 things:\n-Whom you want to tip (@User).\n-The amount to tip.\n\n**Example**\n+tip @FioX#7044 10\n\n", null, 4, 7);
            var tipbalance = new TextStep("**Tipbalance Command**\n\nThis command allows you to check your Tipbalance.\n\n**How to use**\n\nSimply use +tipbalance. No other information is required.\n\n**Example**\n+tipbalance\n\n", null, 4, 7);
            var tipredeem = new TextStep("**Tipredeem Command**\n\nThis command allows you to redeem your Tipbalance.\n\n**How to use**\n\nTo use this command you need 2 things:\n-Your Public Address (Found in-game under settings).\n-The amount of tips to redeem.\n\n**Example**\n+tipredeem 0xa49d64c31A2594e8Fb452238C9a03beFD1119963 10\n\n", null, 4, 7);

            int input = 0;
            string stringinput = string.Empty;

            inputStep.OnValidResult += (result) =>
            {
                input = result;

                switch(input)
                {
                    case 1:
                        inputStep.SetNextStep(ping);
                        break;
                    case 2:
                        inputStep.SetNextStep(setprofile);
                        break;
                    case 3:
                        inputStep.SetNextStep(ncg);
                        break;
                    case 4:
                        inputStep.SetNextStep(ncgprofile);
                        break;
                    case 5:
                        inputStep.SetNextStep(tip);
                        break;
                    case 6:
                        inputStep.SetNextStep(tipbalance);
                        break;
                    case 7:
                        inputStep.SetNextStep(tipredeem);
                        break;
                }
            };

            ping.OnValidResult += (result) =>
            {
                stringinput = result;

                if (stringinput == "+back")
                    ping.SetNextStep(inputStep);             
            };

            setprofile.OnValidResult += (result) =>
            {
                stringinput = result;

                if (stringinput == "+back")
                    setprofile.SetNextStep(inputStep);
            };

            ncg.OnValidResult += (result) =>
            {
                stringinput = result;

                if (stringinput == "+back")
                    ncg.SetNextStep(inputStep);
            };

            ncgprofile.OnValidResult += (result) =>
            {
                stringinput = result;

                if (stringinput == "+back")
                    ncgprofile.SetNextStep(inputStep);
            };

            tip.OnValidResult += (result) =>
            {
                stringinput = result;

                if (stringinput == "+back")
                    tip.SetNextStep(inputStep);
            };

            tipbalance.OnValidResult += (result) =>
            {
                stringinput = result;

                if (stringinput == "+back")
                    tipbalance.SetNextStep(inputStep);
            };

            tipredeem.OnValidResult += (result) =>
            {
                stringinput = result;

                if (stringinput == "+back")
                    tipredeem.SetNextStep(inputStep);
            };

            var inputDialogueHandler = new DialogueHandler(ctx.Client, ctx.Channel, ctx.User, inputStep);

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }
        }

    }
}
