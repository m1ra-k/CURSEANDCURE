using UnityEngine;

public class HandleBehavior : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Handle collided with: " + other.name);
    }
}
