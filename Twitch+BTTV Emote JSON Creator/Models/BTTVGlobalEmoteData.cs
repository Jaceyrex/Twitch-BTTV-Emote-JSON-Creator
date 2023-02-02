using Newtonsoft.Json;
using System;

[Serializable]
public class BTTVGlobalRootobject
{
    public Emote[] Emotes { get; set; }
}

[Serializable]
public class Emote
{
    public string id { get; set; }
    public string code { get; set; }
    public string imageType { get; set; }
    public bool animated { get; set; }
}
