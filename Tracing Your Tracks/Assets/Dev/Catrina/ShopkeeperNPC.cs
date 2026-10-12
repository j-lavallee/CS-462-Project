using UnityEngine;
using UnityEngine.Events;

public enum ShopkeeperState { BeforeIntro, Waiting, Finished }

public class ShopkeeperNPC : MonoBehaviour
{
    public ShopkeeperState state = ShopkeeperState.BeforeIntro;
    public bool hasReceipt;

    public TextAsset introFile;
    public TextAsset receiptFile;
    public DialogueLine[] waitingLines;   // plays while he's waiting for you
    public DialogueLine[] finishedLines;  // plays after the receipt conversation

    public UnityEvent onIntroFinished;
    public UnityEvent onReceiptFinished;  // later: the demo end popup hooks in here

    public void Interact()
    {
        if (DialogueManager.Instance == null) return;

        switch (state)
        {
            case ShopkeeperState.BeforeIntro:
                Talk(DialogueParser.Parse(introFile), OnIntroDone);
                break;
            case ShopkeeperState.Waiting:
                if (hasReceipt) Talk(DialogueParser.Parse(receiptFile), OnReceiptDone);
                else Talk(waitingLines, null);
                break;
            case ShopkeeperState.Finished:
                Talk(finishedLines, null);
                break;
        }
    }

    // The stealth section's receipt pickup will call this
    public void ReceiptObtained()
    {
        hasReceipt = true;
    }

    [ContextMenu("Test: Receipt Obtained")]
    void TestReceipt()
    {
        ReceiptObtained();
    }

    void OnIntroDone()
    {
        state = ShopkeeperState.Waiting;
        onIntroFinished.Invoke();
    }

    void OnReceiptDone()
    {
        state = ShopkeeperState.Finished;
        onReceiptFinished.Invoke();
    }

    void Talk(DialogueLine[] lines, UnityAction after)
    {
        var done = new UnityEvent();
        if (after != null) done.AddListener(after);

        if (lines == null || lines.Length == 0)
        {
            done.Invoke();
            return;
        }
        DialogueManager.Instance.StartDialogue(lines, done);
    }
}