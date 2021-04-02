using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace C9_NCG_DiscordBot.blockchain
{
    class Setup
    {
        public bool SetProfile(string publickey, ulong discordid)
        {
            //save location
            string docPath = "C:/profilerecord/profile.txt";
            string temp = "C:/profilerecord/tempprofile.txt";
            string checkstring ="";

            //Check if this is the first time someone setting profile (aka file doesn't exist yet)
            if (!File.Exists(docPath))
            {
                using (StreamWriter sw = File.CreateText(docPath))
                {
                    sw.WriteLine(discordid+":"+publickey);
                    sw.Close();
                    return true;
                }
            }

            //This if is a bit obsolete but might as well be safe rather than sorry.
            if(File.Exists(docPath))
            {
                Console.WriteLine("File Exists");
                //Let's check if they already have a key set.
                string[] lines = File.ReadAllLines(docPath);
                bool isMatch = false;
                if (File.ReadAllText(docPath).Contains(discordid.ToString()))
                {
                    using (StreamWriter sw = File.AppendText(temp))
                    {
                        for (int x = 0; x < lines.Length; x++)
                        {
                            checkstring = lines[x].Split(':')[0];
                            Console.WriteLine(checkstring);
                            if (discordid.ToString() == checkstring)
                            {
                                Console.WriteLine("Found an existing entry for ID:" + discordid);
                            }
                            else
                            {
                                sw.WriteLine(lines[x]);
                            }
                        }
                        sw.WriteLine(discordid + ":" + publickey);
                        sw.Close();
                        File.Delete(docPath);
                        File.Move(temp,docPath);
                        return true;
                    }
                }
                if (!isMatch)
                {
                    Console.WriteLine("No Match");       
                    using (StreamWriter sw = File.AppendText(docPath))
                    {
                        sw.WriteLine(discordid + ":" + publickey);
                        Console.WriteLine("No math found for: " + discordid + " this has now been added.");
                        sw.Close();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
