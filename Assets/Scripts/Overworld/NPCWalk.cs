using System.Collections.Generic;
using UnityEngine;

public class NPCWalk : MonoBehaviour
{
    private Animator animator;

    private Vector2 movementDirection;
    [SerializeField]
    private Vector2[] pathCoordinates;
    private int index;

    private Vector2 targetPosition;

    private Dictionary<Vector2, string> animationLookup = new Dictionary<Vector2, string>
    {
        { Vector2.up, "LowerWardBoy0NPCWalkUp" },
        { Vector2.down, "LowerWardBoy0NPCWalkDown" },
        { Vector2.left, "LowerWardBoy0NPCWalkLeft" },
        { Vector2.right, "LowerWardBoy0NPCWalkRight" }
    };
    private string currentAnimation = "LowerWardBoy0NPCWalkUp";

    private float moveSpeed = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = pathCoordinates[0];
        SetMovementDirection();
    }

    void Update()
    {
        if ((Vector2)transform.localPosition != targetPosition)
        {
            transform.localPosition = Vector2.MoveTowards(
                transform.localPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            CheckSwitchAnimation();
        }
        else
        {
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
}
