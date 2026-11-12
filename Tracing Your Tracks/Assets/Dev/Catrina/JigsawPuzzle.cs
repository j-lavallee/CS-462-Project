using UnityEngine;
using UnityEngine.Events;

public class JigsawPuzzle : MonoBehaviour
{
    public GameObject panel;
    public JigsawPiece[] pieces;
    public UnityEvent onComplete;

    bool open;

    void Awake() { if (panel != null) panel.SetActive(false); }

    public void Open()
    {
        if (open) return;
        open = true;
        panel.SetActive(true);
        DialogueManager.Instance.SetLocked(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PiecePlaced()
    {
        foreach (var p in pieces) if (!p.placed) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        onComplete.Invoke();
    }

    public void Close()
    {
        open = false;
        panel.SetActive(false);
        DialogueManager.Instance.SetLocked(false);
    }
}