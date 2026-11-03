using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlagSpot : MonoBehaviour
{
    public TextAsset dialogueFile;
    public DialoguePriority priority = DialoguePriority.Clue;
    public ClueData clueOnFinish;      // optional: collected when the played block finishes
    public UnityEvent onFinished;      // optional: extra things to trigger (sounds, objects)

    List<DialogueBlock> blocks;

    void Awake()
    {
        blocks = FlagDialogue.Parse(dialogueFile);
    }

    public void Interact()
    {
        if (DialogueManager.Instance == null) return;

        var block = FlagDialogue.Pick(blocks);
        if (block == null)
        {
            Debug.Log(name + ": no block's requirements passed, nothing to play");
            return;
        }

        if (block.lines.Count == 0)
        {
            Finish(block);
            return;
        }

        var done = new UnityEvent();
        done.AddListener(() => Finish(block));
        DialogueManager.Instance.StartDialogue(block.lines.ToArray(), done, priority);
    }

    void Finish(DialogueBlock block)
    {
        foreach (string cmd in block.sets) FlagDialogue.Apply(cmd);

        if (clueOnFinish != null && ClueManager.Instance != null)
            ClueManager.Instance.Collect(clueOnFinish);

        onFinished.Invoke();
    }
}
