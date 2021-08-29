using C9_NCG_DiscordBot.blockchain;
using C9_NCG_DiscordBot.Handlers;
using C9_NCG_DiscordBot.Models;
using CsvHelper;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.Commands
{
    class ReportCommands : BaseCommandModule
    {
        [Command("mined-yd")]
        [Description("Allows you to request your NCG Balance using your previous saved publickey")]
        public async Task BlockReport(CommandContext ctx, [Description("This is what you called your profile")] string alias = "givemeall")
        {
            var username = ctx.Member.DisplayName;
            var userid = ctx.Member.Id;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            var oldmessage = ctx.Message;

            SqliteDataAccess sqli = new SqliteDataAccess();
            var blocks = new Blocks();
            var ncg = new NCG();
            var comms = new Communication();

            if (alias == "givemeall")
            {
                ProfileModel[] array = await sqli.LoadProfileALL(ctx.User.Id, alias);
                int count = array.Length;
                if (count == 0)
                    await comms.ProfileFailed(ctx, oldmessage, username);
                int y = 1;
                List<DiscordMessage> messagesall = new List<DiscordMessage>();
                await comms.Warning(ctx, oldmessage, username, 15);
                foreach (var entry in array)
                {
                    int result = blocks.BlockReportYesterday(entry.PublicKey);

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
                        if (result == 0 || result == 99999)
                            comms.NoBlockHistory(ctx, oldmessage, username, entry.alias);
                        else
                            messagesall = await comms.YesterdayReportALL(ctx, oldmessage, username, result, entry.alias, count, y, messagesall);
                    }
                    y++;
                }
            }
            else
            {
                ProfileModel profile = await sqli.LoadProfile(userid, alias);
                if (profile == null)
                {
                    await comms.ProfileFailed(ctx, oldmessage, username);
                }
                else
                {
                    await comms.Warning(ctx, oldmessage, username, 15);
                    int result = blocks.BlockReportYesterday(profile.PublicKey);
                    if (result == 0 || result == 99999)
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
                        await comms.YesterdayBlockReport(ctx, oldmessage, username, result, alias);
                }
            }
        }

        [Command("mined-wk")]
        [Description("Allows you to request your NCG Balance using your previous saved publickey")]
        public async Task BlockReportWeek(CommandContext ctx, [Description("This is what you called your profile")] string alias = "givemeall")
        {
            var username = ctx.Member.DisplayName;
            var userid = ctx.Member.Id;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            var oldmessage = ctx.Message;

            SqliteDataAccess sqli = new SqliteDataAccess();
            var blocks = new Blocks();
            var ncg = new NCG();
            var comms = new Communication();

            if (alias == "givemeall")
            {
                ProfileModel[] array = await sqli.LoadProfileALL(ctx.User.Id, alias);
                int count = array.Length;
                if (count == 0)
                    await comms.ProfileFailed(ctx, oldmessage, username);
                int y = 1;
                List<DiscordMessage> messagesall = new List<DiscordMessage>();
                await comms.Warning(ctx, oldmessage, username, 30);
                foreach (var entry in array)
                {
                    int result = blocks.BlockReportWeek(entry.PublicKey);

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
                        if (result == 0 || result == 99999)
                            comms.NoBlockHistory(ctx, oldmessage, username, entry.alias);
                        else
                            messagesall = await comms.WeekBlockReportALL(ctx, oldmessage, username, result, entry.alias, count, y, messagesall);
                    }
                    y++;
                }
            }
            else
            {
                ProfileModel profile = await sqli.LoadProfile(userid, alias);
                if (profile == null)
                {
                    await comms.ProfileFailed(ctx, oldmessage, username);
                }
                else
                {
                    await comms.Warning(ctx, oldmessage, username, 30);
                    int result = blocks.BlockReportWeek(profile.PublicKey);
                    if (result == 0 || result == 99999)
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
                        await comms.WeekBlockReport(ctx, oldmessage, username, result, alias);
                }
            }
        }

        [Command("mined-mn")]
        [Description("Allows you to request your NCG Balance using your previous saved publickey")]
        public async Task BlockReportMonth(CommandContext ctx, [Description("This is what you called your profile")] string alias = "givemeall")
        {
            var username = ctx.Member.DisplayName;
            var userid = ctx.Member.Id;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            var oldmessage = ctx.Message;

            SqliteDataAccess sqli = new SqliteDataAccess();
            var blocks = new Blocks();
            var ncg = new NCG();
            var comms = new Communication();

            if (alias == "givemeall")
            {
                ProfileModel[] array = await sqli.LoadProfileALL(ctx.User.Id, alias);
                int count = array.Length;
                if (count == 0)
                    await comms.ProfileFailed(ctx, oldmessage, username);
                int y = 1;
                List<DiscordMessage> messagesall = new List<DiscordMessage>();
                await comms.Warning(ctx, oldmessage, username, 60);
                foreach (var entry in array)
                {
                    int result = blocks.BlockReportMonth(entry.PublicKey);

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
                        if (result == 0 || result == 99999)
                            comms.NoBlockHistory(ctx, oldmessage, username, entry.alias);
                        else
                            messagesall = await comms.MonthBlockReportALL(ctx, oldmessage, username, result, entry.alias, count, y, messagesall);
                    }
                    y++;
                }
            }
            else
            {
                ProfileModel profile = await sqli.LoadProfile(userid, alias);
                if (profile == null)
                {
                    await comms.ProfileFailed(ctx, oldmessage, username);
                }
                else
                {
                    await comms.Warning(ctx, oldmessage, username, 60);
                    int result = blocks.BlockReportWeek(profile.PublicKey);
                    if (result == 0 || result == 99999)
                    {
                        //snapshot down?
                        if (ncg.NCGGold("0xa49d64c31A2594e8Fb452238C9a03beFD1119963") == null)
                        {
                            //fuck snapshot is down.
                            await comms.SnapshotDown(ctx, oldmessage);
                        }
                        else
                        {
                            comms.NoBlockHistory(ctx, oldmessage, username, alias);
                        }
                    }
                    else
                        await comms.MonthBlockReport(ctx, oldmessage, username, result, alias);
                }
            }
        }


        [Command("exploitreport")]
        //[Cooldown(1, 60, CooldownBucketType.Global)]
        public async Task exploitreport(CommandContext ctx, string address = null)
        {
            var username = ctx.Member.DisplayName;
            var userid = ctx.Member.Id;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
            var oldmessage = ctx.Message;

            if(address == null)
            {

            }
            else
            {
               var result = await Blocks.ExploitReport(address, ctx);

            }
            
        }
    }
}
