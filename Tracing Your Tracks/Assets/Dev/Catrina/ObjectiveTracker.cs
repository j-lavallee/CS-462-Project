using UnityEngine;
using UnityEngine.Events;

public class ObjectiveTracker : MonoBehaviour
{
    public int restockTarget = 3;
    public int clueTarget = 2;

    public DialogueLine[] restockFirstLines;
    public DialogueLine[] cluesFirstLines;
    public DialogueLine[] bothDoneLines;
    public DialogueLine[] talkToShopkeeperLines; // clues done before the quest started

    public UnityEvent onAllRestocked;
    public UnityEvent onAllCluesFound;
    public UnityEvent onAllDone;

    public bool questStarted;
    public int restocked;
    public int cluesFound;

    // The shopkeeper's intro conversation calls this when it finishes
    public void StartQuest()
    {
        questStarted = true;
    }

    public void RestockDone()
    {
        restocked++;
        if (restocked != restockTarget) return;

        onAllRestocked.Invoke();
        if (cluesFound >= clueTarget) Finish();
        else Say(restockFirstLines);
    }

    public void ClueDone()
    {
        cluesFound++;
        if (cluesFound != clueTarget) return;

        onAllCluesFound.Invoke();
        if (!questStarted) Say(talkToShopkeeperLines);
        else if (restocked >= restockTarget) Finish();
        else Say(cluesFirstLines);
    }

    void Finish()
    {
        onAllDone.Invoke();
        Say(bothDoneLines);
    }

    void Say(DialogueLine[] toSay)
    {
        if (DialogueManager.Instance == null) return;
        DialogueManager.Instance.StartDialogue(toSay, null);
    }
}