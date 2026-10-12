using UnityEngine;
using UnityEngine.InputSystem;

public class TestMover : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsBusy) return;

        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

        Vector3 move = new Vector3(x, 0f, z).normalized;
        transform.position += move * speed * Time.deltaTime;
    }
}