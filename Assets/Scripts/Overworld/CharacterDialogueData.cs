using System.Collections.Generic;
using UnityEngine;

public class CharacterDialogueData : MonoBehaviour
{
    public GameProgressionManager GameProgressionManager;
    public List<TextAsset> characterDialogues;
    private HashSet<Vector2> adjacentLocations = new();
    private GameObject lilith;
    private Vector2 lilithPosition;
    private GameObject dialogueCanvas;

    void Awake()
    { 
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();
        
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

    void Update()
    {
        // lilithPosition
        lilithPosition = (Vector2) lilith.transform.position;
    }

    void LateUpdate()
    {
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) && CanTalk() && !GameProgressionManager.currentlyTalking)
        {
            if (GameProgressionManager.DialogueSystemManager.delay)
            {
                GameProgressionManager.DialogueSystemManager.delay = false;
            }
            else
            {
                // TODO: mira, hardcoded to 0 but needs to match GameProgressionManager progression value
                GameProgressionManager.DialogueSystemManager.SetVisualNovelJSONFile(characterDialogues[0]);
                // TODO: afia, now set dialogueCanvas to active and enable the DialogueSystemManager
                GameProgressionManager.DialogueSystemManager.enabled=true;
                GameProgressionManager.dialogueCanvas.SetActive(true);    
            }
        }
    }

    private bool CanTalk()
    {
        return adjacentLocations.Contains(lilithPosition);
    }
}
