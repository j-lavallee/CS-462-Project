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

    // ---- Debug ----
    public static IEnumerable<string> AllFlags() => flags;
    public static IEnumerable<KeyValuePair<string, int>> AllCounters() => counters;
}
