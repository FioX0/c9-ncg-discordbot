using System;
using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using C9_NCG_DiscordBot.blockchain;
using C9_NCG_DiscordBot.Models;
using C9_NCG_DiscordBot.Handlers;
using Newtonsoft.Json.Linq;

namespace C9_NCG_DiscordBot.Commands
{
    public class QueryCommands : BaseCommandModule
    {

        [Command("ping")]
        [Description("Way to check if the bot is alive")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("I'm here").ConfigureAwait(false);
        }

        [Command("setprofile")]
        [Description("Set your public key against your discord profile, will allow you to query ncg without having to use the public key all the time.")]
        public async Task SetProfileKey(CommandContext ctx, [Description("This is what you want to call your profile")] string alias= "null", [Description("This is your PUBLIC Address")] string publickey = "null")
        {
            Console.WriteLine("SetProfileReceived");
            ulong userid = ctx.Member.Id;
            var username = ctx.Member.DisplayName;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            var comms = new Communication();
            var setup = new Setup();
            var oldmessage = ctx.Message;

            if (publickey == "null" || !publickey.Contains("0x") || alias == "null")
            {
                //provide valid key.
                await ctx.Message.DeleteAsync("Invalid Alias provided, might be private key, deleting for security");
                await comms.CustomMessage(ctx, oldmessage,"Invalid SetProfile Request", "The data provided isn't quite right.\n\nI have deleted your message in case you provided a private key by accident.",DiscordColor.Red);
            }
            else
            {
                publickey.Replace(",", "");
                publickey.Replace(".", "");
                publickey.Replace("\"", "");
                var ncg = new NCG();
                string real = ncg.NCGGold(publickey);
                if (real != null)
                {
                    bool state = setup.SetProfileNew(publickey, alias, userid);
                    await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
                    if (state)
                    {
                        await comms.SetProfile(ctx, oldmessage, username, alias, publickey);
                    }
                    else
                    {
                        if (ncg.NCGGold("0xa49d64c31A2594e8Fb452238C9a03beFD1119963") == null)
                        {
                            await comms.SnapshotDown(ctx, oldmessage);
                        }
                        else
                        {
                            await comms.ProfileFailed(ctx, oldmessage, username);
                        }
                    }
                }
                else
                    await comms.CustomMessage(ctx, oldmessage, "Invalid SetProfile Request", "The PublicKey provided doesn't exist or is incorrectly provided.\nEnsure that you haven't provided symbols like commas,dots and so on.", DiscordColor.Red);
            }
        }

        [Command("ncg")]
        [Description("Allows you to request your NCG Balance using your PUBLIC Address")]
        public async Task Gold(CommandContext ctx, [Description("This is your PUBLIC Address")] string publickey = "")
        {
            var comms = new Communication();
            var username = ctx.Member.Username;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            var ncg = new NCG();
            var oldmessage = ctx.Message;

            if (publickey != "")
            {
                //debug
                Console.WriteLine("User: " + username + " requested NCG value against key: " + publickey);
                //debug end

                string result = ncg.NCGGold(publickey);
                Console.WriteLine(result);

                if (result == null)
                {
                    if (ncg.NCGGold("0xa49d64c31A2594e8Fb452238C9a03beFD1119963") == null)
                        await comms.SnapshotDown(ctx, oldmessage);
                }
                else
                    await comms.NormalNCG(ctx, oldmessage, username, result);
            }else
            await comms.CustomMessage(ctx, oldmessage, "Invalid Request", "**" + username + "**\nThe respective request is invalid.\nCorrect usage would be +ncg \"publickey here\"", DiscordColor.Red);
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
            var comms = new Communication();
            if (alias == "givemeall")
            {
                ProfileModel[] array = await sqli.LoadProfileALL(ctx.User.Id, alias);
                int count = array.Length;
                if (count == 0)
                    await comms.ProfileFailed(ctx, oldmessage, username);
                int y=1;
                List<DiscordMessage> messagesall = new List<DiscordMessage>();

                foreach (var entry in array)
                {
                    var result = await ncg.NCGProfileNewAsync(entry.PublicKey);

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
                            await comms.ProfileFailed(ctx, oldmessage, username);
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
                        await comms.ProfileFailed(ctx, oldmessage, username);
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

        [Command("wncg")]
        [Description("Allows you to request your NCG Balance using your PUBLIC Address")]
        public async Task WNCGCheck(CommandContext ctx, [Description("This is your recipient address")] string publickey = "")
        {
            var comms = new Communication();
            var username = ctx.Member.Username;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            var oldmessage = ctx.Message;

            if (publickey != "")
            {
                //debug
                Console.WriteLine("User: " + username + " requested WNCG query against key: " + publickey);
                //debug end

                var txlist = WNCG.GetETHHash();

                int size = txlist.Count;
                int y = 0;
                for(int i=1;i<size;i++)
                {
                    Console.WriteLine(txlist[i].hash);
                    var address = await WNCG.CheckETHHash(txlist[i].hash);
                    if(address.ToLower() == publickey.ToLower())
                    {
                        await comms.CustomMessage(ctx, oldmessage, "Mint Found", "**" + username + "**\nWNCG Mint Transaction ID: \n" + "https://etherscan.io/tx/"+txlist[i].hash, DiscordColor.Green);
                        y = 1;
                        break;
                    }
                }
                if(y==0)
                    await comms.CustomMessage(ctx, oldmessage, "Mint Not Found", "**" + username + "**\nI was unable to find the WNCG Mint request, if you just tried to mint then you might need to wait a bit of time.\n\n If you minted days ago, it might too far back for me to check.", DiscordColor.Red);
            }
            else
                await comms.CustomMessage(ctx, oldmessage, "Invalid Request", "**" + username + "**\nThe respective request is invalid.\nCorrect usage would be +wncg \"recipient address here\"", DiscordColor.Red);
        }
    }
}
