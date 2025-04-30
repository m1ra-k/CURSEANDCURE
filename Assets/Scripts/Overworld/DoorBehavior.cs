using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DoorBehavior : MonoBehaviour
{
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;

    [Header("[Doors]")]
    public Vector2 doorA;
    public Vector2 offsetDoorA;
    public Vector2 doorB;
    public Vector2 offsetDoorB;

    [Header("[Location]")]
    public string doorALocation;
    public string doorBLocation;

    // lilith
    private GridMovement lilithGridMovement;

    // audio
    private AudioSource parentAudioSource;

    void Start()
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        // lilith
        lilithGridMovement = GameProgressionManagerInstance.lilith.GetComponent<GridMovement>();

        // audio source
        parentAudioSource = transform.parent.GetComponent<AudioSource>();
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
        if (Vector2.Distance(GameProgressionManagerInstance.lilithPosition, door) < 0.05f && !lilithGridMovement.currentlyDoorTransitioning)
        {
            lilithGridMovement.currentlyDoorTransitioning = true;
            StartCoroutine(DoorTransition(offsetDoor == offsetDoorA ? doorBLocation : doorALocation, offsetDoor));
        }
    }

    private IEnumerator DoorTransition(string newLocation, Vector2 offsetDoor)
    {
        GameProgressionManagerInstance.currentLocation = newLocation;

        GameProgressionManagerInstance.fadeEffect.FadeIn(GameProgressionManagerInstance.blackTransition, 0.5f);
        
        GameProgressionManagerInstance.CheckMusicChange();

        parentAudioSource.PlayOneShot(parentAudioSource.clip);

        yield return new WaitForSeconds(0.75f);

        lilithGridMovement.DetermineAnimation();
        
        GameProgressionManagerInstance.lilith.transform.position = offsetDoor;
        GameProgressionManagerInstance.fadeEffect.FadeOut(GameProgressionManagerInstance.blackTransition, 0.5f);
        
        yield return new WaitForSeconds(0.5f);

        GameProgressionManagerInstance.CheckFlagsSet();
        
        lilithGridMovement.currentlyDoorTransitioning = false;
    }
}