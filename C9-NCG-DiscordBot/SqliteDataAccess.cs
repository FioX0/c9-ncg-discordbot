using C9_NCG_DiscordBot.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SqlClient;
using MySqlConnector;

namespace C9_NCG_DiscordBot
{

    public class SqliteDataAccess
    {

        # region NCG
        public async Task<ProfileModel> LoadProfile(ulong discordid, string alias)
        {
            if (alias != "")
            {
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    try
                    {
                        string query = "Select * from NCGProfile  WHERE DiscordId = @id and alias =@alias;";
                        var output = cnn.QueryFirst<ProfileModel>(query, new { id = discordid, alias = alias });
                        return output;
                    }
                    catch (Exception) { }
                }
            }

            var fail = new ProfileModel();
            return fail;
        }

        public async Task<ProfileModel[]> LoadProfileALL(ulong discordid, string alias)
        {
            if (alias != "")
            {
                
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    try
                    {
                        var sql = @"Select * from NCGProfile  WHERE DiscordId = @id;";

                        using (var multi = cnn.QueryMultiple(sql, new { id = discordid }))
                        {
                            var customer = multi.Read<ProfileModel>().ToArray();
                            return customer;
                        }
                    }
                    catch (Exception) { }
                }
                return null;
            }
            return null;
        }

        public static bool SaveProfile(ProfileModel profile)
        {
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                Console.WriteLine(profile.Id);
                cnn.Execute("insert into NCGProfile (DiscordId,PublicKey,Value,alias) values (@Id, @PublicKey, @Value, @alias);", profile);
                return true;
            }
        }

        public static bool UpdateProfileKey(ProfileModel profile)
        {
            string sql = "UPDATE NCGProfile SET publickey = @publickey WHERE DiscordId = @id and alias =@alias;";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {

                var affectedRows = cnn.Execute(sql, new { publickey = profile.PublicKey, id = profile.Id, alias = profile.alias }); ;

                Console.WriteLine("Affected Rows: " + affectedRows);
                return true;
            }
        }

        public static int CheckProfile(ProfileModel profile)
        {
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                string query = "Select ID from NCGProfile WHERE DiscordId = @id and alias =@alias;";
                try
                {
                    int output = cnn.QueryFirst<int>(query, new { id = profile.Id, alias = profile.alias });
                    Console.WriteLine(output);
                    return output;
                }
                catch { };
                return 0;
            }
        }

        public async Task<bool> UpdateProfile(ulong discordid, string alias, string value)
        {
            string sql = "UPDATE NCGProfile SET Value = @value WHERE DiscordId = @id and alias =@alias;";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {

                var affectedRows = cnn.Execute(sql, new { value = value, id = discordid, alias = alias }); ;

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }
        #endregion

        #region TipSystem
        public async Task<bool> CreateTipProfile(TipModel profile)
        {
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                Console.WriteLine(profile.Id);
                cnn.Execute("insert into TipProfile (DiscordId,Role,Balance) values (@Id, @Role, @Balance);", profile);
                return true;
            }
            return false;
        }

        public async Task<TipModel> LoadTipProfile(ulong discordid)
        {
            TipModel output;
            try
            {
                string query = "Select * from TipProfile WHERE DiscordId = @id;";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    output = cnn.QueryFirst<TipModel>(query, new { id = discordid });
                    return output;
                }

            }
            catch { };
            var fail = new TipModel();
            return fail;
        }

        public async Task<TipModel> LoadTipProfileForBalance(string discordid)
        {
            TipModel output;
            try
            {
                string query = "Select * from TipProfile WHERE DiscordId = @id;";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    output = cnn.QueryFirst<TipModel>(query, new { id = discordid });
                    return output;
                }
            }
            catch { };
            var fail = new TipModel();
            return fail;
        }

        public async Task<bool> UpdateTipbalance(TipModel profile, float newBalance)
        {
            ulong id = profile.Id;
            string sql = "UPDATE TipProfile SET Balance = @balance WHERE Id = @id;";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new { balance = newBalance, id = id });
                Console.WriteLine("Affected Rows: " + affectedRows);
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> DumpTransfer(string state, TipModel User, TipModel Mention, float amount, string txid = "")
        {
            ulong userid = User.Id;
            ulong mentionid = Mention.Id;
            float ornew;

            if (state == "AdminTip")
                ornew = User.Balance;
            else
                ornew = User.Balance - amount;


            string sql = "insert into TipAudit (OriginProfileId,DestinationProfileId,Details,OriginOldAmount,OriginNewAmount,DestinationOldAmount,DestinationNewAmount,DateTime,TransactionId) values (@id, @id2, @text, @orold, @ornew, @desold, @desnew, @date, @txid);";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                cnn.Execute(sql, new
                {
                    id = userid,
                    id2 = mentionid,
                    text = "[" + state + "] " + "UserId: " + userid + " has made a transfer to UserId: " + mentionid + " in the value of " + amount + "NCG.",
                    ornew = ornew,
                    orold = User.Balance,
                    desnew = Mention.Balance + amount,
                    desold = Mention.Balance,
                    date = DateTime.UtcNow,
                    txid = txid

                });
                return true;
            }
        }

        public async Task<bool> DumpPayment(string state, PaymentModel User, PaymentModel Mention, float amount, string txid = "")
        {
            int userid = User.ProfileId;
            int mentionid = Mention.ProfileId;

            string sql = "insert into TipAudit (OriginProfileId,DestinationProfileId,Details,OriginOldAmount,OriginNewAmount,DestinationOldAmount,DestinationNewAmount,DateTime,TransactionId) values (@id, @id2, @text, @orold, @ornew, @desold, @desnew, @date, @txid);";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                cnn.Execute(sql, new
                {
                    id = userid,
                    id2 = mentionid,
                    text = "[" + state + "] " + "UserId: " + userid + " has cashed out " + amount + " tips.",
                    ornew = 0,
                    orold = 0,
                    desnew = 0,
                    desold = 0,
                    date = DateTime.UtcNow,
                    txid = txid

                });
                return true;
            }
        }

        public async Task<bool> UpdateTipAdmin(ulong discordid, string role)
        {
            string sql = "UPDATE TipProfile SET Role = @role WHERE DiscordId = @id";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new { role = role, id = discordid }); ;

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<string> AdminBalance()
        {
            string output = "";
            try
            {
                string query = "Select sum(Balance) from TipProfile;";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    output = cnn.QueryFirst<string>(query);
                    return output;
                }
            }
            catch { };
            return null;
        }


        public async Task<bool> QueueWithdraw(TipModel profile, ulong discordid, string key, float amount, int authorised, int attempt = 0)
        {


            string sql = "insert into PaymentQueue (ProfileId, DiscordUserId, Key, Amount, Authorised, Attempt) values (@pid, @did, @key, @amount, @auth, @attempt);";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new
                {
                    pid = profile.Id,
                    did = discordid,
                    key = key,
                    amount = amount,
                    auth = authorised,
                    attempt = attempt

                });
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> RemoveWithdrawl(PaymentModel profile)
        {
            string sql = "delete from PaymentQueue where id = @id;";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new { @id = profile.Id }); ;

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<PaymentModel> CheckPaymentQueue()
        {
            int randomid = 1;
            PaymentModel output;
            try
            {
                string query = "Select * from PaymentQueue WHERE Authorised = @id LIMIT 1;";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    output = cnn.QueryFirst<PaymentModel>(query, new { id = randomid });
                    return output;
                }
            }
            catch { };
            var fail = new PaymentModel();
            return fail;
        }

        public async Task<bool> Withdraw(ulong discordid, float amount)
        {

            string sql = "UPDATE TipProfile SET Balance = @amount WHERE DiscordId = @id";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new { amount = amount, id = discordid }); ;

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<int> TipRedeemEnabled()
        {
            int output = 0;
            try
            {
                string query = "Select Enabled from Config where Application = 'TipRedeem';";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    output = cnn.QueryFirst<int>(query);
                    return output;
                }
            }
            catch { };
            return 0;
        }

        public async Task<bool> UpdateTipRedeemStatus(int status)
        {
            string sql = "UPDATE Config SET Enabled = @value WHERE Application = 'TipRedeem'";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new { value = status}); ;

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<int> TipTopUpIDCheck(string ID)
        {
            int output;
            try
            {
                string query = "Select * from TipTopUp WHERE TransactionId = @id;";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    output = cnn.QueryFirst<int>(query, new { id = ID });
                    return output;
                }

            }
            catch { };
            return 0;
        }

        public async Task<bool> TipTopUpAdd(string ID, string sender)
        {
            string sql = "insert into TipTopUp (TransactionId,Sender, DateTime) values (@TransactionId, @Sender, @Date);";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new
                {
                    TransactionId = ID,
                    Sender = sender,
                    Date = DateTime.UtcNow
                });
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }



#endregion

#region RepSystem
public static async Task<RepModel> LoadRepProfile(string discordid)
        {

                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    try
                    {
                        string query = "Select * from Reputation  WHERE DiscordId = @id";
                        var output = cnn.QueryFirst<RepModel>(query, new { id = discordid });
                        return output;
                    }
                    catch (Exception) { }
                }

            var fail = new RepModel();
            return fail;
        }


        public static async Task<bool> CreateRepProfile(string discordid)
        {
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                cnn.Execute("insert into Reputation (UserId,PublicKey,Transactions,ComRep,TradeRep) values (@discordid, '', 0 , 0, 0);");
                return true;
            }
        }
        #endregion

        #region BlockReport
        public static async Task<bool> blockreport(string miner, int blocks)
        {

            string sql = "insert into BlockReport (PublicKey,Blocks) values (@miner,@blocks);";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new
                {
                    miner = miner,
                    blocks = blocks
                });
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public static async Task Deleteblockreport()
        {

            string sql = "DELETE FROM BlockReport;";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql);
            }
        }

        public static async Task<BlockReportModel[]> GetBlockReportData()
        {
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                try
                {
                    var sql = @"Select * from BlockReport Order by Blocks Desc;";

                    using (var multi = cnn.QueryMultiple(sql))
                    {
                        var customer = multi.Read<BlockReportModel>().ToArray();
                        return customer;
                    }
                }
                catch (Exception) { }
            }
            return null;
        }

        public static async Task<int> GetBlockBlockCount()
        {
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                string query = "Select SUM(blocks) from BlockReport;";
                try
                {
                    int output = cnn.QueryFirst<int>(query);
                    Console.WriteLine(output);
                    return output;
                }
                catch { };
                return 0;
            }
        }

        public static async Task<bool> UpdateBlockBlockCount(string id, string blocks)
        {
            string sql = "UPDATE BlockReport SET Control = @value WHERE PublicKey = @id;";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {

                var affectedRows = cnn.Execute(sql, new { value = blocks, id = id }); ;

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }
        #endregion

        #region Extras

        public static bool InsertRPC(ChainModel profile)
        {
            using (MySqlConnection cnn = new MySqlConnection(connectingstring2))
            {
                cnn.Execute("insert into info (Id,address,active,difference,users) values (@nodeid, @address, @active, @index, @users);", profile);
                return true;
            }
        }

        public static async Task<bool> UpdateRPC(ChainModel profile)
        {
            if(profile.nodeid != 0)
            {
                string sql = "UPDATE info SET address = @Address, active = @Active, difference = @Difference, users = @Users, response_time_seconds = @Duration  WHERE Id = @id;";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring2))
                {

                    var affectedRows = cnn.Execute(sql, new { Address = profile.address, Active = profile.active, Difference = profile.index, Users = profile.users, id = profile.nodeid, Duration = profile.duration }); ;

                    Console.WriteLine("Affected Rows: " + affectedRows);
                    return true;
                }
            }
            else
            {
                string sql = "UPDATE info SET address = @Address, active = @Active, difference = @Difference, users = @Users, response_time_seconds = @Duration  WHERE address = @Address;";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring2))
                {

                    var affectedRows = cnn.Execute(sql, new { Address = profile.address, Active = profile.active, Difference = profile.index, Users = profile.users, id = profile.nodeid, Duration = profile.duration }); ;

                    Console.WriteLine("Affected Rows: " + affectedRows);
                    return true;
                }
            }
        }

        public static async Task<bool> UpdateNULLRPC()
        {

            string sql = "UPDATE info SET active ='Unknown', difference = '-99999', users = '-1', response_time_seconds = '-1'";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring2))
            {

                var affectedRows = cnn.Execute(sql);

                Console.WriteLine("Affected Rows: " + affectedRows);
                return true;
            }  
        }

        public float ArenaBlock()
        {
            float output;
            try
            {
                string query = "Select Value from Arena where Name = 'ArenaFinish';";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    output = cnn.QueryFirst<float>(query);
                    return output;
                }
            }
            catch { };
            return 0;
        }

        public float ArenaBlockIgnore()
        {
            float output;
            try
            {
                string query = "Select Value from Arena where Name = 'ArenaIgnore';";
                using (MySqlConnection cnn = new MySqlConnection(connectingstring))
                {
                    output = cnn.QueryFirst<float>(query);
                    return output;
                }
            }
            catch { };
            return 0;
        }

        public async Task<bool> UpdateArena(float value)
        {
            string sql = "UPDATE Arena SET Value = @value WHERE Name = 'ArenaFinish'";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new { value = value }); ;

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> UpdateArenaIgnore(float value)
        {
            string sql = "UPDATE Arena SET Value = @value WHERE Name = 'ArenaIgnore'";
            using (MySqlConnection cnn = new MySqlConnection(connectingstring))
            {
                var affectedRows = cnn.Execute(sql, new { value = value }); ;

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }
        #endregion
    }
}
