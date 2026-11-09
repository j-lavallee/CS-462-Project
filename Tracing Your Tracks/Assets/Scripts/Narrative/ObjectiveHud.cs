using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class ObjectiveEntry
{
    public string text;
    [Tooltip("Same syntax as REQUIRES, one condition per item. All must pass.")]
    public string[] requires;
}

public class ObjectiveHud : MonoBehaviour
{
    public static ObjectiveHud Instance;
    public static string CurrentText = "";   // the pause menu can read this later

    public GameObject panel;
    public TMP_Text label;
    public List<ObjectiveEntry> entries = new List<ObjectiveEntry>();

    void Awake() { Instance = this; }

    void OnEnable()
    {
        GameState.Changed += Refresh;
        Refresh();
    }

    void OnDisable() { GameState.Changed -= Refresh; }

    public void Refresh()
    {
        foreach (var e in entries)
        {
            if (!GameState.CheckAll(e.requires)) continue;

            CurrentText = e.text;
            if (label != null) label.text = "Objective: " + e.text;
            if (panel != null) panel.SetActive(true);
            return;
        }

        CurrentText = "";
        if (panel != null) panel.SetActive(false);
    }
}
