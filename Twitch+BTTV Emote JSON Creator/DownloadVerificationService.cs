using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_BTTV_Emote_JSON_Creator
{
    internal class DownloadVerificationService
    {
        List<TwitchEmoteMetaData> EmoteMetaDataList = new List<TwitchEmoteMetaData>();

        List<EmoteExistsPair> pairs = new List<EmoteExistsPair>();

        List<KeyValuePair<string, bool>> emoteKVP = new List<KeyValuePair<string, bool>>();

        //TODO: Tie each emote to a bool for if it exists or not, false by default - true if it exists with the emotes in the directory. Then go through and list all the ones that are false.
        
        public void VerifyEmotes()
        {
            Console.WriteLine("Verifying emotes....");

            string downloadPath = KnownFolders.Downloads.Path;
            string filePath = downloadPath + "\\EmoteData.json";

            try
            {
                string textBody = File.ReadAllText(filePath);
                EmoteMetaDataList = JsonConvert.DeserializeObject<List<TwitchEmoteMetaData>>(textBody);

                //Outputs title heading before ouputting all emote data.
                Console.WriteLine($"Emote metadata for {EmoteMetaDataList.Count} emotes");

                //Prints the metadata 
                for (int i = 0; i < EmoteMetaDataList.Count; i++)
                {
                    Console.WriteLine($"{i}\t| Emote: {EmoteMetaDataList[i].name,25} \t| Animated: {EmoteMetaDataList[i].isGIF.ToString()} \t| Format: {EmoteMetaDataList[i].format} \t| URL: {EmoteMetaDataList[i].url} ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error attempting to load the file at: {filePath} \n Error: {ex.Message}");
            }


            //for metadatalist, check filename in directory -> do checks to revert names back to original and if match move on if match not found then flag issue
            string directory = KnownFolders.Downloads.Path + "\\TwitchEmotes";

            //Gets names of each file in the directory
            string[] fileList = Directory.GetFiles(directory);

            //To hold emotes that don't exist correctly
            List<string> missingEmotes = new List<string>();

            //For each emote in meta data list
            for (int i = 0; i < EmoteMetaDataList.Count; i++)
            {
                bool notFound = true;

                emoteKVP.Add(new KeyValuePair<string, bool>(EmoteMetaDataList[i].name, false));

                for (int j = 0; j < fileList.Count(); j++)
                {
                    string fileName = fileList[j];

                    //Go through and revert the name of the files back to their original to check against the meta data list names
                    fileName = RevertFileNameToEmote(directory, fileName);

                    Console.WriteLine($"Comparing {EmoteMetaDataList[i].name} against {fileName}");
                    if (EmoteMetaDataList[i].name == fileName)
                    {
                        Console.WriteLine("It's a match!!");
                        KeyValuePair<string, bool> replacementKeyValuePair = new KeyValuePair<string, bool>(EmoteMetaDataList[i].name, true);
                        emoteKVP[i] = replacementKeyValuePair;
                    }
                }
            }

            Console.WriteLine("------------------------ \n List of missing emotes: ");

            int totalMissing = 0;
            for (int i = 0; i < emoteKVP.Count; i++)
            {
                if (emoteKVP[i].Value == false)
                {
                    totalMissing++;
                    Console.WriteLine($"{emoteKVP[i].Key}");
                }
            }
            Console.WriteLine($"Total missing emotes: {totalMissing}\n" +
                $"Consider manually downloading these emotes, as there has been an issue when attempting to download them previously.");
            //for (int i = 0; i < missingEmotes.Count; i++)
            //{
            //    Console.WriteLine($"{missingEmotes[i]}");
            //}
        }

        private static string RevertFileNameToEmote(string directory, string fileName)
        {
            if (fileName.Contains("{lessThan}"))
            {
                Console.WriteLine($"reverting {{lessThan}} to < in {fileName}");
                fileName = fileName.Replace("{lessThan}", "<");

            }
            if (fileName.Contains("{greaterThan}"))
            {
                Console.WriteLine($"reverting {{greaterThan}} to > in {fileName}");
                fileName = fileName.Replace("{greaterThan}", ">");

            }
            if (fileName.Contains("{colon}"))
            {
                Console.WriteLine($"reverting {{colon}} to : in {fileName}");
                fileName = fileName.Replace("{colon}", ":");

            }
            if (fileName.Contains("{doubleQuote}"))
            {
                Console.WriteLine($"reverting {{doubleQuote}} to double quotes in {fileName}");
                fileName = fileName.Replace("{doubleQuote}", @"""");

            }
            if (fileName.Contains("{forwardSlash}"))
            {
                Console.WriteLine($"reverting {{forwardSlash}} to / in {fileName}");
                fileName = fileName.Replace("{forwardSlash}", "/");

            }
            if (fileName.Contains("{backslash}"))
            {
                Console.WriteLine($"reverting {{backslash}} to \\ in {fileName}");
                fileName = fileName.Replace("{backslash}", @"\");

            }
            if (fileName.Contains("{pipe}"))
            {
                Console.WriteLine($"reverting {{pipe}} to | in {fileName}");
                fileName = fileName.Replace("{pipe}", "|");

            }
            if (fileName.Contains("{questionMark}"))
            {
                Console.WriteLine($"reverting {{questionMark}} to ? in {fileName}");
                fileName = fileName.Replace("{questionMark}", "?");

            }
            if (fileName.Contains("{asterisk}"))
            {
                Console.WriteLine($"reverting {{asterisk}} to * in {fileName}");
                fileName = fileName.Replace("{asterisk}", "*");

            }

            fileName = fileName.Replace(".png", "");
            fileName = fileName.Replace(".gif", "");
            fileName = fileName.Replace($"{directory}\\", "");

            return fileName;
        }
    }
    class EmoteExistsPair
    {
        public TwitchEmoteMetaData emote;

        public bool exists;
    }
}
