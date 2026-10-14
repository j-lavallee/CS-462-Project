using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StoryPopup : MonoBehaviour
{
    public static StoryPopup Instance;

    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text bodyText;

    UnityEvent onClosed;
    bool open;
    int shownFrame = -1;

    void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void Show(string title, string body, UnityEvent closed)
    {
        if (panel == null) return;
        titleText.text = title;
        bodyText.text = body;
        onClosed = closed;
        open = true;
        shownFrame = Time.frameCount;
        panel.SetActive(true);
        if (DialogueManager.Instance != null) DialogueManager.Instance.SetLocked(true);
    }

    void Update()
    {
        if (!open || PauseMenu.IsPaused) return;
        var kb = Keyboard.current;
        if (kb != null && kb.eKey.wasPressedThisFrame && Time.frameCount != shownFrame)
            Close();
    }

    void Close()
    {
        open = false;
        panel.SetActive(false);
        if (DialogueManager.Instance != null) DialogueManager.Instance.SetLocked(false);
        if (onClosed != null) onClosed.Invoke();
    }
}