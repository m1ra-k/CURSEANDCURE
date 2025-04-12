using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterDialogueData : MonoBehaviour
{
    public GameProgressionManager GameProgressionManagerInstance;

    // EVENT
    public List<string> flagsToSet;
    public int characterDialoguesIndex;
    public List<TextAsset> characterDialogues;
    
    private HashSet<Vector2> adjacentLocations = new();
    private GameObject lilith;
    private Vector2 lilithPosition;

    void Awake()
    { 
        // locations
        adjacentLocations.UnionWith(new List<Vector2>
        {
            (Vector2) gameObject.transform.position + Vector2.up,
            (Vector2) gameObject.transform.position + Vector2.down,
            (Vector2) gameObject.transform.position + Vector2.left,
            (Vector2) gameObject.transform.position + Vector2.right
        });

        // lilith
        lilith = GameObject.FindWithTag("Player");
    }

    void Start()
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();
    
        foreach (string flag in flagsToSet)
        {
            GameProgressionManagerInstance.progressionSystem.SetFlag(flag, false);
        }
    }

    void Update()
    {
        // if the current flag is true, update dialogue to be the next one possible
        if (characterDialoguesIndex < flagsToSet.Count && GameProgressionManagerInstance.progressionSystem.GetFlag(flagsToSet[characterDialoguesIndex]))
        {
            print("moving index");
            characterDialoguesIndex++;
        }

        // lilithPosition
        lilithPosition = (Vector2) lilith.transform.position;
    }

    void LateUpdate()
    {
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) && CanTalk() && !GameProgressionManagerInstance.currentlyTalking)
        {
            if (GameProgressionManagerInstance.DialogueSystemManager.delay)
            {
                GameProgressionManagerInstance.DialogueSystemManager.delay = false;
            }
            else
            {
                // TODO: mira, hardcoded to 0 but needs to match GameProgressionManager progression value
                GameProgressionManagerInstance.DialogueSystemManager.SetVisualNovelJSONFile(characterDialogues[characterDialoguesIndex]);
                GameProgressionManagerInstance.DialogueSystemManager.enabled = true;
                GameProgressionManagerInstance.dialogueCanvas.SetActive(true);    
            }
        }
    }

    private bool CanTalk()
    {
        return adjacentLocations.Contains(lilithPosition);
    }
}
