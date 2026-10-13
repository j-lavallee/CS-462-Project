using UnityEngine;

public class StealthReset : MonoBehaviour
{
    public Transform resetPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = resetPoint.position;
        }
    }
}
