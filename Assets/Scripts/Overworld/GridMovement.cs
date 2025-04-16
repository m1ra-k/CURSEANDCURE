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

    void Start() 
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        targetPosition = transform.position;
    }

    void Update() 
    {
        Move();
    }

    void Move() 
    {
        if (!overrideIsMoving)
        {
            if (GameProgressionManagerInstance.currentlyTalking)
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
            if (!currentlyDoorTransitioning)
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
    }
}

