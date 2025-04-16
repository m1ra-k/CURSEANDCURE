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
    private GameObject lilith;
    private Vector2 lilithPosition;

    // transition
    private bool currentlyDoorTransitioning;

    void Awake()
    {
        // lilith
        lilith = GameObject.FindWithTag("Player");
    }

    void Start()
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();
    }

    void Update()
    {
        // update position
        lilithPosition = (Vector2) lilith.transform.position;

        // door overlap check
        CheckDoor(doorA, offsetDoorB);
        CheckDoor(doorB, offsetDoorA);
    }

    private void CheckDoor(Vector2 door, Vector2 offsetDoor)
    {
        if (lilithPosition == door && !currentlyDoorTransitioning)
        {
            currentlyDoorTransitioning = true;
            StartCoroutine(DoorTransition(offsetDoor));
        }
    }

    private IEnumerator DoorTransition(Vector2 offsetDoor)
    {
        GameProgressionManagerInstance.fadeEffect.FadeIn(GameProgressionManagerInstance.blackTransition, 0.5f);
        yield return new WaitForSeconds(0.75f);
        lilith.transform.position = offsetDoor;
        GameProgressionManagerInstance.fadeEffect.FadeOut(GameProgressionManagerInstance.blackTransition, 0.5f);

        yield return new WaitForSeconds(0.5f);
        currentlyDoorTransitioning = false;
    }
}