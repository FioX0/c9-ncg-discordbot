using System;
using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using C9_NCG_DiscordBot.blockchain;
using System.IO;
using C9_NCG_DiscordBot.TipSystem;
using C9_NCG_DiscordBot.Models;
using DSharpPlus.Interactivity;
using C9_NCG_DiscordBot.Handlers;

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

        [Command("setprofile")]
        [Description("Set your public key against your discord profile, will allow you to query ncg without having to use the public key all the time.")]
        public async Task SetProfileKey(CommandContext ctx, [Description("This is what you want to call your profile")] string alias, [Description("This is your PUBLIC Address")] string publickey)
        {
            Console.WriteLine("SetProfileReceived");
            ulong userid = ctx.Member.Id;
            var username = ctx.Member.DisplayName;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            var comms = new Communication();
            var setup = new Setup();
            var oldmessage = ctx.Message;

            bool state = setup.SetProfileNew(publickey,alias, userid);
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            if (state)
            {
                await comms.SetProfile(ctx, oldmessage, username, alias, publickey);
            }
            else
            {
                var ncg = new NCG();
                if (ncg.NCGGold("0xa49d64c31A2594e8Fb452238C9a03beFD1119963") == null)
                {
                    await comms.SnapshotDown(ctx, oldmessage);
                }
                else
                {
                    await comms.GenericError(ctx, oldmessage, username);
                }
            }
        }

        [Command("ncg")]
        [Description("Allows you to request your NCG Balance using your PUBLIC Address")]
        public async Task Gold(CommandContext ctx, [Description("This is your PUBLIC Address")] string publickey = "")
        {
            //Debug
            var username = ctx.Member.Username;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            var oldmessage = ctx.Message;
            Console.WriteLine("User: " + username + " requested NCG value against key: " + publickey);
            //debug end
            var ncg = new NCG();
            string result = ncg.NCGGold(publickey);
            Console.WriteLine(result);
            var comms = new Communication();
            if (result == null)
            {
                if (ncg.NCGGold("0xa49d64c31A2594e8Fb452238C9a03beFD1119963") == null)
                    await comms.SnapshotDown(ctx, oldmessage);
                else
                    await comms.GenericError(ctx, oldmessage, username);
            }
            else
                await comms.NormalNCG(ctx, oldmessage, username, result);
        }

        [Command("all")]
        [Description("Allows you to request your NCG Balance using your previous saved publickey")]
        public async Task AllProfiles(CommandContext ctx, [Description("This is what you called your profile")] string alias = "givemeall")
        {
            SqliteDataAccess sqli = new SqliteDataAccess();
            ProfileModel[] array = await sqli.LoadProfileALL(ctx.User.Id, alias);

        }

        [Command("ncgprofile")]
        [Description("Allows you to request your NCG Balance using your previous saved publickey")]
        public async Task GoldProfile(CommandContext ctx, [Description("This is what you called your profile")] string alias = "givemeall")
        {
            var username = ctx.Member.DisplayName;
            var userid = ctx.Member.Id;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            var oldmessage = ctx.Message;
            SqliteDataAccess sqli = new SqliteDataAccess();
            var ncg = new NCG();

            //Debug
            Console.WriteLine("User: " + username + " requested NCG value against profile: " + username);
            //debug end

            if (alias == "givemeall")
            {
                ProfileModel[] array = await sqli.LoadProfileALL(ctx.User.Id, alias);
                int count = array.Length;
                int y=1;
                List<DiscordMessage> messagesall = new List<DiscordMessage>();

                foreach (var entry in array)
                {
                    var result = await ncg.NCGProfileNewAsync(entry.PublicKey);
                    var comms = new Communication();
                    if (result == null)
                    {
                        //snapshot down?
                        if (ncg.NCGGold("0xa49d64c31A2594e8Fb452238C9a03beFD1119963") == null)
                        {
                            //fuck snapshot is down.
                            await comms.SnapshotDown(ctx, oldmessage);
                        }
                        else
                        {
                            await comms.GenericError(ctx, oldmessage, username);
                        }
                    }
                    else
                    {
                        bool state = await sqli.UpdateProfile(userid, entry.alias, result);
                        if (state)
                        {
                            float increase = float.Parse(result) - entry.Value;
                            messagesall = await comms.ProfileNCGALL(ctx, oldmessage, username, result, increase, entry.alias, count, y, messagesall);
                        }
                        else
                            await comms.GenericError(ctx, oldmessage, username);
                    }
                    y++;
                }
            }
            else
            {
                ProfileModel profile = await sqli.LoadProfile(userid, alias);
                var result = await ncg.NCGProfileNewAsync(profile.PublicKey);
                var comms = new Communication();
                if (result == null)
                {
                    //snapshot down?
                    if (ncg.NCGGold("0xa49d64c31A2594e8Fb452238C9a03beFD1119963") == null)
                    {
                        //fuck snapshot is down.
                        await comms.SnapshotDown(ctx, oldmessage);
                    }
                    else
                    {
                        await comms.GenericError(ctx, oldmessage, username);
                    }
                }
                else
                {
                    bool state = await sqli.UpdateProfile(userid, alias, result);
                    if (state)
                    {
                        float increase = float.Parse(result) - profile.Value;
                        await comms.ProfileNCG(ctx, oldmessage, username, result, increase,alias);
                    }
                    else
                        await comms.GenericError(ctx, oldmessage, username);
                }
            }
        }


        //fix this shit
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
                await ctx.Channel.SendMessageAsync(username + ", The requested public key had **" + result + "** NCG before the requested hash, according to my snapshot.").ConfigureAwait(false);
            }
        }
    }
}
