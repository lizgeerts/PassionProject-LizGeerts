using UnityEngine;

public class BallCollideScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
            Debug.Log("Ball hit floor");
        else if (other.CompareTag("Cage"))
            Debug.Log("Ball hit cage");
        else if (other.CompareTag("Glass"))
            Debug.Log("Ball hit glass");
    }
}
