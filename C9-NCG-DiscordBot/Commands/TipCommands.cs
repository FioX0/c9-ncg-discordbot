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
using static C9_NCG_DiscordBot.Enums.TipEnums;

namespace C9_NCG_DiscordBot.Commands
{
    public class TipCommands
    {
        [Command("tipadmin")]
        [Description("Set user as admin on tipping system")]
        public async Task TipCreateAdmin(CommandContext ctx, DiscordUser dis)
        {
            //Locks command to Planetarium Server and Server Owner.
            if (ctx.Guild.Id == 539405872346955788 && ctx.Member.IsOwner)
            {
                string mentionedUserId = dis.Mention;
                mentionedUserId = mentionedUserId.Substring(2, mentionedUserId.Length - 3);
                ulong uLMentionedUserId = ulong.Parse(mentionedUserId);
                Console.WriteLine(uLMentionedUserId);
                //Call AdminProfileFunction to Check/Create/Assign Admin User.
                await Tips.AdminProfileCheck(uLMentionedUserId, "Admin");
                await ctx.Channel.SendMessageAsync("UserID: " + uLMentionedUserId + " is now an admin.").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("I'm sorry, this command can only be executed in the official Planetarium Server").ConfigureAwait(false);
            }
        }

        [Command("tipadminbalance")]
        [Description("Set user as admin on tipping system")]
        public async Task AdminBalance(CommandContext ctx)
        {
            //Locks command to Planetarium Server and Server Owner.
            if (ctx.Guild.Id == 539405872346955788)
            {
                var username = ctx.Member.Username;            
                var oldmessage = ctx.Message;
                TipModel profile = new TipModel();
                var comms = new Communication();

                Console.WriteLine("Balance Called");
                //get user's profile and check if they are an admin, if not don't acknowledge the command exists.
                profile = await Tips.ProfileExistsNew(ctx.Member.Id);
                if(profile.Role == "Admin")
                {
                    var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
                    await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
                    var value = Tips.AdminBalanceCheck();
                    await comms.AdminBalance(ctx, oldmessage, username, value.Result);
                }
            }
        }

        [Command("tipmod")]
        [Description("Set user as mod on tipping system")]
        public async Task TipCreateMod(CommandContext ctx, DiscordUser dis)
        {
            var username = ctx.Member.Username;
            var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
            var oldmessage = ctx.Message;
            TipModel profile = new TipModel();
            var comms = new Communication();
            await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);

            //get user's profile and check if they are an admin, if not don't acknowledge the command exists.
            if (ctx.Guild.Id == 539405872346955788 && profile.Role == "Admin")
            {
                string mentionedUserId = dis.Mention;
                mentionedUserId = mentionedUserId.Substring(2, mentionedUserId.Length - 3);
                ulong uLMentionedUserId = ulong.Parse(mentionedUserId);
                Console.WriteLine(uLMentionedUserId);
                //Call AdminProfileFunction to Check/Create/Assign Mod User.
                await Tips.AdminProfileCheck(uLMentionedUserId, "Mod");
                await ctx.Channel.SendMessageAsync("UserID: " + uLMentionedUserId + " is now a Mod.").ConfigureAwait(false);
            }
            else
            {
            }
        }

        [Command("tip")]
        [Description("tip a user")]
        public async Task Tip(CommandContext ctx, DiscordUser dis = null, int amount = 0)
        {

            if (ctx.Guild.Id == 539405872346955788)
            {
                var username = ctx.Member.Username;
                var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
                await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
                var oldmessage = ctx.Message;
                //
                TipModel caller = new TipModel();
                var comms = new Communication();

                if (amount <= 0 || dis == null)
                {
                    await comms.HelpError(ctx, oldmessage, ctx.Member.Username);
                }
                else
                {
                    string mentionedUserId = dis.Mention;
                    string filteredMentionedUserID = mentionedUserId.Substring(2, mentionedUserId.Length - 3);
                    ulong uLMentionedUserId = ulong.Parse(filteredMentionedUserID);
                    var result = await Tips.TipsSystemAsync(ctx.User.Id, uLMentionedUserId, amount);
                    Console.WriteLine(result);

                    // Get user's profile so we can pass their details to the communication function;
                    caller = await Tips.ProfileExistsNew(ctx.Member.Id);
                    //Check how much NCG is left in the bot in case it was called by an admin.
                    var softcap = Tips.AdminBalanceCheck();
                    //NEB - Not Enough Balance
                    //PASS - Can't Tip yourself.
                    //Fail - Failed Tip
                    //Complete - All green
                    //AComplete - Admin Custom "All Green"

                    switch (result)
                    {
                        case Status.NEB:
                            await comms.NotEnoughBalance(ctx, oldmessage, ctx.Member.Username);
                            break;
                        case Status.PASS:
                            await comms.TipYourself(ctx, oldmessage, ctx.Member.Username);
                            break;
                        case Status.FAIL:
                            await comms.FailedTep(ctx, oldmessage, ctx.Member.Username);
                            break;
                        case Status.COMPLETE:
                            await comms.SuccessFulTip(ctx, oldmessage, ctx.Member.Username, caller, dis.Mention, amount, 0);
                            break;
                        case Status.ACOMPLETE:
                            await comms.SuccessFulTip(ctx, oldmessage, ctx.Member.Username, caller, dis.Mention, amount, softcap.Result);
                            break;
                        default:
                            await comms.FailedTep(ctx, oldmessage, ctx.Member.Username);
                            break;
                    }
                }
            }
        }

        [Command("tipbalance")]
        [Description("Check Tip Balance of User")]
        public async Task TipBalance(CommandContext ctx, DiscordUser dis = null)
        {
            if (ctx.Guild.Id == 539405872346955788)
            {
                var profile = new TipModel();
                var profile2 = new TipModel();
                var comms = new Communication();
                var username = ctx.Member.Username;
                var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
                var oldmessage = ctx.Message;
                string mentionedUserId;

                //If user decides to not mention a user, we assume that they are querying themselves.
                if (dis == null)
                    mentionedUserId = ctx.Member.Id.ToString();
                else
                {
                    mentionedUserId = dis.Mention;
                    mentionedUserId = mentionedUserId.Substring(2, mentionedUserId.Length - 3);
                }

                //Console.WriteLine(ctx.Member.Id.ToString());

                await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);

                // If User is querying themselves.
                if (ctx.Member.Id.ToString() == mentionedUserId)
                {
                    profile = await Tips.ProfileExistsNew(ctx.Member.Id);
                    ///// Discord Embed to Display Balance
                    await comms.UserBalanceCom(ctx, oldmessage, username, profile);
                }
                else
                {
                    //If User is a Admin/Mod allow querying someone else. Otherwise reject.
                    profile = await Tips.ProfileExistsNew(ctx.Member.Id);
                    if (profile.Role == "Admin" || profile.Role == "Mod")
                    {
                        ulong ulongMentionId = ulong.Parse(mentionedUserId);
                        profile2 = await Tips.ProfileExistsNew(ulongMentionId);
                        await comms.UserBalanceCom(ctx, oldmessage, username, profile2);
                    }
                    else
                    {
                        await comms.ErrorBalanceCom(ctx, oldmessage, username);
                    }
                }
            }
        }

        [Command("tipredeem")]
        [Description("Exchange your Tips for NCG")]
        [Cooldown(1,5,CooldownBucketType.User)]
        public async Task TipWithdraw(CommandContext ctx, string key = null, float amount = 0)
        {
            if (ctx.Guild.Id == 539405872346955788)
            {
                if (key == null || !key.Contains("0x") || amount <= 0)
                {
                    //provide valid key.
                    Console.WriteLine("Invalid Data");
                }
                var profile = new TipModel();
                var comms = new Communication();
                var username = ctx.Member.Username;
                var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
                var oldmessage = ctx.Message;
                await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
                profile = await Tips.ProfileExistsNew(ctx.Member.Id);
                if (profile.Balance >= amount)
                {
                    bool state = await Tips.QueueTips(profile, key, amount, ctx.Member.Id);
                    if (!state)
                    {
                        await comms.GenericError(ctx, oldmessage, username);
                    }
                    else
                    {
                        //get new balance
                        profile = await Tips.ProfileExistsNew(ctx.Member.Id);
                        await comms.RedeemQueue(ctx, oldmessage, username, profile.Balance);
                    }
                }
                else
                {
                    await comms.NotEnoughBalance(ctx, oldmessage, ctx.Member.Username);
                }
            }
        }
    }
}
