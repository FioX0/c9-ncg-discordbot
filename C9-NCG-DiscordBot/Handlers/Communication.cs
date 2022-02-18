using C9_NCG_DiscordBot.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace C9_NCG_DiscordBot.Handlers
{
    public class Communication
    {

        public List<DiscordMessage> messages = new List<DiscordMessage>();

        public async Task CustomMessage(CommandContext ctxl, DiscordMessage oldmessage, string title, string description, DiscordColor color)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = color,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }
        //SHIT ton of duplciate code here, we need to eventually clean this crap.
        #region NCG
        public async Task SnapshotDown(CommandContext ctxl, DiscordMessage oldmessage)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            //fuck snapshot is down.
            var snapdown = new DiscordEmbedBuilder
            {
                Title = "NCGProfile Failure",
                Description = "**The GQL Server is currently experiencing issues. Try again later.**",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
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

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Request has failed",
                Description = "**" + username + "**I'm sorry something went wrong.\n\nPlease ensure that you provided all the necessary information to use this command.",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task ProfileFailed(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "NCGProfile Failed",
                Description = "**" + username + "** I was not able to find a NCGProfile under you account with the respective profile name\n\nPlease create a NCGProfile first.\n\n +setprofile \"profilename\" \"publicaddress\"",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }
        public async Task NormalNCG(CommandContext ctxl, DiscordMessage oldmessage, string username, string result)
        {
            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");
            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "NCGRequest",
                Description = "**" + username + "** The requested public key holds **" + result + "** NCG.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);

            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");

            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);

            var interactivity = ctxl.Client.GetInteractivity();

            var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult.Result is null)
                {
                    Console.WriteLine("Time");
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Result.Emoji.Name == delete)
                {
                    await sendmessage.DeleteAsync().ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
            }
            catch (Exception) { }
        }
        public async Task ProfileNCG(CommandContext ctxl, DiscordMessage oldmessage, string username, string result, float increase, string alias)
        {
            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "NCGProfile",
                Description = "**" + username + "** the NCG against the requested 9C-Profile with alias **" + alias + "**  is: **" + result + "**.\n\nThis is an increase of: **" + increase.ToString("0.00") + "** since last request.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
            var interactivity = ctxl.Client.GetInteractivity();
            var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult.Result is null)
                {
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Result.Emoji.Name == delete)
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

        public async Task<List<DiscordMessage>> ProfileNCGALL(CommandContext ctxl, DiscordMessage oldmessage, string username, string result, float increase, string alias, int arraylength, int arrayentry, List<DiscordMessage> messagelist)
        {
            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            if (ctxl.Channel.Id == 856196469727035432 && increase == 0)
            {
                EmbedThumbnail thumbnail = new EmbedThumbnail();
                thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

                var embedmessage = new DiscordEmbedBuilder
                {
                    Title = "NCGProfile",
                    Description = "**" + username + "** the NCG against the requested 9C-Profile with alias **" + alias + "**  is: **" + result + "**.\n\nThis is an increase of: **you got jack shit mate** since last request.",
                    Color = DiscordColor.Green,
                    Thumbnail = thumbnail
                };

                var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
                messagelist.Add(sendmessage);

                if (arraylength == arrayentry)
                {
                    await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
                    var interactivity = ctxl.Client.GetInteractivity();
                    var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);
                    try
                    {
                        if (reactionresult.Result is null)
                        {
                            await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                        }
                        else if (reactionresult.Result.Emoji.Name == delete)
                        {
                            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                            await DeleteMessages(messagelist);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    Console.WriteLine("Not Done yet");
                    return messagelist;
                }
                return null;
            }
            else
            {
                EmbedThumbnail thumbnail = new EmbedThumbnail();
                thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

                var embedmessage = new DiscordEmbedBuilder
                {
                    Title = "NCGProfile",
                    Description = "**" + username + "** the NCG against the requested 9C-Profile with alias **" + alias + "**  is: **" + result + "**.\n\nThis is an increase of: **" + increase.ToString("0.00") + "** since last request.",
                    Color = DiscordColor.Green,
                    Thumbnail = thumbnail
                };
                var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
                messagelist.Add(sendmessage);

                if (arraylength == arrayentry)
                {
                    await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
                    var interactivity = ctxl.Client.GetInteractivity();
                    var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);
                    try
                    {
                        if (reactionresult.Result is null)
                        {
                            await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                        }
                        else if (reactionresult.Result.Emoji.Name == delete)
                        {
                            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                            await DeleteMessages(messagelist);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    Console.WriteLine("Not Done yet");
                    return messagelist;
                }
            }
            return null;
        }

        private async Task DeleteMessages(List<DiscordMessage> list)
        {
            foreach (var message in list)
            {
                await message.DeleteAsync().ConfigureAwait(false);
            }
        }

        public async Task SetProfile(CommandContext ctxl, DiscordMessage oldmessage, string username, string alias, string publickey)
        {

            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";


            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "SetProfile",
                Description = "**" + username + "**" + " A NCGProfile has been set under your Discord ID\n\n With Alias '**" + alias + "'**\n\n For Key **'" + publickey + "'**.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
            var interactivity = ctxl.Client.GetInteractivity();
            var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult.Result is null)
                {
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Result.Emoji.Name == delete)
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

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";
            
            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipBalance",
                Description = "**" + username + "**: - The queried profile's tip balance is **" + profile.Balance + "** Tips.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task SuccessFulTip(CommandContext ctxl, DiscordMessage oldmessage, string username, TipModel profile, string mentionuser, int ammount, float softcap)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");
            float balance;
            if (softcap > 0)
                balance = softcap;
            else
                balance = profile.Balance;

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipRequest",
                Description = "**" + username + "**: - I have **Succesfully** processed your request: \n\n Tip to **" + mentionuser + "** amount of **" + ammount + "** Tips.\n\nYou still have **" + balance + "** Tips.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task AdminBalance(CommandContext ctxl, DiscordMessage oldmessage, string username, float value)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Admin Balance",
                Description = "**" + username + "**: - I have **Succesfully** processed your request: \n\nThere's still **" + value + "** left in the Admin Pot",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task RedeemSuccess(DiscordClient client, PaymentModel profile, string txid)
        {

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Tip Withdrawal",
                Description = "**<@" + profile.DiscordUserId + ">**: - I have **Succesfully** processed your withdrawal:\n\nYour TransactionId(txid) is: **[" + txid + "](https://explorer.libplanet.io/9c-main/transaction/?" + txid + ")**\n\n",
                Color = DiscordColor.Green,
                ImageUrl = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png"
            };
            DiscordChannel channel = await client.GetChannelAsync(829007486479499315);
            await client.SendMessageAsync(channel, embed: embedmessage).ConfigureAwait(false);
            var ping = await client.SendMessageAsync(channel, "<@" + profile.DiscordUserId + ">").ConfigureAwait(false);
            await ping.DeleteAsync("PingSent");
        }

        public async Task RedeemQueue(CommandContext ctxl, DiscordMessage oldmessage, string username, float value)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Tip Withdrawal",
                Description = "**" + username + "**: - I have **Succesfully** queued your withdrawal:\n\nI will let you know when this has been processed.\n\nThere's still **" + value + "** left in your TipBalance",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task ErrorBalanceCom(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipBalance",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\nYou are only able to query your own TipBalance",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task HelpError(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipBalance",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\nThis is likely to missing a required value.\n\nA correct example would be:\n +tip @FioX 10",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task NotEnoughBalance(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipRequest",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\nYou do not have enough TipBalance to proceed with the tip.",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task TipYourself(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipRequest",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\nI'm afraid you can't tip yourself.",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task FailedTep(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "TipRequest",
                Description = "**" + username + "**: - I'm not able to proceed with your request.\n\nSomething has gone very wrong.\n\nI will ping my minion to have a look. I recommend to not repeat your tip as it could have gone through.",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
            };
            var ping = await ctxl.Channel.SendMessageAsync("<@145915312343220224>").ConfigureAwait(false);
            await ping.DeleteAsync("PingSent");
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }

        public async Task TopTupStarded(CommandContext ctxl, DiscordMessage oldmessage, string username)
        {

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Tip Top Up",
                Description = "**" + username + "**: - I have received a **Top-up request**.\n\nPlease send the top-up to **0x6AEbea29B88a4b6BB21B877bb6b0E6C6F8C247B8**.\n\nI will check for 1 NCG transaction to arrive from your indicated address for the next 20minutes.",
                Color = DiscordColor.Yellow,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
        }

        public async Task TopTupDone(CommandContext ctxl, DiscordMessage oldmessage, string username, int value)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Tip Top Up",
                Description = "**" + username + "**: - **Top-up Succesful**, your TipBalance has been increased by **" + value + "** tips.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };
            await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
            await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
        }
        #endregion


        #region RepSystem
        public async Task<bool> RepCheck(CommandContext ctxl, string username, string mentionedUser)
        {
            //Check Message

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Transaction Check",
                Description = "Hi <@" + mentionedUser + ">\n\nUser " + username + " has reported that a Succesful NCG transaction has occured.\n\nPress :white_check_mark: to confirm\n\n Press :no_entry_sign: to deny.",
                Color = DiscordColor.Yellow,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);

            var no = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");

            var yes = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            await sendmessage.CreateReactionAsync(yes).ConfigureAwait(false);
            await sendmessage.CreateReactionAsync(no).ConfigureAwait(false);

            var interactivity = ctxl.Client.GetInteractivity();

            var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == yes || x.Emoji.Name == no, sendmessage, ctxl.User).ConfigureAwait(false);

            Console.WriteLine(reactionresult.Result.Emoji.Name.ToString());

            if (reactionresult.Result.Emoji.Name == yes)
            {
                return true;
            }
            else if (reactionresult.Result.Emoji.Name == no)
            {
                return false;
            }
            return false;
        }
        #endregion

        #region BlockReports
        public async Task YesterdayBlockReport(CommandContext ctxl, DiscordMessage oldmessage, string username, int result, string alias)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Block Report",
                Description = "**" + username + "** You have mined **" + result + "** block yesterday on profile **"+alias+"**.\n\nThis is **"+result*10+"** NCG.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);

            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");

            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);

            var interactivity = ctxl.Client.GetInteractivity();

            var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult.Result is null)
                {
                    Console.WriteLine("Time");
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Result.Emoji.Name == delete)
                {
                    await sendmessage.DeleteAsync().ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
            }
            catch (Exception) { }
        }

        public async Task<List<DiscordMessage>> YesterdayReportALL(CommandContext ctxl, DiscordMessage oldmessage, string username, int result, string alias, int arraylength, int arrayentry, List<DiscordMessage> messagelist)
        {
            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Block Report",
                Description = "**" + username + "** You have mined **" + result + "** block yesterday on profile **" + alias + "**.\n\nThis is **" + result * 10 + "** NCG.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };
            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            messagelist.Add(sendmessage);

            if (arraylength == arrayentry)
            {
                await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
                var interactivity = ctxl.Client.GetInteractivity();
                var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);
                try
                {
                    if (reactionresult.Result is null)
                    {
                        await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                        await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                        await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                    }
                    else if (reactionresult.Result.Emoji.Name == delete)
                    {
                        await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                        await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                        await DeleteMessages(messagelist);
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Console.WriteLine("Not Done yet");
                return messagelist;
            }
            return null;
        }

        public async Task WeekBlockReport(CommandContext ctxl, DiscordMessage oldmessage, string username, int result, string alias)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Block Report",
                Description = "**" + username + "** You have mined **" + result + "** block last 7 on profile"+alias+".\n\nThis is **" + result * 10 + "** NCG.\n\nThis is excluding today.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);

            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");

            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);

            var interactivity = ctxl.Client.GetInteractivity();

            var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult.Result is null)
                {
                    Console.WriteLine("Time");
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Result.Emoji.Name == delete)
                {
                    await sendmessage.DeleteAsync().ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
            }
            catch (Exception) { }
        }

        public async Task<List<DiscordMessage>> WeekBlockReportALL(CommandContext ctxl, DiscordMessage oldmessage, string username, int result, string alias, int arraylength, int arrayentry, List<DiscordMessage> messagelist)
        {
            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Block Report",
                Description = "**" + username + "** You have mined **" + result + "** block last 7 on profile" + alias + ".\n\nThis is **" + result * 10 + "** NCG.\n\nThis is excluding today.",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };
            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            messagelist.Add(sendmessage);

            if (arraylength == arrayentry)
            {
                await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
                var interactivity = ctxl.Client.GetInteractivity();
                var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);
                try
                {
                    if (reactionresult.Result is null)
                    {
                        await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                        await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                        await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                    }
                    else if (reactionresult.Result.Emoji.Name == delete)
                    {
                        await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                        await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                        await DeleteMessages(messagelist);
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Console.WriteLine("Not Done yet");
                return messagelist;
            }
            return null;
        }

        public async Task MonthBlockReport(CommandContext ctxl, DiscordMessage oldmessage, string username, int result, string alias)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Block Report",
                Description = "**" + username + "** You have mined **" + result + "** block last 30days on profile **" + alias + "**.\n\nThis is **" + result * 10 + "** NCG.\n\nThis is excluding today",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);

            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");

            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);

            var interactivity = ctxl.Client.GetInteractivity();

            var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult.Result is null)
                {
                    Console.WriteLine("Time");
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Result.Emoji.Name == delete)
                {
                    await sendmessage.DeleteAsync().ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
            }
            catch (Exception) { }
        }

        public async Task<List<DiscordMessage>> MonthBlockReportALL(CommandContext ctxl, DiscordMessage oldmessage, string username, int result, string alias, int arraylength, int arrayentry, List<DiscordMessage> messagelist)
        {
            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Block Report",
                Description = "**" + username + "** You have mined **" + result + "** block last 30days on profile **" + alias + "**.\n\nThis is **" + result * 10 + "** NCG.\n\nThis is excluding today",
                Color = DiscordColor.Green,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            messagelist.Add(sendmessage);

            if (arraylength == arrayentry)
            {
                await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);
                var interactivity = ctxl.Client.GetInteractivity();
                var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);
                try
                {
                    if (reactionresult.Result is null)
                    {
                        await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                        await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                        await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                    }
                    else if (reactionresult.Result.Emoji.Name == delete)
                    {
                        await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                        await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                        await DeleteMessages(messagelist);
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Console.WriteLine("Not Done yet");
                return messagelist;
            }
            return null;
        }

        public async Task NoBlockHistory(CommandContext ctxl, DiscordMessage oldmessage, string username, string alias)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "Block Report",
                Description = "**" + username + "** I was unable to find any recent mining history for the profile **"+alias+"**.",
                Color = DiscordColor.Red,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);

            var delete = DiscordEmoji.FromName(ctxl.Client, ":no_entry_sign:");

            await sendmessage.CreateReactionAsync(delete).ConfigureAwait(false);

            var interactivity = ctxl.Client.GetInteractivity();

            var reactionresult = await interactivity.WaitForReactionAsync(x => x.Emoji.Name == delete, sendmessage, ctxl.User).ConfigureAwait(false);

            try
            {
                if (reactionresult.Result is null)
                {
                    Console.WriteLine("Time");
                    await sendmessage.DeleteAllReactionsAsync("TimedOut").ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
                else if (reactionresult.Result.Emoji.Name == delete)
                {
                    await sendmessage.DeleteAsync().ConfigureAwait(false);
                    await oldmessage.DeleteAllReactionsAsync("Done").ConfigureAwait(false);
                    await oldmessage.CreateReactionAsync(done).ConfigureAwait(false);
                }
            }
            catch (Exception) { }

        }

        public async Task Warning(CommandContext ctxl, DiscordMessage oldmessage, string username, int duration)
        {
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");

            EmbedThumbnail thumbnail = new EmbedThumbnail();
            thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "NCGReport Warning",
                Description = "**" + username + "** depending how much you mine, this can take a while.\n\n**Up to "+duration+" minutes, please be patient**.",
                Color = DiscordColor.Yellow,
                Thumbnail = thumbnail
            };

            var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);

        }

        public async Task WarningTransfer(DiscordClient client,string userkey)
        {
 
            var embedmessage = new DiscordEmbedBuilder
            {
                Title = "**WARNING**",
                Description = "**BlackListed Address**: "+userkey+" has transfered it's gold.",
                Color = DiscordColor.Yellow,
                ImageUrl = client.CurrentUser.AvatarUrl
            };
            DiscordChannel channel = await client.GetChannelAsync(826783795842777102);
            var sendmessage = await channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
            await channel.SendMessageAsync("<@145915312343220224>");
            await channel.SendMessageAsync("<@98809406824673280>");
            await channel.SendMessageAsync("<@293411782561628160>");

        }

        public async Task FullBlockReport(CommandContext ctxl, DiscordMessage oldmessage, string username, BlockReportModel[] list)
        {

            var size = list.Length;
            //set of 50;
            var set = (size / 20) + 1;
            int offset = 0;
            for (int y = 0; y < set; y++)
            {
                string description = String.Empty;
                for (int i = 0; i <= 20; i++)
                {

                    description += "Key: " + list[i+offset].PublicKey + " Total: " + list[i+offset].Blocks;
                }

                EmbedThumbnail thumbnail = new EmbedThumbnail();
                thumbnail.Url = "https://cdn.discordapp.com/avatars/826378705185538059/44f64314271603e6e5ed5ddd60b63ead.png";

                var embedmessage = new DiscordEmbedBuilder
                {
                    Title = "Block Report",
                    Description = "**" + username + "** please find the report below;\n\n\n" + description,
                    Color = DiscordColor.Green,
                    Thumbnail = thumbnail
                };

                var sendmessage = await ctxl.Channel.SendMessageAsync(embed: embedmessage).ConfigureAwait(false);
                offset += 20;
            }
            var done = DiscordEmoji.FromName(ctxl.Client, ":white_check_mark:");
        }
        #endregion
    }
}
