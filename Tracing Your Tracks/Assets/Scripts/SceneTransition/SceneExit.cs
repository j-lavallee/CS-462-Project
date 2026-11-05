using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

// While the player is inside the trigger, the prompt shows.
// Pressing E loads the destination scene.
[RequireComponent(typeof(Collider))]
public class SceneExit : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject prompt;
    public string destinationName;
    public string promptFormat = "Press E to go to the {0}";

    bool playerInside;

    void Start()
    {
        if (prompt == null) return;

        var label = prompt.GetComponentInChildren<TMP_Text>(true);
        if (label != null && !string.IsNullOrEmpty(destinationName))
            label.text = string.Format(promptFormat, destinationName);

        prompt.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }

    void Update()
    {
        bool busy = PauseMenu.IsPaused ||
                    (DialogueManager.Instance != null && DialogueManager.Instance.IsBusy);
        bool canLeave = playerInside && !busy;

        if (prompt != null)
        {
            prompt.SetActive(canLeave);
            if (canLeave && Camera.main != null)
                prompt.transform.rotation = Camera.main.transform.rotation;
        }

        var kb = Keyboard.current;
        if (canLeave && kb != null && kb.eKey.wasPressedThisFrame)
        {
            Debug.Log($"{name}: loading {sceneToLoad}");   
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}