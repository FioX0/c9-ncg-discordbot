using C9_NCG_DiscordBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace C9_NCG_DiscordBot.blockchain
{
    class Setup
    {

        public bool SetProfileNew(string publickey,string alias, ulong discordid)
        {
            ProfileModel profile = new ProfileModel();
            profile.Id = discordid;
            profile.PublicKey = publickey;
            profile.alias = alias;
            profile.Value = 0;
            //Check if user doesn't already have a profile with the same alias, if so let's update this entry instead
            if (SqliteDataAccess.CheckProfile(profile) != 0)
            {
                return SqliteDataAccess.UpdateProfileKey(profile);
            }
            else
            {
                //New Profile        
                return SqliteDataAccess.SaveProfile(profile);
            }
        }


    }
}
