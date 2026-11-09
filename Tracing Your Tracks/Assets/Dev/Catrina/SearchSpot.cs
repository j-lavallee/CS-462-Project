using UnityEngine;
using UnityEngine.Events;

public enum SpotType { Restock, RedHerring, Clue, Dialogue }

[System.Serializable]
public class FlagLines
{
    public string[] requires;          // all must pass for these lines to play
    public DialogueLine[] lines;
}

public class SearchSpot : MonoBehaviour
{
    public SpotType type;
    public DialogueLine[] lines;        // plays the first time
    public DialogueLine[] repeatLines;  // plays after it's been used for the first time
    public ClueData clue;
    public AudioClip firstSearchSound;               // only used by Clue spots
    public UnityEvent onFirstSearch;    // the objective tracker will connect here later
    public DialoguePriority priority = DialoguePriority.Clue;

    [Header("Flags (leave empty to ignore)")]
    [Tooltip("All must pass, or the spot is locked")]
    public string[] requires;
    [Tooltip("Plays when locked. Empty = says nothing")]
    public DialogueLine[] lockedLines;
    [Tooltip("Applied when the first search finishes")]
    public string[] setsOnFirstSearch;
    [Tooltip("Optional. Remembers this spot was searched even after changing scenes")]
    public string searchedFlag;
    [Tooltip("Repeat visits: the first entry whose requires pass replaces Repeat Lines")]
    public FlagLines[] repeatBranches;
    bool used;

    public void Interact()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("No DialogueManager in scene");
            return;
        }
        
        if (!GameState.CheckAll(requires))
        {
            Play(lockedLines, null);
            return;
        }

        // Red herrings never remember anything
        if (type == SpotType.RedHerring)
        {
            Play(lines, null);
            return;
        }

        if (IsUsed())
        {
            Play(PickRepeatLines(), null);
            return;
        }

        var done = new UnityEvent();
        done.AddListener(OnFirstFinished);
        Play(lines, done);
    }

    void OnFirstFinished()
    {
        used = true;

        if (!string.IsNullOrEmpty(searchedFlag)) GameState.Set(searchedFlag);
        GameState.ApplyAll(setsOnFirstSearch);

        if (type == SpotType.Clue && clue != null && ClueManager.Instance != null)
            ClueManager.Instance.Collect(clue);

        onFirstSearch.Invoke();
    }

    bool IsUsed()
    {
        return used || (!string.IsNullOrEmpty(searchedFlag) && GameState.Has(searchedFlag));
    }

    DialogueLine[] PickRepeatLines()
    {
        if (repeatBranches != null)
            foreach (var b in repeatBranches)
                if (GameState.CheckAll(b.requires)) return b.lines;
        return repeatLines;
    }
    void Play(DialogueLine[] toPlay, UnityEvent done)
    {
        if (toPlay == null || toPlay.Length == 0)
        {
            if (done != null) done.Invoke();
            return;
        }
        DialogueManager.Instance.StartDialogue(toPlay, done, priority);
    }
}