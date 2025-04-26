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

    public Vector2 targetPosition;

    private Dictionary<Vector2, string> animationLookup = new Dictionary<Vector2, string>
    {
        { Vector2.up, "Male1NPCWalkUp" },
        { Vector2.down, "Male1NPCWalkDown" },
        { Vector2.left, "Male1NPCWalkLeft" },
        { Vector2.right, "Male1NPCWalkRight" }
    };
    private string currentAnimation = "Male1NPCWalkUp";

    private float moveSpeed = 1f;

    private bool isWalking = true;

    void Start()
    {
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        animator = GetComponent<Animator>();
        SetNextUnitTarget();
    }

    void Update()
    {
        CheckShouldStopWalking();

        if (!isWalking)
            return;

        if ((Vector2) transform.localPosition != targetPosition)
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
            Vector2 fullTarget = pathCoordinates[index];

            if ((Vector2) transform.localPosition == fullTarget)
            {
                index = (index + 1) % pathCoordinates.Length;
            }

            if (isWalking)
            {
                SetNextUnitTarget();
            }
        }
    }

    private void SetMovementDirection(Vector2 direction)
    {
        movementDirection = Vector2.zero;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            movementDirection.x = Mathf.Sign(direction.x);
        }
        else
        {
            movementDirection.y = Mathf.Sign(direction.y);
        }
    }

    private void SetNextUnitTarget()
    {
        Vector2 fullTarget = pathCoordinates[index];
        Vector2 direction = fullTarget - (Vector2)transform.localPosition;

        if (direction == Vector2.zero)
        {
            targetPosition = fullTarget;
            return;
        }

        SetMovementDirection(direction);

        Vector2 nextStep = (Vector2)transform.localPosition + new Vector2(
            movementDirection.x,
            movementDirection.y
        );

        targetPosition = nextStep;
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
        Vector2 lilithPosition = GameProgressionManagerInstance.lilithGridMovement.lilithCurrentPosition;
        Vector2 lilithNextPosition = GameProgressionManagerInstance.lilithGridMovement.lilithNextPosition;

        if (targetPosition == lilithPosition || targetPosition == lilithNextPosition)
        {
            print($"boy's target: {targetPosition}, lilith position: {lilithPosition}, lilith next position: {lilithNextPosition}");
        Vector2 targetSnapPosition = new Vector2(Mathf.Floor(transform.localPosition.x) + 0.5f, Mathf.Floor(transform.localPosition.y) + 0.5f);

    transform.localPosition = Vector2.Lerp(transform.localPosition, targetSnapPosition, moveSpeed * 2 * Time.deltaTime);
            isWalking = false;
        }
        else
        {
            isWalking = true;
        }
    }
}
