using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;

    public GameObject popupPanel;
    public TMP_Text popupTitle;
    public TMP_Text popupDescription;
    public float popupSeconds = 3f;
    public List<ClueData> collected = new List<ClueData>();

    Coroutine hideRoutine;

    void Awake()
    {
        Instance = this;
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void Collect(ClueData clue)
    {
        if (clue == null) return;

        string flag = "CLUE_" + clue.name;
        if (GameState.Has(flag))
        {
            Debug.Log("Already have " + clue.clueTitle);
            return;
        }

        collected.Add(clue);
        AudioManager.Play(clue.pickupSound);

        GameState.Set(flag);
        GameState.Add("CLUES_FOUND");
        if (!string.IsNullOrEmpty(clue.area))
            GameState.Add("CLUES_" + clue.area.ToUpper());

        ShowPopup("Clue found: " + clue.clueTitle, clue.description);
    }

    public void ShowPopup(string title, string body)
    {
        if (popupPanel == null) return;
        popupTitle.text = title;
        popupDescription.text = body;
        popupPanel.SetActive(true);

        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(popupSeconds);
        popupPanel.SetActive(false);
    }
}