using System.Collections.Generic;
using UnityEngine;

public class NPCRun : MonoBehaviour
{
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;

    private Animator animator;

    [SerializeField] private Vector2[] pathCoordinates;
    private int index;
    private Vector2 targetPosition;
    private Vector2 movementDirection;

    [Header("[Movement]")]
    [SerializeField] private float moveSpeed = 1f;

    [Header("[Collision]")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float checkDistance = 0.55f;

    private Dictionary<Vector2, string> animationLookup = new Dictionary<Vector2, string>
    {
        { Vector2.up, "Male1NPCWalkUp" },
        { Vector2.down, "Male1NPCWalkDown" },
        { Vector2.left, "Male1NPCWalkLeft" },
        { Vector2.right, "Male1NPCWalkRight" }
    };
    private string currentAnimation = "";

    private bool isMoving = true;
    private bool waitingForClearPath = false;

    void Start()
    {
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();
        animator = GetComponent<Animator>();

        targetPosition = pathCoordinates[0];
        SetMovementDirection();
    }

    void Update()
    {
        if (!isMoving && waitingForClearPath)
        {
            TryResumeMovement();
            return;
        }

        if (!isMoving) return;

        Vector2 direction = (targetPosition - (Vector2)transform.localPosition).normalized;

        if (Physics2D.Raycast(transform.localPosition, direction, checkDistance, playerLayer))
        {
            StopAndSnap();
            return;
        }

        transform.localPosition += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.localPosition, targetPosition) <= 0.05f)
        {
            transform.localPosition = SnapToHalf(targetPosition);
            index = (index + 1) % pathCoordinates.Length;
            targetPosition = pathCoordinates[index];
            SetMovementDirection();
        }

        UpdateAnimation();
    }

    private void SetMovementDirection()
    {
        Vector2 diff = targetPosition - (Vector2) transform.localPosition;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            movementDirection = new Vector2(Mathf.Sign(diff.x), 0);
        }
        else
        {
            movementDirection = new Vector2(0, Mathf.Sign(diff.y));
        }
    }

    private void UpdateAnimation()
    {
        if (animationLookup.TryGetValue(movementDirection, out string newAnimation))
        {
            if (currentAnimation != newAnimation)
            {
                animator.Play(newAnimation);
                currentAnimation = newAnimation;
            }
        }
    }

    private void StopAndSnap()
    {
        isMoving = false;
        waitingForClearPath = true;
        transform.localPosition = SnapToHalf(transform.localPosition);
    }

    private void TryResumeMovement()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.localPosition).normalized;
        Vector2 checkPosition = (Vector2)transform.localPosition + direction * 0.25f;

        float checkRadius = 0.3f;

        if (!Physics2D.OverlapCircle(checkPosition, checkRadius, playerLayer))
        {
            isMoving = true;
            waitingForClearPath = false;
            SetMovementDirection();
        }
    }

    private Vector2 SnapToHalf(Vector2 position)
    {
        return new Vector2(
            Mathf.Round(position.x * 2f) * 0.5f,
            Mathf.Round(position.y * 2f) * 0.5f
        );
    }
}
