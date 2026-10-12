using UnityEngine;
using UnityEngine.Events;

public class ObjectiveTracker : MonoBehaviour
{
    public int restockTarget = 3;
    public int clueTarget = 2;

    public DialogueLine[] restockFirstLines;
    public DialogueLine[] cluesFirstLines;
    public DialogueLine[] bothDoneLines;

    public UnityEvent onAllRestocked;
    public UnityEvent onAllCluesFound;
    public UnityEvent onAllDone;   // later: the shopkeeper counter conversation hooks in here

    public int restocked;
    public int cluesFound;

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
        if (restocked >= restockTarget) Finish();
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