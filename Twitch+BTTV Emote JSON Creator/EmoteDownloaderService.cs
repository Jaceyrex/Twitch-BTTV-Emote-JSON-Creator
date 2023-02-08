using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Microsoft.WindowsAPICodePack.Shell;
using System.Net;

namespace Twitch_BTTV_Emote_JSON_Creator
{
    internal class EmoteDownloaderService
    {
        //Creates new list
        List<TwitchEmoteMetaData> EmoteMetaDataList = new List<TwitchEmoteMetaData>();
        public async Task<Task<int>> DownloadEmotesFromJSON()
        {
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

            if (EmoteMetaDataList != null)
            {
                string emoteDirectory = downloadPath + "\\TwitchEmotes";

                //Checks if directory doesn't exist, creates it if not
                if (!Directory.Exists(emoteDirectory))
                {
                    Directory.CreateDirectory(emoteDirectory);
                    Console.WriteLine($"Emote directory did not exist previously, it has been created at: {emoteDirectory} \n" +
                        $"-------------");
                }
                else
                {
                    Console.WriteLine($"The directory at: {emoteDirectory} - already exists, no action taken with folder structure \n" +
                        $"-------------");
                }
                for (int i = 0; i < EmoteMetaDataList.Count; i++)
                {
                    try
                    {
                        //Filepath that the image will be saved to (will be changed if 
                        string imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";

                        //checks to see if the image doesn't already exist in the directory
                        //if (!Directory.Exists(imageFilePath))
                        //{
                        using (HttpClient client = new HttpClient())
                        {
                            using (var response = client.GetAsync(EmoteMetaDataList[i].url).Result)
                            {
                                response.EnsureSuccessStatusCode();

                                using (var stream = response.Content.ReadAsStreamAsync().Result)
                                {
                                    //List of characters illegal in file syntax
                                    char[] bannedChars = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
                                    char[] emoteNameAsChars = EmoteMetaDataList[i].name.ToCharArray();

                                    //Used to determine whether a file has no illegal characters
                                    bool containsNoIllegalCharacters = true;

                                    //Check for characters that are banned from being in file names (bit hardcoded but gets the job done I suppose)
                                    //Replaces illegal characters with bracketed descriptor
                                    if (emoteNameAsChars.Contains('<'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing < with {{lessThan}}");

                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace("<", "{lessThan}");
                                        //EmoteMetaDataList[i].name.Replace("<", "{Less Than}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";

                                        containsNoIllegalCharacters = true;
                                    }
                                    if (EmoteMetaDataList[i].name.Contains('>'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing > with {{greaterThan}}");
                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace(">", "{greaterThan}");

                                        //EmoteMetaDataList[i].name.Replace(">", "{greater than}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";

                                        containsNoIllegalCharacters = true;
                                    }
                                    if (EmoteMetaDataList[i].name.Contains(':'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing : with {{colon}}");

                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace(":", "{colon}");
                                        //EmoteMetaDataList[i].name.Replace(":", "{colon}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";

                                        containsNoIllegalCharacters = true;
                                    }
                                    if (EmoteMetaDataList[i].name.Contains('"'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing quotes with {{doubleQuote}}");

                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace("quotes", "{doubleQuote}");
                                        //EmoteMetaDataList[i].name.Replace(@"""", "{double quote}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";
                                        containsNoIllegalCharacters = true;
                                    }
                                    if (EmoteMetaDataList[i].name.Contains('/'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing / with {{forwardSlash}}");

                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace("/", "{forwardSlash}");
                                        //EmoteMetaDataList[i].name.Replace("/", "{forward slash}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";
                                        containsNoIllegalCharacters = true;
                                    }
                                    if (EmoteMetaDataList[i].name.Contains('\\'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing \\ with {{backslash}}");

                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace("\\", "{backslash}");
                                        //EmoteMetaDataList[i].name.Replace("\\", "{backslash}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";
                                        containsNoIllegalCharacters = true;
                                    }
                                    if (EmoteMetaDataList[i].name.Contains('|'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing | with {{pipe}}");

                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace("|", "{pipe}");
                                        //EmoteMetaDataList[i].name.Replace("|", "{pipe}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";
                                        containsNoIllegalCharacters = true;
                                    }
                                    if (EmoteMetaDataList[i].name.Contains('?'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing ? with {{questionMark}}");

                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace("?", "{questionMark}");
                                        //EmoteMetaDataList[i].name.Replace("?", "{question mark}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";
                                        containsNoIllegalCharacters = true;
                                    }
                                    if (EmoteMetaDataList[i].name.Contains('*'))
                                    {
                                        Console.WriteLine($"For ' {EmoteMetaDataList[i].name} ' replacing * with {{asterisk}}");

                                        EmoteMetaDataList[i].name = EmoteMetaDataList[i].name.Replace("*", "{asterisk}");
                                        //EmoteMetaDataList[i].name.Replace("*", "{asterisk}");
                                        imageFilePath = $"{emoteDirectory}\\{EmoteMetaDataList[i].name}.{EmoteMetaDataList[i].format}";
                                        containsNoIllegalCharacters = true;
                                    }


                                    //Saves the image
                                    using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
                                    {
                                        stream.CopyTo(fileStream);
                                        if (containsNoIllegalCharacters)
                                        {
                                            Console.WriteLine($"Saved {EmoteMetaDataList[i].name} to ' {imageFilePath} '");
                                        }

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load the image with exception: {ex.Message}");
                    }
                }
            }

            //ends task
            return Task.FromResult(0);  
        }
    }
}
