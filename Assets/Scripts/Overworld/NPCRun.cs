using System.Collections.Generic;
using UnityEngine;

public class NPCRun : MonoBehaviour
{
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;

    private Animator animator;

    private Vector2 movementDirection;
    [SerializeField]
    private Vector2[] pathCoordinates;
    private int index;

    private Vector2 targetPosition;

    private Dictionary<Vector2, string> animationLookup = new Dictionary<Vector2, string>
    {
        { Vector2.up, "Male1NPCWalkUp" },
        { Vector2.down, "Male1NPCWalkDown" },
        { Vector2.left, "Male1NPCWalkLeft" },
        { Vector2.right, "Male1NPCWalkRight" }
    };
    private string currentAnimation = "Male1NPCWalkUp";

    private float moveSpeed = 1f;

    [Header("[Collision]")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float checkRadius = 0.5f;

    void Start()
    {
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();
        animator = GetComponent<Animator>();
        targetPosition = pathCoordinates[0];
        SetMovementDirection();
    }

    void Update()
    {
        if (Vector2.Distance(transform.localPosition, targetPosition) > 0.005f)
        {
            float distanceToMove = moveSpeed * Time.deltaTime;

            while (distanceToMove > 0f)
            {
                Vector2 direction = (targetPosition - (Vector2)transform.localPosition).normalized;
                float step = Mathf.Min(distanceToMove, 0.01f);

                Vector2 nextPosition = (Vector2)transform.localPosition + direction * step;

                // Snap next position to nearest .5 if close enough
                // nextPosition = SnapToHalf(nextPosition);

                // Collision check
                if (Physics2D.OverlapCircle(nextPosition, checkRadius, playerLayer))
                {
                    Debug.Log("NPC would collide with player. Stopping movement.");
                    transform.localPosition = SnapToHalf(transform.localPosition);
                    return;
                }

                transform.localPosition = nextPosition;
                distanceToMove -= step;
            }

            CheckSwitchAnimation();
        }
        else
        {
            // Reached target
            transform.localPosition = SnapToHalf(targetPosition);
            index = (index + 1) % pathCoordinates.Length;
            targetPosition = pathCoordinates[index];
            SetMovementDirection();
        }
    }

    private void SetMovementDirection()
    {
        movementDirection = Vector2.zero;

        if (Mathf.Abs(targetPosition.x - transform.localPosition.x) > Mathf.Epsilon)
        {
            movementDirection.x = targetPosition.x > transform.localPosition.x ? 1 : -1;
        }
        else if (Mathf.Abs(targetPosition.y - transform.localPosition.y) > Mathf.Epsilon)
        {
            movementDirection.y = targetPosition.y > transform.localPosition.y ? 1 : -1;
        }
    }

    private void CheckSwitchAnimation()
    {
        if (movementDirection != Vector2.zero && animationLookup.TryGetValue(movementDirection, out string newAnimation))
        {
            if (currentAnimation != newAnimation)
            {
                animator.Play(newAnimation);
                currentAnimation = newAnimation;
            }
        }
    }

    private Vector2 SnapToHalf(Vector2 position)
    {
        float snappedX = Mathf.Round(position.x * 2f) * 0.5f;
        float snappedY = Mathf.Round(position.y * 2f) * 0.5f;
        return new Vector2(snappedX, snappedY);
    }
}
