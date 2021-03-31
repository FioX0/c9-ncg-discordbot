using System;
using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using C9_NCG_DiscordBot.blockchain;

namespace C9_NCG_DiscordBot.Commands
{
    public class QueryCommands
    {
        [Command("ping")]
        [Description("Way to check if the bot is alive")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("I'm here").ConfigureAwait(false);
        }

        [Command("ncg")]
        [Description("Allows you to request your NCG Balance using your PUBLIC Address")]
        public async Task Gold(CommandContext ctx, [Description("This is your PUBLIC Address")] string publickey)
        {   
            //Debug
            var username = ctx.Member.Username;
            Console.WriteLine("User: " + username + " requested NCG value against key: " + publickey);
            //debug end
            var ncg = new NCG();
            string result = ncg.NCGGold(publickey);
            if (result == "false")
                await ctx.Channel.SendMessageAsync(username+", I'm sorry, something went wrong.\nPlease ensure that you provided the public key on your request. \nIf this error persists please contact the developer of this bot").ConfigureAwait(false);
            else
                await ctx.Channel.SendMessageAsync(username+", your current NCG is: **" + result + "**, according to my snapshot.").ConfigureAwait(false);
        }
    }
}
