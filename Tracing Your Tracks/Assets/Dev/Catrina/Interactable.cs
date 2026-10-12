using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    public GameObject circle;
    public GameObject prompt;
    public float showRange = 6f;
    public float interactRange = 2.5f;
    public UnityEvent onInteract;

    Transform player;
    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("No object tagged Player found");
        SetVisuals(false, false);   
    }


    void Update()
    {
        if (player == null) return;

        Vector3 d = player.position - transform.position;
        d.y = 0f;
        float dist = d.magnitude;

        bool inInteract = dist <= interactRange;
        bool inShow = dist <= showRange && !inInteract;
        SetVisuals(inShow, inInteract);

        var kb = Keyboard.current;
        bool talking = DialogueManager.Instance != null && DialogueManager.Instance.IsBusy;
        if (!talking && inInteract && kb != null && kb.eKey.wasPressedThisFrame)
        
        {
            Debug.Log("Interacted with " + name);
            onInteract.Invoke();
        }

        if (prompt != null && prompt.activeSelf && Camera.main != null)
            prompt.transform.rotation = Camera.main.transform.rotation;
    }

    void SetVisuals(bool showCircle, bool showPrompt)
    {
        if (circle != null) circle.SetActive(showCircle);
        if (prompt != null) prompt.SetActive(showPrompt);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, showRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
