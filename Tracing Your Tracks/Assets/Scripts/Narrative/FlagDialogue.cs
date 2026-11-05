using System.Collections.Generic;
using UnityEngine;

public class DialogueBlock
{
    public string name = "(default)";
    public List<string> requires = new List<string>();
    public List<string> sets = new List<string>();
    public List<DialogueLine> lines = new List<DialogueLine>();
}

public static class FlagDialogue
{
    // ---- Parsing ----
    public static List<DialogueBlock> Parse(TextAsset file)
    {
        var blocks = new List<DialogueBlock>();
        if (file == null) return blocks;

        DialogueBlock current = null;

        foreach (string raw in file.text.Split('\n'))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//")) continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                current = new DialogueBlock { name = line.Substring(1, line.Length - 2).Trim() };
                blocks.Add(current);
                continue;
            }

            if (current == null)
            {
                current = new DialogueBlock();
                blocks.Add(current);
            }

            if (line.StartsWith("REQUIRES:", System.StringComparison.OrdinalIgnoreCase))
                AddList(current.requires, line.Substring(9));
            else if (line.StartsWith("SETS:", System.StringComparison.OrdinalIgnoreCase))
                AddList(current.sets, line.Substring(5));
            else
                current.lines.Add(ParseLine(line));
        }
        return blocks;
    }

    static void AddList(List<string> target, string text)
    {
        foreach (string item in text.Split(','))
        {
            string t = item.Trim();
            if (t.Length > 0) target.Add(t);
        }
    }

    static DialogueLine ParseLine(string line)
    {
        var dl = new DialogueLine();
        int colon = line.IndexOf(':');
        if (colon > 0 && colon <= 20)
        {
            dl.speaker = line.Substring(0, colon).Trim();
            dl.text = line.Substring(colon + 1).Trim();
        }
        else dl.text = line;
        return dl;
    }

    // ---- Choosing a block ----
    public static DialogueBlock Pick(List<DialogueBlock> blocks)
    {
        if (blocks == null) return null;
        foreach (var b in blocks)
        {
            bool ok = true;
            foreach (string cond in b.requires)
                if (!Passes(cond)) { ok = false; break; }
            if (ok) return b;
        }
        return null;
    }

    public static bool Passes(string cond)
    {
        cond = cond.Trim();
        if (cond.StartsWith("!")) return !GameState.Has(cond.Substring(1).Trim());

        string[] ops = { ">=", "<=", "==", ">", "<" };
        foreach (string op in ops)
        {
            int i = cond.IndexOf(op);
            if (i <= 0) continue;

            string name = cond.Substring(0, i).Trim();
            if (!int.TryParse(cond.Substring(i + op.Length).Trim(), out int n))
            {
                Debug.LogWarning("Bad number in condition: " + cond);
                return false;
            }
            int v = GameState.Get(name);
            switch (op)
            {
                case ">=": return v >= n;
                case "<=": return v <= n;
                case "==": return v == n;
                case ">":  return v > n;
                case "<":  return v < n;
            }
        }
        return GameState.Has(cond);
    }

    // ---- Applying SETS ----
    public static void Apply(string cmd)
    {
        cmd = cmd.Trim();
        if (cmd.StartsWith("!")) { GameState.Set(cmd.Substring(1).Trim(), false); return; }

        int plus = cmd.IndexOf('+');
        if (plus > 0 && int.TryParse(cmd.Substring(plus + 1).Trim(), out int add))
        {
            GameState.Add(cmd.Substring(0, plus).Trim(), add);
            return;
        }

        int eq = cmd.IndexOf('=');
        if (eq > 0 && int.TryParse(cmd.Substring(eq + 1).Trim(), out int val))
        {
            GameState.SetCount(cmd.Substring(0, eq).Trim(), val);
            return;
        }

        GameState.Set(cmd);
    }
}
