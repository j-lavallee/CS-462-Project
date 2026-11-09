using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class OpeningSequence : MonoBehaviour
{
    public GameObject newspaper;       // the full-screen newspaper overlay
    public DialogueLine[] newspaperLines;
    public string[] setsAfterNewspaper;
    public DialogueLine[] arrivalLines;
    public string[] setsAfterArrival;
    public float startDelay = 0.5f;

    IEnumerator Start()
    {
        if (newspaper != null) newspaper.SetActive(false);
        if (GameState.Has("NEW_GAME_STARTED")) yield break;

        GameState.Set("NEW_GAME_STARTED");
        yield return null;   // let the managers finish starting
        if (DialogueManager.Instance != null) DialogueManager.Instance.SetLocked(true);

        yield return new WaitForSeconds(startDelay);
        if (newspaper != null) newspaper.SetActive(true);
        yield return null;   // so a key held from earlier doesn't count
        while (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame || PauseMenu.IsPaused)
            yield return null;

        AfterNewspaper();
    }

    void AfterNewspaper()
    {
        if (newspaper != null) newspaper.SetActive(false);
        GameState.ApplyAll(setsAfterNewspaper);
        Play(newspaperLines, PlayArrival);
    }

    void PlayArrival()
    {
        Play(arrivalLines, AfterArrival);
    }

    void AfterArrival()
    {
        GameState.Set("SQUARE_CONTROL_GIVEN");
        GameState.ApplyAll(setsAfterArrival);
        if (DialogueManager.Instance != null) DialogueManager.Instance.SetLocked(false);
    }

    void Play(DialogueLine[] toPlay, UnityAction next)
    {
        if (toPlay == null || toPlay.Length == 0 || DialogueManager.Instance == null) { next(); return; }

        var done = new UnityEvent();
        done.AddListener(next);
        DialogueManager.Instance.StartDialogue(toPlay, done, DialoguePriority.Npc);
    }
}
