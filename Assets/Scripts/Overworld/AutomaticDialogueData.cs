using System;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDialogueData : MonoBehaviour
{
    // TODO sort but later
    public GameProgressionManager GameProgressionManagerInstance;

    // EVENT
    public bool repeated;
    public bool active;
    public List<string> flagsToSet;
    public int dialoguesIndex;
    public List<TextAsset> dialogues;
    private bool triggeredOnce;

    private GameObject lilith;
    private GridMovement gridMovement;
    private Vector2 lilithPosition;
    
    private Dictionary<string, Vector2> pushBackDirection = new Dictionary<string, Vector2>
    {
        { "up", Vector2.down },
        { "down", Vector2.up },
        { "left", Vector2.right },
        { "right", Vector2.left }
    };

    void Awake()
    {
        // lilith
        lilith = GameObject.FindWithTag("Player");
        gridMovement = lilith.GetComponent<GridMovement>();
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
        if (!active && GameProgressionManagerInstance.progressionSystem.GetFlag(flagsToSet[0])) // these will have: repeat - 2, !repeat - 1
        {
            // first flag being true is what makes it active
            active = true;
        }

        if ((repeated && GameProgressionManagerInstance.progressionSystem.GetFlag(flagsToSet[flagsToSet.Count - 1])) || (!repeated && triggeredOnce))
        {
            // no longer need to repeat, so it is no longer active
            active = false;
            enabled = false;
        }
    }

    void LateUpdate()
    {
        if (active && CanTalk() && !GameProgressionManagerInstance.currentlyTalking && (!repeated || (repeated && !gridMovement.overrideIsMoving)))
        {
            lilith.transform.position = transform.position;

            if (GameProgressionManagerInstance.DialogueSystemManager.delay)
            {
                triggeredOnce = true;
                GameProgressionManagerInstance.DialogueSystemManager.delay = false;
                if (repeated)
                {
                    PushBack();
                }
            }
            else
            {
                // TODO: mira, hardcoded to 0 but needs to match GameProgressionManager progression value
                GameProgressionManagerInstance.DialogueSystemManager.SetVisualNovelJSONFile(dialogues[0]);
                GameProgressionManagerInstance.DialogueSystemManager.enabled = true;
                GameProgressionManagerInstance.dialogueCanvas.SetActive(true);    
            }
        }
    }

    private bool CanTalk()
    {
        return (Vector2) transform.position == lilithPosition && (repeated || !triggeredOnce);
    }

    private void PushBack()
    {
        gridMovement.overrideIsMoving = true;
        gridMovement.movementVector = pushBackDirection[gridMovement.directionFacing];
    }
}
