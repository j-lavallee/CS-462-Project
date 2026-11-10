using UnityEngine;

// Marks where the player appears when arriving through a SceneExit
public class SpawnPoint : MonoBehaviour
{
    public string id;

    public static string NextId;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnPlay()
    {
        NextId = null;
    }

    void Start()
    {
        if (string.IsNullOrEmpty(NextId) || NextId != id) return;
        NextId = null;   

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        player.transform.SetPositionAndRotation(transform.position, transform.rotation);
        if (controller != null) controller.enabled = true;
    }
}
