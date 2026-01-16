using UnityEngine;

public class HitSensor : MonoBehaviour
{
    public bool BallInHitCircle;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            BallInHitCircle = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            BallInHitCircle = false;
        }
    }

    
}
