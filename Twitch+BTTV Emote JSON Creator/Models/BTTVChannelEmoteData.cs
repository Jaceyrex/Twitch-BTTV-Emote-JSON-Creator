using System;

[Serializable]
public class BTTVChannelRootobject
{
    public SharedEmote[] emotes;
}

[Serializable]
public class SharedEmote
{
    public string id;
    public string code;
    public bool animated;
}
