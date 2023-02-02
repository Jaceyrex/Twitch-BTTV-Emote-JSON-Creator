using System;
using Newtonsoft.Json;

List<TwitchEmoteMetaData> EmoteMetaDataList = new List<TwitchEmoteMetaData>();

string twitchGlobalUri = $"https://api.twitch.tv/helix/chat/emotes/global";
string twitchChannelUri = $"https://api.twitch.tv/helix/chat/emotes?broadcaster_id={Secrets.broadcaster_id}";
string BTTVGlobalUri = $"https://api.betterttv.net/3/cached/emotes/global";
string BTTVChannelUri = $"https://api.betterttv.net/3/cached/users/twitch/{Secrets.BTTV_id}";

//Task TwitchGlobal = TwitchGetRequest(twitchGlobalUri);
//Task TwitchChannel = TwitchGetRequest(twitchChannelUri);
Task BTTVGlobal = BTTVGlobalGetRequest(BTTVGlobalUri);
Task BTTVShared = BTTVChannelGetRequest(BTTVChannelUri);

//Waits for all tasks to finish to ensure list is filled
//TwitchGlobal.Wait();
//TwitchChannel.Wait();
BTTVGlobal.Wait();
BTTVShared.Wait();

//Outputs title heading before ouputting all emote data.
Console.WriteLine($"Emote metadata for {EmoteMetaDataList.Count} emotes");


for (int i = 0; i < EmoteMetaDataList.Count; i++)
{
    Console.WriteLine($"{i} | Emote: {EmoteMetaDataList[i].name} | URL: {EmoteMetaDataList[i].url} | Animated: {EmoteMetaDataList[i].isGIF.ToString()}");
}

async Task TwitchGetRequest(string uri)
{
    List<TwitchEmoteMetaData> tempEmoteList = new List<TwitchEmoteMetaData>();

    var client = new HttpClient();

    try
    {
        //Sends headers
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Secrets.OAUTH_TOKEN_NO_PREFIX);
        client.DefaultRequestHeaders.Add("Client-Id", Secrets.client_id);

        HttpResponseMessage response = await client.GetAsync(twitchGlobalUri);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<TwitchRootobject>(responseBody);

        tempEmoteList = result.data
                    .Select(data => new TwitchEmoteMetaData
                    {
                        isGIF = data.format.Contains("animated"),
                        name = data.name,
                        url = data.images.url_4x,
                    }).ToList();

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
    List<TwitchEmoteMetaData> tempEmoteList = new List<TwitchEmoteMetaData>();

    var client = new HttpClient();

    try
    {
        HttpResponseMessage response = await client.GetAsync(twitchGlobalUri);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<BTTVGlobalEmotesRoot>(responseBody);

        tempEmoteList = result.emotes
                        .Select(emotes => new TwitchEmoteMetaData
                        {
                            isGIF = emotes.animated,
                            name = emotes.code,
                            url = $"https://cdn.betterttv.net/emote/{emotes.id}/3x.png" //Using PNG to avoid issues with webp in Unity, for animated emotes this gets replaced with .gif
                        }).ToList();

        //replaced .png with .gif if emote is animated (may not work in final program with .webp conversion)
        for (int i = 0; i < tempEmoteList.Count; i++)
        {
            if (tempEmoteList[i].isGIF)
            {
                tempEmoteList[i].url.Replace(".png", ".gif");
            }
        }

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
    List<TwitchEmoteMetaData> tempEmoteList = new List<TwitchEmoteMetaData>();

    var client = new HttpClient();

    try
    {
        HttpResponseMessage response = await client.GetAsync(twitchGlobalUri);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<BTTVChannelRootobject>(responseBody);

        tempEmoteList = result.emotes
                        .Select(emotes => new TwitchEmoteMetaData
                        {
                            isGIF = emotes.animated,
                            name = emotes.code,
                            url = $"https://cdn.betterttv.net/emote/{emotes.id}/3x.png" //Using PNG to avoid issues with webp in Unity, for animated emotes this gets replaced with .gif
                        }).ToList();

        //replaced .png with .gif if emote is animated (may not work in final program with .webp conversion)
        for (int i = 0; i < tempEmoteList.Count; i++)
        {
            if (tempEmoteList[i].isGIF)
            {
                tempEmoteList[i].url.Replace(".png", ".gif");
            }
        }

        EmoteMetaDataList.AddRange(tempEmoteList);
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("\nException Caught in BTTV Channel Get Request");
        Console.WriteLine($"Message : {e.Message}");
    }
}