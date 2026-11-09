using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NpcBranch
{
    public string label;             // just a name for you, e.g. "Intro" (shows as the list element title)
    public string[] requires;        // all must pass; empty = always plays
    public TextAsset file;           // long conversations (.txt)
    public DialogueLine[] lines;     // short lines, used only if File is empty
    public string[] setsOnFinish;    // applied when the conversation ends
    public UnityEvent onFinished;
}

public class LibraryNPC : MonoBehaviour
{
    [Tooltip("Checked top to bottom. The first branch whose Requires all pass is played.")]
    public NpcBranch[] branches;

    public void Interact()
    {
        if (DialogueManager.Instance == null || branches == null) return;

        foreach (var b in branches)
        {
            if (b.requires != null && b.requires.Length > 0 && !GameState.CheckAll(b.requires)) continue;
            Play(b);
            return;
        }
    }

    void Play(NpcBranch b)
    {
        DialogueLine[] toPlay = b.file != null ? DialogueParser.Parse(b.file) : b.lines;

        var done = new UnityEvent();
        done.AddListener(() =>
        {
            if (b.setsOnFinish != null && b.setsOnFinish.Length > 0) GameState.ApplyAll(b.setsOnFinish);
            b.onFinished?.Invoke();
        });

        if (toPlay == null || toPlay.Length == 0) { done.Invoke(); return; }
        DialogueManager.Instance.StartDialogue(toPlay, done, DialoguePriority.Npc);
    }
}