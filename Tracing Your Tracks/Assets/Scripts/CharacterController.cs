using UnityEngine;

public class CharacterController : MonoBehaviour
{

    public float movespeed = 5f;
    private Vector3 initalPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalPosition = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0f, moveZ);

        Vector3 newPosition = transform.position + move * movespeed * Time.deltaTime;
        transform.position = newPosition;
        
    }
}
