using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridMovement : MonoBehaviour
{
    // TODO: NEED TO SORT ALL
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;

    [Header("[Properties]")]
    public Vector2 lilithCurrentPosition;
    public Vector2 lilithNextPosition;
    public Vector2 movementVector;
    public Vector2 prevMovementVector;
    public Vector2 prevPrevMovementVector;
    public bool startStep;
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

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.Play("LilithWalkDown", 0, 0f);
        animator.speed = 0;
    }

    void Start() 
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        targetPosition = transform.position;

        audioSource = GetComponent<AudioSource>();
        
        if (GameProgressionManagerInstance.patientCharacterDialogueData != null && GameProgressionManagerInstance.patientCharacterDialogueData.postHealingGame)
        {
            DetermineAnimation();
        }
    }

    void Update() 
    {
        Move();

        if (movementVector != prevMovementVector)
        {
            completedFirstMovement = true;
            animator.speed = 1;
            DetermineAnimation();
        }

        if (movementVector == Vector2.zero && completedFirstMovement)
        {
            DetermineStopFrame();
        }

        prevPrevMovementVector = prevMovementVector;
        prevMovementVector = movementVector;
    }

    void Move() 
    {
        if (!overrideIsMoving)
        {
            if (GameProgressionManagerInstance.currentlyTalking || currentlyDoorTransitioning || GameProgressionManagerInstance.transitioning || GameProgressionManagerInstance.tutorial.activeSelf)
            {
                startStep = false;
                movementVector = Vector2.zero;
            }
            else
            {
                if (!startStep)
                {
                    lilithCurrentPosition = transform.localPosition;
                    if (Input.GetKey(KeyCode.UpArrow)) 
                    {
                        movementVector = Vector2.up;
                        GameProgressionManagerInstance.directionFacing = "up";
                    }
                    else if (Input.GetKey(KeyCode.DownArrow)) 
                    {
                        movementVector = Vector2.down;
                        GameProgressionManagerInstance.directionFacing = "down";
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow)) 
                    {
                        movementVector = Vector2.left;
                        GameProgressionManagerInstance.directionFacing = "left";
                    }
                    else if (Input.GetKey(KeyCode.RightArrow)) 
                    {
                        movementVector = Vector2.right;
                        GameProgressionManagerInstance.directionFacing = "right";
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
            if (!startStep)
            {   
                TryStep();
            }
            else
            {
                FinishStep();
            }
        }

        if (!startStep)
        {
            lilithCurrentPosition = lilithNextPosition;
        }
    }

    // ANIMATIONS
    public void DetermineAnimation()
    {
        switch (GameProgressionManagerInstance.directionFacing)
        {
            case "up":
                animator.Play(lilithAnimations[!GameProgressionManagerInstance.currentLocation.Equals("lowerWard") ? 0 : 4].name, 0, 0f);
                break;
            case "down":
                animator.Play(lilithAnimations[!GameProgressionManagerInstance.currentLocation.Equals("lowerWard") ? 1 : 5].name, 0, 0f);
                break;
            case "left":
                animator.Play(lilithAnimations[!GameProgressionManagerInstance.currentLocation.Equals("lowerWard") ? 2 : 6].name, 0, 0f);
                break;
            case "right":
                animator.Play(lilithAnimations[!GameProgressionManagerInstance.currentLocation.Equals("lowerWard") ? 3 : 7].name, 0, 0f);
                break;
        }
    }

    public void DetermineStopFrame()
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
            lilithNextPosition = targetPosition;
            startStep = true;
        }
        else
        {
            if (!bumpDirection.Equals(GameProgressionManagerInstance.directionFacing))
            {
                audioSource.PlayOneShot(audioSource.clip);
                bumpDirection = GameProgressionManagerInstance.directionFacing;
            }
            startStep = false;
        }
    }

    void FinishStep()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        print($"curr was: {lilithCurrentPosition} and target is {lilithNextPosition}");


        if ((Vector2) transform.position == targetPosition) 
        {
            print("WAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            lilithCurrentPosition = targetPosition;
            print($"curr: {lilithCurrentPosition}");
            startStep = false;
            overrideIsMoving = false;
        }
    }
}

