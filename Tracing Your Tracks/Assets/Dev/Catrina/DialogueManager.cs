using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 5)] public string text;
}

// Higher number plays first when several conversations are waiting
public enum DialoguePriority { Deduction = 0, Clue = 1, Npc = 2 }

[Serializable]
public class SpeakerStyle
{
    public string code;          // main name: MC, ID, S, T
    public string[] aliases;     // other names that mean the same speaker
    public string displayName;   // what the name label shows (blank = no label)
    public Color panelColor = new Color(0.23f, 0.16f, 0.12f, 0.94f);
    public bool italic;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject panel;
    public TMP_Text speakerText;
    public TMP_Text bodyText;
    public GameObject hintPanel;

    [Tooltip("Characters revealed per second")]
    public float charsPerSecond = 35f;

    [Header("Speaker styles (leave empty to use the built-in defaults)")]
    public List<SpeakerStyle> styles = new List<SpeakerStyle>();
    public Color defaultPanelColor = new Color(0.23f, 0.16f, 0.12f, 0.94f);

    class Pending
    {
        public DialogueLine[] lines;
        public UnityEvent finished;
        public DialoguePriority priority;
    }

    List<Pending> queue = new List<Pending>();
    Image panelImage;

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

    public bool IsBusy => open || locked || Time.frameCount == closedFrame || PauseMenu.IsPaused;

    void RefreshHint()
    {
        if (hintPanel != null) hintPanel.SetActive(!open && !locked);
    }
    public void SetLocked(bool value)
    {
        locked = value;
        if (!value) closedFrame = Time.frameCount;
        RefreshHint();
    }

    void Awake()
    {
        Instance = this;
        if (panel != null)
        {
            panelImage = panel.GetComponent<Image>();
            panel.SetActive(false);
        }
        if (styles == null || styles.Count == 0) styles = DefaultStyles();
    }

    void OnDestroy()
    {
        StarterAssets.PlayerFreeze.Frozen = false;
    }

    static Color Hex(string h)
    {
        ColorUtility.TryParseHtmlString(h, out Color c);
        return c;
    }

    static List<SpeakerStyle> DefaultStyles()
    {
        return new List<SpeakerStyle>
        {
            new SpeakerStyle { code = "MC", aliases = new[] { "Raccoon" },
                            displayName = "MC", panelColor = Hex("#8E4B63EE") },
            new SpeakerStyle { code = "ID", aliases = new[] { "Internal" },
                            displayName = "", panelColor = Hex("#2F3A4AEE"), italic = true },
            new SpeakerStyle { code = "S",  aliases = new[] { "Mr. S", "Squirrel", "Mr. Squirrel", "Shopkeeper" },
                            displayName = "Mr. S", panelColor = Hex("#4F6B3AEE") },
            new SpeakerStyle { code = "T",  aliases = new[] { "Teal", "Miss Teal", "Cockatiel", "Librarian" },
                            displayName = "Miss Teal", panelColor = Hex("#3F6F8FEE") },
        };
    }

    SpeakerStyle FindStyle(string speaker)
    {
        if (string.IsNullOrWhiteSpace(speaker)) return null;
        speaker = speaker.Trim();

        foreach (var s in styles)
        {
            if (Matches(s.code, speaker)) return s;
            if (s.aliases == null) continue;
            foreach (var alias in s.aliases)
                if (Matches(alias, speaker)) return s;
        }
        return null;
    }

    static bool Matches(string a, string b)
    {
        return string.Equals(a == null ? null : a.Trim(), b, StringComparison.OrdinalIgnoreCase);
    }

    // If another conversation is open, this one waits in line instead of being dropped.
    public void StartDialogue(DialogueLine[] newLines, UnityEvent finished,
                              DialoguePriority priority = DialoguePriority.Clue)
    {
        if (newLines == null || newLines.Length == 0) return;

        if (open)
        {
            queue.Add(new Pending { lines = newLines, finished = finished, priority = priority });
            return;
        }
        Begin(newLines, finished);
    }

    void Begin(DialogueLine[] newLines, UnityEvent finished)
    {
        lines = newLines;
        onFinished = finished;
        index = 0;
        open = true;
        startedFrame = Time.frameCount;
        panel.SetActive(true);
        ShowLine();
        RefreshHint();
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
        var line = lines[index];
        var style = FindStyle(line.speaker);

        speakerText.text = style != null ? style.displayName : line.speaker;
        if (panelImage != null) panelImage.color = style != null ? style.panelColor : defaultPanelColor;
        bodyText.fontStyle = (style != null && style.italic) ? FontStyles.Italic : FontStyles.Normal;

        bodyText.text = line.text;
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

        // If that event didn't start something new, play the next waiting conversation
        if (!open) PlayNextQueued();
        
        RefreshHint();
    }

    void PlayNextQueued()
    {
        if (queue.Count == 0) return;

        int best = 0;
        for (int i = 1; i < queue.Count; i++)
            if (queue[i].priority > queue[best].priority) best = i;

        var next = queue[best];
        queue.RemoveAt(best);
        Begin(next.lines, next.finished);
    }
}