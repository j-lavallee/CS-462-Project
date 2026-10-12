using System.Collections.Generic;
using UnityEngine;

public static class DialogueParser
{
    public static DialogueLine[] Parse(TextAsset file)
    {
        var result = new List<DialogueLine>();
        if (file == null) return result.ToArray();

        foreach (string raw in file.text.Split('\n'))
        {
            string line = raw.Trim();
            if (line.Length == 0) continue;

            var dl = new DialogueLine();
            int colon = line.IndexOf(':');
            if (colon > 0 && colon <= 12)
            {
                dl.speaker = line.Substring(0, colon).Trim();
                dl.text = line.Substring(colon + 1).Trim();
            }
            else
            {
                dl.text = line;
            }
            result.Add(dl);
        }
        return result.ToArray();
    }
}