using System.Collections.Generic;
using UnityEngine;

public class CharacterDialogueData : MonoBehaviour
{
    public GameProgressionManager GameProgressionManagerInstance;

    [Header("[EVENT]")]
    public int patient = -1;
    public bool postHealingGame;
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
        // lilithPosition
        lilithPosition = (Vector2) lilith.transform.position;

        // flag check
        // if the current flag is true, update dialogue to be the next one possible
        if (characterDialoguesIndex < flagsToSet.Count && GameProgressionManagerInstance.progressionSystem.GetFlag(flagsToSet[characterDialoguesIndex]))
        {
            characterDialoguesIndex++;
        }
    }

    void LateUpdate()
    {
        if (((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) && CanTalk() && !GameProgressionManagerInstance.currentlyTalking && !GameProgressionManagerInstance.transitioning) || postHealingGame)
        {
            if (GameProgressionManagerInstance.DialogueSystemManager.delay)
            {
                GameProgressionManagerInstance.DialogueSystemManager.delay = false;

                GameProgressionManagerInstance.lastTalkedNPC = gameObject.name;

                postHealingGame = false;

                if (patient == GameProgressionManagerInstance.lilithPatientNumber)
                {
                    GameProgressionManagerInstance.lilithPatientNumber++;
                    GameProgressionManagerInstance.TransitionScene("HealingGame");
                }
            }
            else
            {
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
