using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stepSize = 0.9f;
    private Vector2 targetPosition;
    private bool isMoving = false;
    void Start() 
    {
        targetPosition = transform.position;
    }

    void Move() 
    {
        if (!isMoving) 
        {
            Vector2 movementVector = Vector2.zero;
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
            if (movementVector != Vector2.zero) 
            {
                targetPosition = (Vector2)transform.position + movementVector * stepSize;
                isMoving = true;
            }
        }
        if (isMoving) 
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if ((Vector2)transform.position == targetPosition) 
            {
                isMoving = false;
            }
        }
    }

    void Update() 
    {
        Move();
    }
}

