using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRun : MonoBehaviour
{
    private Animator animator;

    private int frameNumber = 0;

    private Vector2 movementDirection;
    [SerializeField]
    private Vector2[] pathCoordinates;
    private int index;

    private Dictionary<Vector2, string> animationLookup = new Dictionary<Vector2, string>
    {
        { Vector2.up, "Male1NPCWalkUp" },
        { Vector2.down, "Male1NPCWalkDown" },
        { Vector2.left, "Male1NPCWalkLeft" },
        { Vector2.right, "Male1NPCWalkRight" }
    };
    private string currentAnimation = "Male1NPCWalkUp";
    private string possibleAnimation;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckSwitchAnimation();

        if (frameNumber == 10)
        {
            movementDirection = Vector2.zero;

            if (transform.localPosition.x != pathCoordinates[index].x)
            {
                movementDirection.x = pathCoordinates[index].x > transform.localPosition.x ? 1 : -1;
            }
            else if (transform.localPosition.y != pathCoordinates[index].y)
            {
                movementDirection.y = pathCoordinates[index].y > transform.localPosition.y ? 1 : -1;
            }
    
            transform.localPosition = (Vector2) transform.localPosition + movementDirection;

            if ((Vector2) transform.localPosition == pathCoordinates[index])
            {
                index = (index + 1) % 4;
            }

            frameNumber = 0;
        }

        frameNumber++;
    }

    private void CheckSwitchAnimation()
    {
        if (movementDirection != Vector2.zero)
        {
            possibleAnimation = animationLookup[movementDirection];

            if (!currentAnimation.Equals(possibleAnimation))
            {
                animator.Play(possibleAnimation);
                currentAnimation = possibleAnimation;
            }
        }
    }
}
