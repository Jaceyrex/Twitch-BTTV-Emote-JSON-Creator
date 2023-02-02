using Newtonsoft.Json;
using System;

[Serializable]
public class BTTVChannelRootobject
{
    public Sharedemote[] sharedEmotes { get; set; }
}

[Serializable]
public class Sharedemote
{
    public string id { get; set; }
    public string code { get; set; }
    public string imageType { get; set; }
    public bool animated { get; set; }
}