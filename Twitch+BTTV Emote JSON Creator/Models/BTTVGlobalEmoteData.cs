using System;

[Serializable]
public class BTTVGlobalEmotesRoot
{
    public BTTVGlobalEmote[] emotes;
}

[Serializable]
public class BTTVGlobalEmote
{
    public string id;
    public string code;
    public bool animated;

}

