using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;

    private Vector2 movementVector;
    private float moveSpeed = 5f;
    private float stepSize = 1f;
    private Vector2 targetPosition;
    private bool isMoving = false;

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
        if (!isMoving && !GameProgressionManagerInstance.currentlyTalking)
        {
            if (Input.GetKey(KeyCode.UpArrow)) 
            {
                movementVector = Vector2.up;
            }
            else if (Input.GetKey(KeyCode.DownArrow)) 
            {
                movementVector = Vector2.down;
            }
            else if (Input.GetKey(KeyCode.LeftArrow)) 
            {
                movementVector = Vector2.left;
            }
            else if (Input.GetKey(KeyCode.RightArrow)) 
            {
                movementVector = Vector2.right;
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
        if (isMoving) 
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if ((Vector2) transform.position == targetPosition) 
            {
                isMoving = false;
            }
        }
    }
}

