using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StateRule
{
    public string label;         // just a name to reference
    public string[] requires;    // all must pass
    public string[] sets;        // applied when they do
    public UnityEvent onFired;   // optional: runs right after Sets are applied
}

public class LibraryStateCheck : MonoBehaviour
{
    [Tooltip("A rule fires when all Requires pass. Always include \"!FLAG\" for the flag it sets, so it only fires once.")]
    public StateRule[] rules;

    bool running;

    void OnEnable()  { GameState.Changed += Recheck; }
    void OnDisable() { GameState.Changed -= Recheck; }
    void Start()     { Recheck(); }

    void Recheck()
    {
        if (running || rules == null) return;
        running = true;
        try
        {
            // One rule firing can make another pass, so repeat until nothing fires.
            for (int pass = 0; pass < 10; pass++)
            {
                bool anyFired = false;
                foreach (var r in rules)
                {
                    if (r.requires == null || r.requires.Length == 0) continue;
                    if (r.sets == null || r.sets.Length == 0) continue;
                    if (!GameState.CheckAll(r.requires)) continue;
                    GameState.ApplyAll(r.sets);
                    r.onFired?.Invoke();
                    anyFired = true;
                }
                if (!anyFired) break;
            }
        }
        finally { running = false; }
    }
}