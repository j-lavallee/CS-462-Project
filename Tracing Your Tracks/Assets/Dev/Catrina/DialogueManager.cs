using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 5)] public string text;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject panel;
    public TMP_Text speakerText;
    public TMP_Text bodyText;

    DialogueLine[] lines;
    int index;
    UnityEvent onFinished;
    bool open;
    int startedFrame = -1;
    int closedFrame = -1;

    // True while talking, plus the frame it closes, so that E press doesn't re-trigger an Interactable
    bool locked;

    public bool IsBusy => open || locked || Time.frameCount == closedFrame;

    public void SetLocked(bool value)
    {
        locked = value;
        if (!value) closedFrame = Time.frameCount;
    }

    void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void StartDialogue(DialogueLine[] newLines, UnityEvent finished)
    {
        if (open || newLines == null || newLines.Length == 0) return;
        lines = newLines;
        onFinished = finished;
        index = 0;
        open = true;
        startedFrame = Time.frameCount;
        panel.SetActive(true);
        ShowLine();
    }

    void Update()
    {
        StarterAssets.PlayerFreeze.Frozen = IsBusy;
        if (!open) return;
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame && Time.frameCount != startedFrame)
        {
            index++;
            if (index >= lines.Length) End();
            else ShowLine();
        }
    }

    void OnDestroy()
    {
        StarterAssets.PlayerFreeze.Frozen = false;
    }

    void ShowLine()
    {
        speakerText.text = lines[index].speaker;
        bodyText.text = lines[index].text;
    }

    void End()
    {
        open = false;
        closedFrame = Time.frameCount;
        panel.SetActive(false);
        if (onFinished != null) onFinished.Invoke();
    }
}