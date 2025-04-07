using UnityEngine;

public class HandleBehavior : MonoBehaviour
{
    [Header("[Self]")]
    private Collider2D handleCollider;
    private int framesInsideRange;

    [Header("[Healing Game Manager]")]
    [SerializeField]
    private HealingGameManager healingGameManager;

    void Start()
    {
        handleCollider = GetComponent<Collider2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!healingGameManager.finishedGame && HandleFullyInside(handleCollider, other))
        {
            Debug.Log("fully inside trigger: " + other.name);
            framesInsideRange++;
            // two second leeway
            healingGameManager.score = Mathf.FloorToInt(framesInsideRange / 1680f * 100);
        }
    }

    bool HandleFullyInside(Collider2D inner, Collider2D outer)
    {
        Bounds innerBounds = inner.bounds;
        Bounds outerBounds = outer.bounds;

        return outerBounds.Contains(innerBounds.min) && outerBounds.Contains(innerBounds.max);
    }
}
