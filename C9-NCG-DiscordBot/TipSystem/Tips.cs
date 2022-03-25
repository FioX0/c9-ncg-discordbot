using C9_NCG_DiscordBot.blockchain;
using C9_NCG_DiscordBot.Handlers;
using C9_NCG_DiscordBot.Models;
using DSharpPlus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static C9_NCG_DiscordBot.Enums.TipEnums;

namespace C9_NCG_DiscordBot.TipSystem
{
    class Tips
    {
        public static async Task<Enum> TipsSystemAsync(ulong discordid, ulong mentionid, float amount)
        {

            //Let's Check if profile exists before we do anything
            var db = new SqliteDataAccess();
            var userProfile = new TipModel();
            var mentionProfile = new TipModel();
            userProfile =  await ProfileExistsNew(discordid);
            mentionProfile = await ProfileExistsNew(mentionid);

            if (userProfile.Role == "Admin")
            {
                ///Soft Tip Cap based on available ncg - already given out tips.
                ///
                var ncg = new NCG();
                var balance = AdminBalanceCheck();

                // Needs to check that Total balance of all users is less than Balance
                if (balance.Result < amount)
                    return Status.NEB; //Not Enough Balance
                else if (userProfile.Id == mentionProfile.Id)
                    return Status.PASS; //Can't Tip yourself.
                else
                {
                    //balance we are going to attribute to the receiver
                    float mentionbalance = mentionProfile.Balance + amount;
                    //verify that we managed to update the receiver.
                    bool check = await db.UpdateTipbalance(mentionProfile, mentionbalance);
                    if (check)
                    {   
                        //Let's log the transaction.
                        bool done2 = await db.DumpTransfer("AdminTip", userProfile, mentionProfile, amount);
                        if (done2)
                            return Status.ACOMPLETE;
                        else
                        {
                            //we should never see this.
                            return Status.WTF;
                        }
                    }
                    else
                        return Status.FAIL;
                }
            }
            else
            {
                ///Hard Tip Cap / NON-Admin Request
                ///
                if (userProfile.Balance < amount)
                    return Status.NEB; //Not Enough Balance
                else if (userProfile.Id == mentionProfile.Id)
                    return Status.PASS; //Can't Tip yourself.
                else
                {
                    float userbalance = userProfile.Balance - amount;
                    float mentionblanace = mentionProfile.Balance + amount;
                    //verify that we managed to update the senders's balance
                    bool check = await db.UpdateTipbalance(userProfile, userbalance);
                    if (check)
                    {
                        //now let's add the tip to the sender's balance.
                        bool done = await db.UpdateTipbalance(mentionProfile, mentionblanace);
                        if (done)
                        {
                            //All went well, let's log transaction.
                            bool done2 = await db.DumpTransfer("UserTip", userProfile, mentionProfile, amount);
                            if (done2)
                                return Status.COMPLETE;
                            else
                                return Status.WTF;
                        }
                        else
                        {
                            //shit happened, revert the transaction, give the tip back to the sender and log failure.
                            await db.UpdateTipbalance(userProfile, mentionblanace-amount);
                            await db.DumpTransfer("REVERT", userProfile, mentionProfile, amount);
                            return Status.FAIL;
                        }                    
                    }
                    return Status.FAIL;
                }
            }
        }

        public static async Task<TipModel> ProfileExistsNew(ulong discordid)
        {
            //check if user exists, if not let's create one for them with "user" role.
            var db = new SqliteDataAccess();
            var profile = new TipModel();
            profile = await db.LoadTipProfile(discordid);

            if (profile.Id == 0)
            {
                Console.WriteLine("NewUser");
                await CreateProfileNew(discordid,"User");
                profile = await db.LoadTipProfile(discordid);
            }

            Console.WriteLine("Profile Loaded for DiscordId:" + discordid);
            Console.WriteLine("According to the Profile, your discordid is " + profile.DiscordId);
            return profile;
        }

        public static async Task CreateProfileNew(ulong discordid, string role)
        {
            //creates new profile function
            int balance = 0;
            var db = new SqliteDataAccess();

            Console.WriteLine("Profile doesn't exist, creating one right now.");
            Console.WriteLine("DiscordID being added into the model is:" +discordid);
            TipModel _data = new TipModel();
            _data.Id = discordid;
            _data.Balance = balance;
            _data.Role = role;

            await db.CreateTipProfile(_data);
        }

        public static async Task AdminProfileCheck(ulong discordid, string role)
        {
            //Only called by the TipAdmin Function, requires it's own function as we want to retain the balance if we are upgrading a user.
            var db = new SqliteDataAccess();

            var profile = await db.LoadTipProfile(discordid);
            //Profile already exists or not.
            if (profile.Id == 0)
            {
                Console.WriteLine("NewUser");
                await CreateProfileNew(discordid, role);
            }
            else
            {
                Console.WriteLine("Update Existing user to Admin");
                await db.UpdateTipAdmin(discordid, role);
            }
        }

        public static async Task<float> AdminBalanceCheck()
        {
            //checks how much NCG is available - the amount of Tips already provided to users.
            //Available balance = Total NCG - Amount of Tops already given.
            string botaccount = "0x6AEbea29B88a4b6BB21B877bb6b0E6C6F8C247B8";
            var ncg = new NCG();
            var db = new SqliteDataAccess();

            string balance = ncg.NCGGold(botaccount);
            var usedBalance = db.AdminBalance();
            float result = float.Parse(balance) - float.Parse(usedBalance.Result);
            return result;
        }

        public static async Task<bool> QueueTips(TipModel profile, string key, float amount, ulong userid)
        {
            //checks how much NCG is available - the amount of Tips already provided to users.
            //Available balance = Total NCG - Amount of Tops already given.
            var ncg = new NCG();
            var db = new SqliteDataAccess();
            string botaccount = "0x6AEbea29B88a4b6BB21B877bb6b0E6C6F8C247B8";

            //let's do every possible check in existance.
            string balance = ncg.NCGGold(botaccount);
            Console.WriteLine(balance);
            float floatPot = float.Parse(balance.ToString());

            if (profile.Balance < amount)
                return false;
            if (amount > floatPot)
                return false;
            //Insert Anti-Exploit Function here.
            /////////////////////////////////////
            //Everything Looks fine, let's go ahead.

            //
            float transferamount = amount;
            amount = profile.Balance - amount;
            bool check = await db.UpdateTipbalance(profile, amount); 
            if (check)
            {

                bool check2 = await db.QueueWithdraw(profile, userid, key, transferamount, 1);
                if (check2)
                {
                    await db.DumpTransfer("PaymentQueued", profile, profile, transferamount);
                    return true;
                }
                else
                {
                    await db.UpdateTipbalance(profile, amount+amount);
                    return false;
                }
            }
            return false;
        }

        public static async Task PaymentProcessor(DiscordClient client)
        {
            var db = new SqliteDataAccess();
            var comms = new Communication();
            var ncg = new NCG();
            Console.WriteLine("PaymentProcessor Enabled");
            while(true)
            {
                //every 2 minutes we take 1 payment to process.
                var sw1 = Stopwatch.StartNew();
                for (int ix = 0; ix < 30; ++ix) await Task.Delay(1000);
                sw1.Stop();
                //Console.WriteLine("Sleep: {0}", sw1.ElapsedMilliseconds);
                //are we good to proceed?
                await ChainReady(ncg, db);
                var enabled = db.TipRedeemEnabled();
                if(enabled.Result == 1 && ncg.NCGGold("0x6AEbea29B88a4b6BB21B877bb6b0E6C6F8C247B8") != null && AddressCheck("23061") == "0x6AEbea29B88a4b6BB21B877bb6b0E6C6F8C247B8")
                {
                    Console.WriteLine("TipRedeem is Enabled");
                    PaymentModel paymentprofile = await db.CheckPaymentQueue();
                    TipModel profile = await db.LoadTipProfileForBalance(paymentprofile.DiscordUserId.ToString());

                    if (paymentprofile.Id != 0 && paymentprofile.Amount <= profile.Balance)
                    {
                        if (ncg.NCGGold("0x6AEbea29B88a4b6BB21B877bb6b0E6C6F8C247B8") == null)
                        {
                            //Bot's Snapshot is down, we can't do any transfers.
                            Console.WriteLine("Down");
                        }
                        else
                        {
                            var txid = await ncg.SendNCG(paymentprofile.Key, paymentprofile.Amount);
                            Console.WriteLine("Payment to " + paymentprofile.Key + "has been sent LOCALLY");
                            var sw2 = Stopwatch.StartNew();
                            for (int ix = 0; ix < 300; ++ix) Thread.Sleep(1000);
                            sw2.Stop();
                            //check if TX is on Main Chain
                            var txverify = ncg.CheckNCG(txid);
                            Console.WriteLine("Payment to " + paymentprofile.Key + "has been sent CHAIN with id "+txverify);
                            if (txid == null)
                            {
                                //Something made the payment fail, maybe the snapsshot is down. Let's keep it here for a re-try.
                                //This logic can be expanded later to try 3~ times before it puts the transaction "on-hold" until someone reviews it. For now it will keep trying.
                                await db.DumpPayment("RedeemFail", paymentprofile, paymentprofile, paymentprofile.Amount, txid);
                            }
                            if (txid != null && txverify == null)
                            {
                                //Something made the payment fail, maybe the snapsshot is down. Let's keep it here for a re-try.
                                //This logic can be expanded later to try 3~ times before it puts the transaction "on-hold" until someone reviews it. For now it will keep trying.
                                await db.DumpPayment("RedeemFail", paymentprofile, paymentprofile, paymentprofile.Amount, txid);
                                Console.WriteLine("Chain is Outdated");
                                await db.UpdateTipRedeemStatus(0);
                                //foreach (var process in Process.GetProcessesByName("NineChronicles.Headlesss.Executable.exe"))
                                //{
                                //    process.Kill();
                                //}
                                //System.Diagnostics.Process.Start("C:/Users/carina/AppData/Local/Programs/Nine Chronicles/resources/app/publish/test.bat");
                                //var sw3 = Stopwatch.StartNew();
                                //for (int ix = 0; ix < 30; ++ix) Thread.Sleep(60000);
                                //sw3.Stop();
                            }
                            else if (txid != null && txverify != null)
                            {
                                await db.RemoveWithdrawl(paymentprofile);
                                Console.WriteLine("Payment Succesful");
                                await comms.RedeemSuccess(client, paymentprofile, txid);
                                await db.DumpPayment("RedeemSuccesful", paymentprofile, paymentprofile, paymentprofile.Amount, txid);
                            }
                            else
                            {
                                Console.WriteLine("STOP ME RIGHT NOW");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("TipRedeem is Disabled");
                }
            }
        }

        public static string AddressCheck(string port)
        {
            var client = new RestClient("http://localhost:23061/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query{\\r\\n  minerAddress\\r\\n}\",\"variables\":{}}",
                       ParameterType.RequestBody);
            try
            {
                IRestResponse response = client.Execute(request);
                JObject joResponse = JObject.Parse(response.Content);

                JValue block = (JValue)joResponse["data"]["minerAddress"];
                string topblock = block.ToString();

                return topblock;
            }
            catch (Exception ex) { return ""; };
        }

        public static async Task<bool> ChainReady(NCG ncg, SqliteDataAccess db)
        {
            var ready = ncg.ChainReady();
            //prep
            if (ready.Result.ToString() == "True")
            {
                await db.UpdateTipRedeemStatus(1);
                Console.WriteLine("Chain Ready");
                return true;
            }
            else
            {
                await db.UpdateTipRedeemStatus(0);
                Console.WriteLine("Chain NOT Ready");
                return false;
            }
        }
       
    }
}
