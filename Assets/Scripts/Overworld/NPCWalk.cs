using System.Collections.Generic;
using System.Linq;
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

    public string npcName;

    private Dictionary<Vector2, string> animationLookup;
    private string currentAnimation;

    [SerializeField]
    private float moveSpeed = 3f;

    private bool isWalking = true;

    void Start()
    {
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        animator = GetComponent<Animator>();
        SetNextUnitTarget();

        currentAnimation = $"{npcName}WalkLeft";

        animationLookup = new Dictionary<Vector2, string>
        {
            { Vector2.up, $"{npcName}WalkUp" },
            { Vector2.down, $"{npcName}WalkDown" },
            { Vector2.left, $"{npcName}WalkLeft" },
            { Vector2.right, $"{npcName}WalkRight" }
        };
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
        Vector2 direction = fullTarget - (Vector2) transform.localPosition;

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
        bool blocked = targetPosition == GameProgressionManagerInstance.lilithGridMovement.lilithCurrentPosition || targetPosition == GameProgressionManagerInstance.lilithGridMovement.lilithNextPosition;

        if (blocked)
        {
            Vector2 targetSnapPosition = new Vector2(Mathf.Floor(transform.localPosition.x) + 0.5f, Mathf.Floor(transform.localPosition.y) + 0.5f);
            
            if ((Vector2) transform.localPosition != targetSnapPosition) transform.localPosition = Vector2.Lerp(transform.localPosition, targetSnapPosition, moveSpeed * 2 * Time.deltaTime);
            
            if (!GameProgressionManagerInstance.currentlyTalking)
            {
                animator.Play(currentAnimation, 0, 0f);
                animator.speed = 0f;
            }
        }
        
        isWalking = !blocked;

        // TODO CLEAN BUT JUST FOR ANA AT THE END
        if (pathCoordinates.Length == 1 && (Vector2) transform.localPosition == pathCoordinates[0]) 
        {
            isWalking = false;
            animator.Play(currentAnimation, 0, 0f);
            animator.speed = 0f;
        }

        if (isWalking && animator.speed == 0)  animator.speed = 1f;
    }
}
