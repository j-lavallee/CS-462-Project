using UnityEngine;
using UnityEngine.Events;

public enum SpotType { Restock, RedHerring, Clue }

public class SearchSpot : MonoBehaviour
{
    public SpotType type;
    public DialogueLine[] lines;        // plays the first time
    public DialogueLine[] repeatLines;  // plays after it's been used for the first time
    public ClueData clue;               // only used by Clue spots
    public UnityEvent onFirstSearch;    // the objective tracker will connect here later

    bool used;

    public void Interact()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("No DialogueManager in scene");
            return;
        }

        // Red herrings never remember anything
        if (type == SpotType.RedHerring)
        {
            Play(lines, null);
            return;
        }

        if (used)
        {
            Play(repeatLines, null);
            return;
        }

        var done = new UnityEvent();
        done.AddListener(OnFirstFinished);
        Play(lines, done);
    }

    void OnFirstFinished()
    {
        used = true;

        if (type == SpotType.Clue && clue != null && ClueManager.Instance != null)
            ClueManager.Instance.Collect(clue);

        onFirstSearch.Invoke();
    }

    void Play(DialogueLine[] toPlay, UnityEvent done)
    {
        if (toPlay == null || toPlay.Length == 0)
        {
            if (done != null) done.Invoke();
            return;
        }
        DialogueManager.Instance.StartDialogue(toPlay, done);
    }
}