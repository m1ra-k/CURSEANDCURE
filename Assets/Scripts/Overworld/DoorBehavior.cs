using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;

    [Header("[Doors]")]
    public Vector2 doorA;
    public Vector2 offsetDoorA;
    public Vector2 doorB;
    public Vector2 offsetDoorB;
    
    // lilith
    private GridMovement lilithGridMovement;

    void Start()
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        // lilith
        lilithGridMovement = GameProgressionManagerInstance.lilith.GetComponent<GridMovement>();
    }

    void Update()
    {
        // update position
        GameProgressionManagerInstance.lilithPosition = (Vector2) GameProgressionManagerInstance.lilith.transform.position;

        // door overlap check
        CheckDoor(doorA, offsetDoorB);
        CheckDoor(doorB, offsetDoorA);
    }

    private void CheckDoor(Vector2 door, Vector2 offsetDoor)
    {
        if (GameProgressionManagerInstance.lilithPosition == door && !lilithGridMovement.currentlyDoorTransitioning)
        {
            lilithGridMovement.currentlyDoorTransitioning = true;
            StartCoroutine(DoorTransition(offsetDoor));
        }
    }

    private IEnumerator DoorTransition(Vector2 offsetDoor)
    {
        GameProgressionManagerInstance.fadeEffect.FadeIn(GameProgressionManagerInstance.blackTransition, 0.5f);
        yield return new WaitForSeconds(0.75f);
        GameProgressionManagerInstance.lilith.transform.position = offsetDoor;
        GameProgressionManagerInstance.fadeEffect.FadeOut(GameProgressionManagerInstance.blackTransition, 0.5f);

        yield return new WaitForSeconds(0.5f);
        lilithGridMovement.currentlyDoorTransitioning = false;
    }
}