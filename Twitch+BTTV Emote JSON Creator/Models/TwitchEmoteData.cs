using System;

    [Serializable]
    public class TwitchRootobject
    {
        public Datum[] data;
        public string template;
    }

    [Serializable]
    public class Datum
    {
        public string name;
        public Images images;
        public string[] format;
    }

    [Serializable]
    public class Images
    {
        public string url_4x;
    }

    public class TwitchEmoteMetaData
    {
        public string name { get; set; }

        public string url { get; set; }

        public bool isGIF { get; set; }

        public string format { get; set; }
    }