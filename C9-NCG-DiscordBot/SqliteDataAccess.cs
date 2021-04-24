﻿using C9_NCG_DiscordBot.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace C9_NCG_DiscordBot
{
    public class SqliteDataAccess
    {
        # region NCG
        public async Task<ProfileModel> LoadProfile(ulong discordid, string alias)
        {
            if (alias != "")
            {
                using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
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
        public static bool SaveProfile(ProfileModel profile)
        {
            using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
            {
                Console.WriteLine(profile.Id);
                cnn.Execute("insert into NCGProfile (DiscordId,PublicKey,Value,alias) values (@Id, @PublicKey, @Value, @alias);", profile);
                return true;
            }
        }

        public static bool UpdateProfileKey(ProfileModel profile)
        {
            string sql = "UPDATE NCGProfile SET publickey = @publickey WHERE DiscordId = @id and alias =@alias;";
            using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
            {

                var affectedRows = cnn.Execute(sql, new { publickey = profile.PublicKey, id = profile.Id, alias = profile.alias }); ;

                Console.WriteLine("Affected Rows: " + affectedRows);
                return true;
            }
        }

        public static int CheckProfile(ProfileModel profile)
        {
            using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
            {
                string query = "Select ID from NCGProfile WHERE DiscordId = @id and alias =@alias;";
                try
                {
                    int output = cnn.QueryFirst<int>(query, new { id = profile.Id, alias = profile.alias});
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
            using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
            {

                var affectedRows = cnn.Execute(sql, new { value = value, id = discordid, alias = alias }); ;

                Console.WriteLine("Affected Rows: " + affectedRows);
                return true;
            }
        }
        #endregion


        #region TipSystem
        public async Task<bool> CreateTipProfile(TipModel profile)
        {
            using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
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
                using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
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
                using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
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
            using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
            {
                var affectedRows = cnn.Execute(sql, new { balance = newBalance, id = id}); 
                Console.WriteLine("Affected Rows: " + affectedRows);
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> DumpTransfer(string state, TipModel User, TipModel Mention, float amount)
        {
            ulong userid = User.Id;
            ulong mentionid = Mention.Id;
            float ornew;

            if (state == "AdminTip")
                ornew = User.Balance;
            else
                ornew = User.Balance - amount;


            string sql = "insert into TipAudit (OriginProfileId,DestinationProfileId,Details,OriginOldAmount,OriginNewAmount,DestinationOldAmount,DestinationNewAmount,DateTime) values (@id, @id2, @text,@orold,@ornew,@desold,@desnew,@date);";
            using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
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
                    date = DateTime.UtcNow
                });
                return true;
            }
        }

        public async Task<bool> UpdateTipAdmin(ulong discordid, string role)
        {
            string sql = "UPDATE TipProfile SET Role = @role WHERE DiscordId = @id";
            using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
            { 
                var affectedRows = cnn.Execute(sql, new { role = role, id = discordid}); ;

                Console.WriteLine("Affected Rows: " + affectedRows);
                return true;
            }
        }

        public async Task<string> AdminBalance()
        {
            string output ="";
            try
            {
                string query = "Select sum(Balance) from TipProfile;";
                using (IDbConnection cnn = new SQLiteConnection("Data Source=database.db;"))
                {
                    output = cnn.QueryFirst<string>(query);
                    return output;
                }
            }
            catch { };
            return null;
        }
        #endregion
    }
}
