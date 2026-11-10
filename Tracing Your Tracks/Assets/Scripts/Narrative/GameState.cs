using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    static HashSet<string> flags = new HashSet<string>();
    static Dictionary<string, int> counters = new Dictionary<string, int>();

    // Anything that needs to re-check state (objective text, state checks) listens to this
    public static event Action Changed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnPlay()
    {
        flags.Clear();
        counters.Clear();
        Changed = null;
    }

    // ---- Flags ----
    public static bool Has(string flag) => flags.Contains(flag);

    public static void Set(string flag, bool value = true)
    {
        bool changed = value ? flags.Add(flag) : flags.Remove(flag);
        if (changed) Changed?.Invoke();
    }

    // ---- Counters ----
    public static int Get(string counter)
    {
        return counters.TryGetValue(counter, out int v) ? v : 0;
    }

    public static void SetCount(string counter, int value)
    {
        counters[counter] = value;
        Changed?.Invoke();
    }

    public static void Add(string counter, int amount = 1)
    {
        SetCount(counter, Get(counter) + amount);
    }

     // ---- Conditions (REQUIRES lists in the Inspector) ----
    // Examples: "MY_FLAG"   "!MY_FLAG"   "CLUES_FOUND >= 2"   (also <=, ==, >, <)
    public static bool Check(string cond)
    {
        if (string.IsNullOrWhiteSpace(cond)) return true;
        cond = cond.Trim();
        if (cond.StartsWith("!")) return !Has(cond.Substring(1).Trim());

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
            int v = Get(name);
            switch (op)
            {
                case ">=": return v >= n;
                case "<=": return v <= n;
                case "==": return v == n;
                case ">":  return v > n;
                case "<":  return v < n;
            }
        }
        return Has(cond);
    }

    public static bool CheckAll(string[] conds)
    {
        if (conds == null) return true;
        foreach (string c in conds)
            if (!Check(c)) return false;
        return true;
    }

    // ---- Commands (SETS lists in the Inspector) ----
    // Examples: "MY_FLAG" turns it on, "!MY_FLAG" turns it off,
    //           "MY_COUNT+1" adds to a counter, "MY_COUNT=3" sets it
    public static void Apply(string cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd)) return;
        cmd = cmd.Trim();
        if (cmd.StartsWith("!")) { Set(cmd.Substring(1).Trim(), false); return; }

        int plus = cmd.IndexOf('+');
        if (plus > 0 && int.TryParse(cmd.Substring(plus + 1).Trim(), out int add))
        {
            Add(cmd.Substring(0, plus).Trim(), add);
            return;
        }

        int eq = cmd.IndexOf('=');
        if (eq > 0 && int.TryParse(cmd.Substring(eq + 1).Trim(), out int val))
        {
            SetCount(cmd.Substring(0, eq).Trim(), val);
            return;
        }

        Set(cmd);
    }

    public static void ApplyAll(string[] cmds)
    {
        if (cmds == null) return;
        foreach (string c in cmds) Apply(c);
    }
    
    // ---- Debug ----
    public static IEnumerable<string> AllFlags() => flags;
    public static IEnumerable<KeyValuePair<string, int>> AllCounters() => counters;
}
