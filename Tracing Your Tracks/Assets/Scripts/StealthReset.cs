using UnityEngine;

public class StealthReset : MonoBehaviour
{
    public Transform resetPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.transform.position = resetPoint.position;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
}
