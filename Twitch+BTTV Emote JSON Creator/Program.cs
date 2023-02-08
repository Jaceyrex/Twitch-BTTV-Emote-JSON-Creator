using Twitch_BTTV_Emote_JSON_Creator;

partial class Program
{
    static async Task Main(string[] Args)
    {
        //Asks whether to download new JSON
        Console.WriteLine("Download new JSON data for emotes? If no, ensure there is an EmoteData.json file in the downloads folder (Y/N)");

        string response;
        bool correctResponse = false;

        while (!correctResponse)
        {
            response = Console.ReadLine().ToUpper();

            if (response == "Y")
            {
                correctResponse = true;
                var jsonCreatorService = new JSONCreatorService();

                await jsonCreatorService.GetJsonAsync();

                var emoteDownloaderService = new EmoteDownloaderService();

                await emoteDownloaderService.DownloadEmotesFromJSON();
            }
            else if (response == "N")
            {
                correctResponse = true;
                var emoteDownloaderService = new EmoteDownloaderService();

                await emoteDownloaderService.DownloadEmotesFromJSON();
            
            }
            else
            {
                Console.WriteLine("Incorrect answer, Y or N response required.\n");
            }
        }

        

        Console.WriteLine("\n Would you like to verify that every downloaded emote matches the JSON data? (Y/N) \n" +
            "Note: Naming will be resolved automatically");
        correctResponse = false;
        response = "";
        while (!correctResponse)
        {
            response = Console.ReadLine().ToUpper();

            if (response == "Y")
            {
                correctResponse = true;
                var emoteVerificationService = new DownloadVerificationService();

                emoteVerificationService.VerifyEmotes();
            }
            if (response == "N")
            {
                correctResponse = true;
            }
            else
            {
                Console.WriteLine("Incorrect answer, Y or N response required.\n");
            }
        }
    }
}