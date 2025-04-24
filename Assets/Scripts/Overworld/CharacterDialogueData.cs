using System.Collections.Generic;
using UnityEngine;

public class CharacterDialogueData : MonoBehaviour
{
    public GameProgressionManager GameProgressionManagerInstance;

    [Header("[EVENT]")]
    public int patient = -1;
    public bool postHealingGame;
    public List<string> flags;
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
            (Vector2) gameObject.transform.localPosition + Vector2.up,
            (Vector2) gameObject.transform.localPosition + Vector2.down,
            (Vector2) gameObject.transform.localPosition + Vector2.left,
            (Vector2) gameObject.transform.localPosition + Vector2.right
        });

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
        // lilithPosition
        lilithPosition = (Vector2) lilith.transform.position;

        // flag check
        // if the current flag is true, update dialogue to be the next one possible
        if (characterDialoguesIndex < flags.Count && GameProgressionManagerInstance.progressionSystem.GetFlag(flags[characterDialoguesIndex]))
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

                // other dialogue after getting healed (usually). plays after thank you cutscene.
                if (postHealingGame)
                {
                    characterDialoguesIndex++;
                }

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
        foreach (var adjacentLocation in adjacentLocations)
        {
            if (Vector2.Distance(adjacentLocation, lilithPosition) <= 0.05f)
            {
                lilith.transform.position = adjacentLocation;
                return true;
            }
        }
        return false;
    }
}
