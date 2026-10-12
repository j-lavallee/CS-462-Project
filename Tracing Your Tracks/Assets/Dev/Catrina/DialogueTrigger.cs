using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine[] lines;
    public UnityEvent onFinished;

    public void Play()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("No DialogueManager in scene");
            return;
        }
        DialogueManager.Instance.StartDialogue(lines, onFinished);
    }
}