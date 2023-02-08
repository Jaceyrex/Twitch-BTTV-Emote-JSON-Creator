using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;

namespace Twitch_BTTV_Emote_JSON_Creator
{
    internal class JSONCreatorService
    {
        //Creates new list
        List<TwitchEmoteMetaData> EmoteMetaDataList = new List<TwitchEmoteMetaData>();

        //Setting uri's using secret variables stored elsewhere.
        string twitchGlobalUri = $"https://api.twitch.tv/helix/chat/emotes/global";
        string twitchChannelUri = $"https://api.twitch.tv/helix/chat/emotes?broadcaster_id={Secrets.broadcaster_id}";
        string BTTVGlobalUri = $"https://api.betterttv.net/3/cached/emotes/global";
        string BTTVChannelUri = $"https://api.betterttv.net/3/cached/users/twitch/{Secrets.BTTV_id}";

        public async Task GetJsonAsync()
        {
            //does the GET requests for all emote information
            await TwitchGetRequest(twitchGlobalUri);
            await TwitchGetRequest(twitchChannelUri);
            await BTTVGlobalGetRequest(BTTVGlobalUri);
            await BTTVChannelGetRequest(BTTVChannelUri);

            //Outputs title heading before ouputting all emote data.
            Console.WriteLine($"Emote metadata for {EmoteMetaDataList.Count} emotes");

            //Prints the metadata 
            for (int i = 0; i < EmoteMetaDataList.Count; i++)
            {
                Console.WriteLine($"{i}\t| Emote: {EmoteMetaDataList[i].name,25} \t| Animated: {EmoteMetaDataList[i].isGIF.ToString()} \t| URL: {EmoteMetaDataList[i].url} ");
            }

            //Serialises the list into a JSON format
            var serialisedList = JsonConvert.SerializeObject(EmoteMetaDataList, Formatting.Indented);

            //Gets downloads folder path (Seems to be the most sensible way to get it in case of updated / changed locations for folders)
            string downloadPath = KnownFolders.Downloads.Path;


            //Async writes the file to the download path and "nicely" prints location.
            await File.WriteAllTextAsync(downloadPath + "\\EmoteData.json", serialisedList);
            Console.WriteLine($"\n" +
                                    $"---------------------------------------------------------------------" +
                                    $"\nJSON has been saved to {downloadPath}\\EmoteData.json");

        }

        //Gets the home path 
        static string GetHomePath()
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                return System.Environment.GetEnvironmentVariable("HOME");
            }

            return System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }

        //Twitch GET request
        async Task TwitchGetRequest(string uri)
        {

            //Creates a new temporary emote list to store gathered emote metadata
            List<TwitchEmoteMetaData> tempEmoteList = new List<TwitchEmoteMetaData>();

            //creates a new HTTP Client
            var client = new HttpClient();

            try
            {
                //Sends headers
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Secrets.OAUTH_TOKEN_NO_PREFIX);
                client.DefaultRequestHeaders.Add("Client-Id", Secrets.client_id);

                //Waits for response from the GET request
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                //Sets a string to be the contents of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                //Creates object from the GET response
                var result = JsonConvert.DeserializeObject<TwitchRootobject>(responseBody);

                //Sets temp list data based on retrieved data
                tempEmoteList = result.data
                            .Select(data => new TwitchEmoteMetaData
                            {
                                isGIF = data.format.Contains("animated"),
                                name = data.name,
                                url = data.images.url_4x,
                            }).ToList();

                //Ensures format of the image is correct (as Twitch API response doesn't include file format, only whether it's animated or static)
                for (int i = 0; i < tempEmoteList.Count; i++)
                {
                    if (tempEmoteList[i].isGIF == false)
                    {
                        tempEmoteList[i].format = "png";
                    }
                    else if (tempEmoteList[i].isGIF == true)
                    {
                        tempEmoteList[i].format = "gif";

                        //Replaces the static portion of URL (Thank you Twitch Madge) with default
                        string[] urlArray = tempEmoteList[i].url.Split('/');

                        for (int j = 0; j < urlArray.Length; j++)
                        {
                            //replaces static with default for animated emotes (URL will only show animated version when NOT set to static, then rejoins the string as a URL
                            if (urlArray[j] == "static")
                            {
                                urlArray[j] = "default";
                                tempEmoteList[i].url = String.Join("/", urlArray);

                            }
                        }
                    }
                }

                //Adds temporary emotes to the full list of emote metadata.
                EmoteMetaDataList.AddRange(tempEmoteList);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught in Twitch Get Request!");
                Console.WriteLine($"Message : {e.Message}");
            }
        }

        async Task BTTVGlobalGetRequest(string uri)
        {
            //Creates a new temporary emote list to store gathered emote metadata
            List<TwitchEmoteMetaData> tempEmoteList = new List<TwitchEmoteMetaData>();

            //creates a new HTTP Client
            var client = new HttpClient();

            try
            {
                //Waits for response from the GET request
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                //Sets a string to be the contents of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                //Creates object from the GET response
                var result = JsonConvert.DeserializeObject<Emote[]>(responseBody);

                //Sets temp list data based on retrieved data
                tempEmoteList = result
                                .Select(emotes => new TwitchEmoteMetaData
                                {
                                    isGIF = emotes.animated,
                                    format = emotes.imageType,
                                    name = emotes.code,
                                    url = $"https://cdn.betterttv.net/emote/{emotes.id}/3x.{emotes.imageType}" //Using PNG to avoid issues with webp in Unity, for animated emotes this gets replaced with .gif
                                }).ToList();

                //Adds temporary emotes to the full list of emote metadata.
                EmoteMetaDataList.AddRange(tempEmoteList);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught in BTTV Global Get Request!");
                Console.WriteLine($"Message : {e.Message}");
            }
        }

        async Task BTTVChannelGetRequest(string uri)
        {
            //Creates a new temporary emote list to store gathered emote metadata
            List<TwitchEmoteMetaData> tempEmoteList = new List<TwitchEmoteMetaData>();

            //creates a new HTTP Client
            var client = new HttpClient();

            try
            {
                //Waits for response from the GET request
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                //Sets a string to be the contents of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                //Creates object from the GET response
                var result = JsonConvert.DeserializeObject<BTTVChannelRootobject>(responseBody);

                //Sets temp list data based on retrieved data
                tempEmoteList = result.sharedEmotes
                                .Select(emotes => new TwitchEmoteMetaData
                                {
                                    isGIF = emotes.animated,
                                    format = emotes.imageType,
                                    name = emotes.code,
                                    url = $"https://cdn.betterttv.net/emote/{emotes.id}/3x.{emotes.imageType}" //Using PNG to avoid issues with webp in Unity, for animated emotes this gets replaced with .gif
                                }).ToList();

                //Adds temporary emotes to the full list of emote metadata.
                EmoteMetaDataList.AddRange(tempEmoteList);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught in BTTV Channel Get Request");
                Console.WriteLine($"Message : {e.Message}");
            }
        }
    }
}
