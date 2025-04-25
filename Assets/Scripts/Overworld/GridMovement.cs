using UnityEngine;
using UnityEngine.SceneManagement;


public class GridMovement : MonoBehaviour
{
    // TODO: NEED TO SORT ALL
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;

    [Header("[Properties]")]
    public string directionFacing = "down";
    public Vector2 movementVector;
    public Vector2 prevMovementVector;
    public bool isMoving;
    private bool completedFirstMovement;
    public bool overrideIsMoving;
    public bool currentlyDoorTransitioning;

    private float moveSpeed = 5f;
    private float stepSize = 1f;
    private Vector2 targetPosition;

    public AnimationClip[] lilithAnimations;
    private Animator animator;

    private AudioSource audioSource;
    private string bumpDirection = "";

    [SerializeField] private float checkRadius = 0.1f;

    void Start() 
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        targetPosition = transform.position;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animator.Play("LilithWalkDown", 0, 0f);
        animator.speed = 0;
    }

    void Update() 
    {
        Move();

        if (movementVector != prevMovementVector)
        {
            completedFirstMovement = true;
            DetermineAnimation();
        }

        if (movementVector == Vector2.zero && completedFirstMovement)
        {
            DetermineStopFrame();
        }

        prevMovementVector = movementVector;
    }

    void Move() 
    {
        if (!overrideIsMoving)
        {
            if (GameProgressionManagerInstance.currentlyTalking || currentlyDoorTransitioning || GameProgressionManagerInstance.transitioning)
            {
                isMoving = false;
                movementVector = Vector2.zero;
            }
            else
            {
                if (!isMoving)
                {
                    if (Input.GetKey(KeyCode.UpArrow)) 
                    {
                        movementVector = Vector2.up;
                        directionFacing = "up";
                    }
                    else if (Input.GetKey(KeyCode.DownArrow)) 
                    {
                        movementVector = Vector2.down;
                        directionFacing = "down";
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow)) 
                    {
                        movementVector = Vector2.left;
                        directionFacing = "left";
                    }
                    else if (Input.GetKey(KeyCode.RightArrow)) 
                    {
                        movementVector = Vector2.right;
                        directionFacing = "right";
                    }
                    else
                    {
                        movementVector = Vector2.zero;
                    }

                    if (movementVector != Vector2.zero) 
                    {
                        TryStep();
                    }
                }
                else 
                {
                    FinishStep();
                }
            }
        }
        else
        {
            if (!isMoving)
            {   
                TryStep();
            }
            else
            {
                FinishStep();
            }
        }
    }

    // ANIMATIONS
    void DetermineAnimation()
    {
        animator.speed = 1;

        switch (movementVector)
        {
            case Vector2 v when v == Vector2.up:
                print("up");
                animator.Play(lilithAnimations[!GameProgressionManagerInstance.currentLocation.Equals("lowerWard") ? 0 : 4].name, 0, 0f);
                break;
            case Vector2 v when v == Vector2.down:
                print("down");
                animator.Play(lilithAnimations[!GameProgressionManagerInstance.currentLocation.Equals("lowerWard") ? 1 : 5].name, 0, 0f);
                break;
            case Vector2 v when v == Vector2.left:
                print("left");
                animator.Play(lilithAnimations[!GameProgressionManagerInstance.currentLocation.Equals("lowerWard") ? 2 : 6].name, 0, 0f);
                break;
            case Vector2 v when v == Vector2.right:
                print("right");
                animator.Play(lilithAnimations[!GameProgressionManagerInstance.currentLocation.Equals("lowerWard") ? 3 : 7].name, 0, 0f);
                break;
        }
    }

    void DetermineStopFrame()
    {
        animator.Play(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, 0, 0);
        animator.speed = 0;
    }

    // STEPS
    void TryStep()
    {
        Vector2 proposedPosition = (Vector2) transform.position + movementVector * stepSize;
                        
        if (!Physics2D.OverlapCircle(proposedPosition, checkRadius))
        {
            bumpDirection = "";
            targetPosition = proposedPosition;
            isMoving = true;
        }
        else
        {
            if (!bumpDirection.Equals(directionFacing))
            {
                audioSource.PlayOneShot(audioSource.clip);
                bumpDirection = directionFacing;
            }
            isMoving = false;
        }
    }

    void FinishStep()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if ((Vector2) transform.position == targetPosition) 
        {
            isMoving = false;
            overrideIsMoving = false;
        }
    }
}

