using UnityEngine;
using UnityEngine.Events;

public class DemoFlow : MonoBehaviour
{
    public string introTitle = "The Story So Far";
    [TextArea(4, 10)] public string introText =
        "All the coins in the town fountain have been stolen! They were supposed to go to a charity!\n\n" +
        "Luckily, you've investigated the fountain and found a clue: some chocolate fingerprints and a candy wrapper from the general store.\n\n" +
        "See if you can find out where it came from and look for more clues!";

    public string endTitle = "Demo Complete";
    public AudioClip endSound;
    [TextArea(2, 6)] public string endText =
        "This concludes our demo! What will you discover in the Library next?";

    public UnityEvent onIntroClosed;
    public UnityEvent onDemoEnd;   // later: return to the main menu

    void Start()
    {
        ShowIntro();
    }

    public void ShowIntro()
    {
        if (StoryPopup.Instance == null) { Debug.LogWarning("No StoryPopup in scene"); return; }
        StoryPopup.Instance.Show(introTitle, introText, onIntroClosed);
    }

    public void ShowEnd()
    {
        AudioManager.Play(endSound);
        if (StoryPopup.Instance == null) { Debug.LogWarning("No StoryPopup in scene"); return; }
        StoryPopup.Instance.Show(endTitle, endText, onDemoEnd);
    }
}