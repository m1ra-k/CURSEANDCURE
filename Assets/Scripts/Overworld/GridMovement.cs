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

    [SerializeField] private float checkRadius = 0.1f;
    public LayerMask obstacleLayer;
    public Vector2 boxSize = new Vector2(0.8f, 0.8f);

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
                        Vector2 target = (Vector2)transform.position + movementVector * stepSize;
                        
                        bool blocked = Physics2D.BoxCast(
                            origin: transform.position,
                            size: boxSize,
                            angle: 0f,
                            direction: movementVector,
                            distance: stepSize,
                            layerMask: obstacleLayer
                        );

                        if (!blocked)
                        {
                            targetPosition = target;
                            isMoving = true;
                        }
                    }
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                    if ((Vector2)transform.position == targetPosition)
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
                Vector2 pos = transform.position;

                bool xNeedsSnap = pos.x % 1f != 0.5f;
                bool yNeedsSnap = pos.y % 1f != 0.5f;

                if (xNeedsSnap || yNeedsSnap)
                {
                    print("needed snap, so i snapped");
                    GridSnap();
                }


        Debug.Log("m");
        
                Vector2 target = (Vector2)transform.position + movementVector * stepSize;

                bool blocked = Physics2D.BoxCast(
                    origin: transform.position,
                    size: boxSize,
                    angle: 0f,
                    direction: movementVector,
                    distance: stepSize,
                    layerMask: obstacleLayer
                );

                if (!blocked)
                {
                    targetPosition = target;
                    isMoving = true;
                }
            }
            else
            {
        
                print("hi");
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if ((Vector2)transform.position == targetPosition)
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

    void GridSnap()
    {
        Vector2 pos = transform.position;
        
        // get nearest 0.5 for both x and y
        float snappedX = Mathf.Round(pos.x * 2f) / 2f;
        float snappedY = Mathf.Round(pos.y * 2f) / 2f;
        
        // truncate to 1 decimal place
        snappedX = Mathf.Round(snappedX * 10f) / 10f;
        snappedY = Mathf.Round(snappedY * 10f) / 10f;

        print($"need to snap to: {snappedX}, {snappedY}");
        
        transform.position = new Vector2(snappedX, snappedY);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        Debug.Log($"{gameObject.name} col with {col.collider.name}");
        if (col.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Debug.Log("i need to snap!");
            isMoving = false;
            GridSnap();
        }
    }
}

