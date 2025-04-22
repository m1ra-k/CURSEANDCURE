using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;

    [Header("[Properties]")]
    public string directionFacing = "down";
    public Vector2 movementVector;
    public bool isMoving;
    public bool overrideIsMoving;
    public bool currentlyDoorTransitioning;

    private float moveSpeed = 5f;
    private float stepSize = 1f;
    private Vector2 targetPosition;

    private Animator animator;

    private Vector2 lastDir = Vector2.down;
    [SerializeField] float walkAnimSpeed = 0.5f;
    [SerializeField] float idleAnimSpeed = 1f;

    void Start() 
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        targetPosition = transform.position;

        animator = GetComponent<Animator>();
    }

    void Update() 
    {
        Move();
        UpdateAnimation();
        if (isMoving) {
            animator.speed = walkAnimSpeed;
        } 
        else 
        {
            animator.speed = idleAnimSpeed;
        }
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
                        targetPosition = (Vector2) transform.position + movementVector * stepSize;
                        isMoving = true;
                    }
                }
                else 
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                    if ((Vector2) transform.position == targetPosition) 
                    {
                        isMoving = false;
                    }
                }
            }
        }
        else
        {
            if (!isMoving)
            {
                targetPosition = (Vector2) transform.position + movementVector * stepSize;
                isMoving = true;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if ((Vector2) transform.position == targetPosition) 
                {
                    isMoving = false;
                    overrideIsMoving = false;
                }
            }
        }  
    }

    void UpdateAnimation()
    {
        animator.SetBool("IsWalking", isMoving);

        if (movementVector != Vector2.zero)
        {
            lastDir = movementVector;
            animator.SetFloat("Horizontal", movementVector.x);
            animator.SetFloat("Vertical",   movementVector.y);
        }
        else
        {
            animator.SetFloat("Horizontal", lastDir.x);
            animator.SetFloat("Vertical",   lastDir.y);
        }
    }
}

