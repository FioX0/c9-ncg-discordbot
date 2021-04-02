using System;
using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using C9_NCG_DiscordBot.blockchain;
using System.IO;

namespace C9_NCG_DiscordBot.Commands
{
    public class QueryCommands
    {

        int usage = 0;

        [Command("ping")]
        [Description("Way to check if the bot is alive")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("I'm here").ConfigureAwait(false);
        }

        [Command("activity")]
        [Description("You can check how many times people have used your bot commands since last restart.")]
        public async Task Activity(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Since last restart, I have been used: " +usage + " times.").ConfigureAwait(false);
        }

        [Command("setprofile")]
        [Description("Set your public key against your discord profile, will allow you to query ncg without having to use the public key all the time.")]
        public async Task SetProfileKey(CommandContext ctx, string publickey)
        {
            Console.WriteLine("SetProfileReceived");
            ulong userid = ctx.Member.Id;
            var setup = new Setup();
            bool state = setup.SetProfile(publickey, userid);

            if (state == false)
            {
                await ctx.Channel.SendMessageAsync("Something has gone wrong, please report this as a bug, for key: " + publickey).ConfigureAwait(false);
            }
            usage++;
            await ctx.Channel.SendMessageAsync("I have now updated your profile with the following public key " + publickey).ConfigureAwait(false);
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
                await ctx.Channel.SendMessageAsync(username + ", I'm sorry, something went wrong.\nPlease ensure that you provided the public key on your request. \nIf this error persists please contact the developer of this bot").ConfigureAwait(false);
            else
            {
                usage++;
                await ctx.Channel.SendMessageAsync(username + ", your current NCG is: **" + result + "**, according to my snapshot.").ConfigureAwait(false);
            }
        }

        [Command("ncgprofile")]
        [Description("Allows you to request your NCG Balance using your previous saved publickey")]
        public async Task GoldProfile(CommandContext ctx)
        {
            //Debug
            var username = ctx.Member.DisplayName;
            var userid = ctx.Member.Id;
            Console.WriteLine("User: " + username + " requested NCG value against profile: " + username);
            //debug end
            var ncg = new NCG();
            string result = ncg.NCGProfile(userid);
            if (result == "false")
                await ctx.Channel.SendMessageAsync(username + ", I'm sorry, something went wrong.\nPlease ensure that you provided the public key on your request. \nIf this error persists please contact the developer of this bot").ConfigureAwait(false);
            else
            {
                usage++;
                await ctx.Channel.SendMessageAsync(username + ", your current NCG is: **" + result + "**, according to my snapshot.").ConfigureAwait(false);
            }

        }

        [Command("ncgbeforehash")]
        [Description("Allows you to request your NCG Balance using your PUBLIC Address before a specific block occured")]
        public async Task GoldBeforeHash(CommandContext ctx, [Description("This is your PUBLIC Address")] string publickey, [Description("Hash of the block you want to check against")] string hash)
        {
            //Debug
            var username = ctx.Member.Username;
            Console.WriteLine("User: " + username + " requested NCG value against key: " + publickey + " with hash "+hash+".");
            //debug end
            var ncg = new NCG();
            string result = ncg.NCGGoldBeforeHash(publickey,hash);
            if (result == "false")
                await ctx.Channel.SendMessageAsync(username + ", I'm sorry, something went wrong.\nPlease ensure that you provided the public key on your request. \nIf this error persists please contact the developer of this bot").ConfigureAwait(false);
            else
            {
                usage++;
                await ctx.Channel.SendMessageAsync(username + ", your NCG at that time was **" + result + "**, according to my snapshot.").ConfigureAwait(false);
            }
        }
    }
}
