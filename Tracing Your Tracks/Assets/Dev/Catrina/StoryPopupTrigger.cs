using UnityEngine;
using UnityEngine.Events;

public class StoryPopupTrigger : MonoBehaviour
{
    public string title;
    [TextArea(3, 8)] public string body;
    public UnityEvent onClosed;

    public void Play()
    {
        if (StoryPopup.Instance == null)
        {
            Debug.LogWarning("StoryPopupTrigger: no StoryPopup in this scene.");
            return;
        }
        StoryPopup.Instance.Show(title, body, onClosed);
    }
}