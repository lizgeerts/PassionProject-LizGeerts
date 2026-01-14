using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public EspConnect espData;
    public float moveSpeed = 3f;
    private Vector3 velocity;
    private float deceleration = 5f;


    void Start()
    {
        
    }

    void Update()
    {
        if (espData.buttonPressed)
        {
            Vector3 moveDir = transform.forward * moveSpeed;
            velocity = Vector3.Lerp(velocity, moveDir, Time.deltaTime * 10f);  
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * deceleration);  
        }
        transform.position += velocity * Time.deltaTime;
    }
}
