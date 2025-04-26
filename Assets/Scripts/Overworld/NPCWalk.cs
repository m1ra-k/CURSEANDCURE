using System.Collections.Generic;
using UnityEngine;

public class NPCWalk : MonoBehaviour
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

    private float moveSpeed = 3f;

    private bool isWalking = true;

    void Start()
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        animator = GetComponent<Animator>();
        targetPosition = pathCoordinates[0];
        SetMovementDirection();
    }

    void Update()
    {
        if (!isWalking)
            return;

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

            CheckShouldStopWalking();

            if (isWalking) // Only set new movement if still walking
            {
                SetMovementDirection();
            }
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

    private void CheckShouldStopWalking()
    {
        // Assuming you have a singleton called GameProgressionManagerInstance
        Vector2 lilithPosition = GameProgressionManagerInstance.lilithPosition;
        Vector2 lilithNextPosition = GameProgressionManagerInstance.lilithGridMovement.lilithNextPosition;

        if (ApproximatelyEqual(targetPosition, lilithPosition) || ApproximatelyEqual(targetPosition, lilithNextPosition))
        {
            isWalking = false;
        }
    }

    private bool ApproximatelyEqual(Vector2 a, Vector2 b)
    {
        // You can adjust this tolerance if needed
        float tolerance = 0.01f;
        return Vector2.Distance(a, b) < tolerance;
    }
}
