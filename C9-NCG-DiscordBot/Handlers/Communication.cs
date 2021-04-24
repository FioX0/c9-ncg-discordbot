using C9_NCG_DiscordBot.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.Handlers
{
    public class Communication
    {
        #region NCG
        public async Task SnapshotDown(CommandContext ctxl, DiscordMessage oldmessage)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            //fuck snapshot is down.
            var snapdown = new DiscordEmbedBuilder
            {
                Title = "NCGProfile Failure",
                Description = "***I believe my snapshot is down, please wait while I call my minion to come and fix this*** <@145915312343220224>.",
                Color = DiscordColor.Red,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: snapdown).ConfigureAwait(false);
            //since embeds don't ping, let's send the below message to ping me, we can delete this right away as the ping will remain.
            var ping = await ctxl.Channel.SendMessageAsync("<@145915312343220224>").ConfigureAwait(false);
            await ping.DeleteAsync("PingSent");
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }
        public async Task GenericError(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Request has failed",
                Description = "**" + username + "** I'm sorry something went wrong.\n\nPlease ensure that you provided all the necessary information to use this command.\n\n Use +help if you need assistance.",
                Color = DiscordColor.Red,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }
        public async Task NormalNCG (CommandContext ctxl, DiscordMessage oldmessage, string username, string result)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");
            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "NCGRequest",
                Description = "**" + username + "** The requested public key holds **" + result + "** NCG.",
                Color = DiscordColor.Green,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);

            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");

            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);

            var interactivity = ctxl.Client.GetInteractivityModule();

            var reactionresult = await interactivity.WaitForMessageReactionAsync(x => x.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult is null)
                {
                    Console.WriteLine("Time");
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Emoji == delete)
                {
                    await sendmessage.DeleteAsync().ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
            }
            catch (Exception) { }
        }
        public async Task ProfileNCG (CommandContext ctxl, DiscordMessage oldmessage, string username, string result, float increase)
        {
            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "NCGProfile",
                Description = "The NCG against the requested 9C-Profile is: **" + result + "**.\n\nThis is an increase of: **" + increase.ToString("0.00") + "** since last request.",
                Color = DiscordColor.Green,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
            var interactivity = ctxl.Client.GetInteractivityModule();
            var reactionresult = await interactivity.WaitForMessageReactionAsync(x => x.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult is null)
                {
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Emoji == delete)
                {
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                    await sendmessage.DeleteAsync().ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task SetProfile(CommandContext ctxl, DiscordMessage oldmessage, string username, string alias, string publickey)
        {

            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "SetProfile",
                Description = "**" + username + "**" + " A NCGProfile has been set under your Discord ID\n\n With Alias '**" + alias + "'**\n\n For Key **'" + publickey + "'**.",
                Color = DiscordColor.Green,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
            var interactivity = ctxl.Client.GetInteractivityModule();
            var reactionresult = await interactivity.WaitForMessageReactionAsync(x => x.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult is null)
                {
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Emoji == delete)
                {
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                    await sendmessage.DeleteAsync().ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region TipSystem
        public async Task UserBalanceCom(CommandContext ctxl, DiscordMessage oldmessage, string username, TipModel profile)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipBalance",
                Description = "**" + username + "**: - The queried profile's tip balance is **"+ profile.Balance+"** Tips.",
                Color = DiscordColor.Green,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task SuccessFulTip(CommandContext ctxl, DiscordMessage oldmessage, string username,TipModel profile ,string mentionuser, int ammount, float softcap)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");
            float balance;
            if (softcap > 0)
                balance = softcap;
            else
                balance = profile.Balance;

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipRequest",
                Description = "**" + username + "**: - I have **Succesfully** processed your request: \n\n Tip to **"+mentionuser+"** amount of **"+ammount+"** Tips.\n\n You still have **" + balance + "** Tips.",
                Color = DiscordColor.Green,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task AdminBalance(CommandContext ctxl, DiscordMessage oldmessage, string username, float value)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Admin Balance",
                Description = "**" + username + "**: - I have **Succesfully** processed your request: \n\n There's still "+value+" left in the Admin Pot",
                Color = DiscordColor.Green,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task ErrorBalanceCom(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipBalance",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\n You are only able to query your own TipBalance",
                Color = DiscordColor.Red,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task HelpError(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipBalance",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\n This is likely to missing a required value.\n\nA correct example would be:\n +tip @FioX 10",
                Color = DiscordColor.Red,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task NotEnoughBalance(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipRequest",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\n You do not have enough TipBalance to proceed with the tip.",
                Color = DiscordColor.Red,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task TipYourself(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipRequest",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\n I'm afraid you can't tip yourself.",
                Color = DiscordColor.Red,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task FailedTep(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipRequest",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\n Something has gone very wrong.\n\nI will ping my minion to have a look. I recommend to not repeat your tip as it could have gone through.",
                Color = DiscordColor.Red,
                ThumbnailUrl = ctxl.Client.CurrentUser.AvatarUrl
            };
            var ping = await ctxl.Channel.SendMessageAsync("<@145915312343220224>").ConfigureAwait(false);
            await ping.DeleteAsync("PingSent");
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }
        #endregion
    }
}
