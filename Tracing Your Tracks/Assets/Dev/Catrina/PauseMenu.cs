using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class MenuTab
{
    public string name;
    public GameObject page;
    public Button button;   // optional
}

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    public GameObject menuRoot;
    public Button exitButton;
    public MenuTab[] tabs;

    StarterAssetsInputs playerInput;
    bool prevCursorLocked;
    bool prevLookInput;

    void Awake()
    {
        IsPaused = false;
        menuRoot.SetActive(false);
        exitButton.onClick.AddListener(ExitGame);

        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            if (tabs[i].button != null)
                tabs[i].button.onClick.AddListener(() => ShowTab(index));
        }
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || !kb.tabKey.wasPressedThisFrame) return;

        if (IsPaused) Resume();
        else Pause();
    }

    void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        menuRoot.SetActive(true);
        ShowTab(0);

        playerInput = FindFirstObjectByType<StarterAssetsInputs>();
        if (playerInput != null)
        {
            prevCursorLocked = playerInput.cursorLocked;
            prevLookInput = playerInput.cursorInputForLook;
            playerInput.cursorLocked = false;
            playerInput.cursorInputForLook = false;
            playerInput.look = Vector2.zero;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        menuRoot.SetActive(false);

        if (playerInput != null)
        {
            playerInput.cursorLocked = prevCursorLocked;
            playerInput.cursorInputForLook = prevLookInput;
            Cursor.lockState = prevCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !prevCursorLocked;
        }
    }

    public void ShowTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].page != null) tabs[i].page.SetActive(i == index);
            // greyed-out button = the tab you're on (placeholder highlight)
            if (tabs[i].button != null) tabs[i].button.interactable = (i != index);
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }
}