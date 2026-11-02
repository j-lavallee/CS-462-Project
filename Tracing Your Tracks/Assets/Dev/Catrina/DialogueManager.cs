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

    [Tooltip("Characters revealed per second")]
    public float charsPerSecond = 40f;

    DialogueLine[] lines;
    int index;
    UnityEvent onFinished;
    bool open;
    bool locked;
    bool scrolling;
    float revealed;
    int totalChars;
    int startedFrame = -1;
    int closedFrame = -1;

    // True while talking, plus the frame it closes, so that E press doesn't re-trigger an Interactable
    public bool IsBusy => open || locked || Time.frameCount == closedFrame || PauseMenu.IsPaused;

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

    void OnDestroy()
    {
        StarterAssets.PlayerFreeze.Frozen = false;
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

        if (!open || PauseMenu.IsPaused) return;

        if (scrolling)
        {
            revealed += charsPerSecond * Time.deltaTime;
            if (revealed >= totalChars) FinishScrolling();
            else bodyText.maxVisibleCharacters = Mathf.FloorToInt(revealed);
        }

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame && Time.frameCount != startedFrame)
        {
            if (scrolling)
            {
                // First press while typing: fill in the rest of the line
                FinishScrolling();
            }
            else
            {
                AudioManager.PlayDialogueAdvance();
                index++;
                if (index >= lines.Length) End();
                else ShowLine();
            }
        }
    }

    void ShowLine()
    {
        speakerText.text = lines[index].speaker;
        bodyText.text = lines[index].text;
        bodyText.ForceMeshUpdate();
        totalChars = bodyText.textInfo.characterCount;
        revealed = 0f;
        bodyText.maxVisibleCharacters = 0;
        scrolling = totalChars > 0;
        if (!scrolling) bodyText.maxVisibleCharacters = int.MaxValue;
    }

    void FinishScrolling()
    {
        scrolling = false;
        bodyText.maxVisibleCharacters = int.MaxValue;
    }

    void End()
    {
        open = false;
        scrolling = false;
        closedFrame = Time.frameCount;
        panel.SetActive(false);
        if (onFinished != null) onFinished.Invoke();
    }
}