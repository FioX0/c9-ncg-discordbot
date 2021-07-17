using C9_NCG_DiscordBot.blockchain;
using C9_NCG_DiscordBot.Handlers;
using C9_NCG_DiscordBot.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.Commands
{
    class RepCommands
    {
        //[Command("trade")]
        //[Description("Rep a person you have just traded with.")]
        //public async Task RepTransaction(CommandContext ctx, [Description("This is your TxId")] string txid = "", DiscordUser dis = null)
        //{
        //    var username = ctx.Member.Username;
        //    var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
        //    var oldmessage = ctx.Message;
        //    string mentionedUserId;
        //    await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
        //    var transactions = new Transactions();
        //    var comms = new Communication();
        //    if (dis == null || txid == "")
        //    {
        //        //stop, we need a valid mention and a valid txid
        //        Console.WriteLine("stop");
        //    }
        //    else
        //    {
        //        mentionedUserId = dis.Mention;
        //        mentionedUserId = mentionedUserId.Substring(2, mentionedUserId.Length - 3);
        //        //Let's verify the transaction provided
        //        string[] transactionAddreses = transactions.TransactionDetails(txid);
        //        //Okay we confirmed the transaction is valid and we got the sender's and receiver's addresss.
        //        //Let's ask for receiver for confirmation of the trade.
        //        bool state = await comms.RepCheck(ctx, username, mentionedUserId);
        //        if(state)
        //        {

        //        }
        //    }
        //}

        //[Command("rep")]
        //[Description("Rep a person.")]
        //public async Task<object> RepUser(CommandContext ctx, DiscordUser dis = null, int number = 0)
        //{
        //    var username = ctx.Member.Username;
        //    var eyes = DiscordEmoji.FromName(ctx.Client, ":eyes:");
        //    var oldmessage = ctx.Message;
        //    string mentionedUserId;
        //    int represult;
        //    await ctx.Message.CreateReactionAsync(eyes).ConfigureAwait(false);
        //    var transactions = new Transactions();
        //    var comms = new Communication();
        //    if (dis == null || number == 0)
        //    {
        //        //stop, we need a valid mention and a valid entry
        //        Console.WriteLine("stop");
        //    }
        //    else
        //    {
        //        mentionedUserId = dis.Mention;
        //        mentionedUserId = mentionedUserId.Substring(2, mentionedUserId.Length - 3);
        //        //Let's verify the user already exists in the ecosystem;
        //        RepModel userProfile = await SqliteDataAccess.LoadRepProfile(mentionedUserId);
        //        if(userProfile == null)
        //        {
        //            bool done = await SqliteDataAccess.CreateRepProfile(mentionedUserId);
        //            if (!done)
        //            {
        //                Console.WriteLine("Failed Rep Profile");
        //                return null;
        //            }
        //        }
        //        //okay we got user, now let's rep him.
        //        if (number > 0)
        //            represult = 1;
        //        else
        //            represult = -1;

        //    }
        //}
    }
}
