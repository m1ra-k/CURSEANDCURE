using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRun : MonoBehaviour
{
    private int frameNumber = 0;

    [SerializeField]
    private Vector2[] pathCoordinates;
    private int index;

    void Update()
    {
        if (frameNumber == 10)
        {
            Vector2 direction = Vector2.zero;

            if (transform.localPosition.x != pathCoordinates[index].x)
            {
                print("x was not the same");
                direction.x = pathCoordinates[index].x > transform.localPosition.x ? 1 : -1;
            }
            else if (transform.localPosition.y != pathCoordinates[index].y)
            {
                print("y was not the same");
                direction.y = pathCoordinates[index].y > transform.localPosition.y ? 1 : -1;
            }
    
            transform.localPosition = (Vector2) transform.localPosition + direction;

            if ((Vector2) transform.localPosition == pathCoordinates[index])
            {
                index = (index + 1) % 4;
            }

            frameNumber = 0;
        }

        frameNumber++;
    }
}
