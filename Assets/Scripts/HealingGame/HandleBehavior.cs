using UnityEngine;

public class HandleBehavior : MonoBehaviour
{
    private Collider2D handleCollider;

    void Start()
    {
        handleCollider = GetComponent<Collider2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (HandleFullyInside(handleCollider, other))
        {
            Debug.Log("fully inside trigger: " + other.name);
        }
    }

    bool HandleFullyInside(Collider2D inner, Collider2D outer)
    {
        Bounds innerBounds = inner.bounds;
        Bounds outerBounds = outer.bounds;

        return outerBounds.Contains(innerBounds.min) && outerBounds.Contains(innerBounds.max);
    }
}
