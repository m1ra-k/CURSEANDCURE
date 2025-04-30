using System;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDialogueData : MonoBehaviour
{
    // TODO sort but later
    public GameProgressionManager GameProgressionManagerInstance;

    // EVENT
    public List<Vector2> triggerPositions;
    public bool repeated;
    public bool active;
    public List<string> flags;
    public int dialoguesIndex;
    public List<TextAsset> dialogues;
    private bool triggeredOnce;

    private GameObject lilith;
    private GridMovement gridMovement;
    private Vector2 lilithPosition;

    [SerializeField]
    private List<string> triggeredFlags;
    [SerializeField]
    private int triggerMusicIndex;

    private Vector2 setPosition;


    private Dictionary<string, (Vector2, string)> pushBackDirection = new Dictionary<string, (Vector2, string)>
    {
        { "up", (Vector2.down, "down") },
        { "down", (Vector2.up, "up") },
        { "left", (Vector2.right, "right") },
        { "right", (Vector2.left, "left") }
    };

    void Awake()
    {
        // lilith
        lilith = GameObject.FindWithTag("Player");
        gridMovement = lilith.GetComponent<GridMovement>();

        triggerPositions.Add((Vector2) transform.localPosition);
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
        if (!active && GameProgressionManagerInstance.progressionSystem.GetFlag(flags[0])) // these will have: repeat - 2, !repeat - 1
        {
            // first flag being true is what makes it active
            active = true;
        }
        else if ((repeated && GameProgressionManagerInstance.progressionSystem.GetFlag(flags[flags.Count - 1])) || (!repeated && triggeredOnce))
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
            lilith.transform.position = setPosition;

            if (GameProgressionManagerInstance.DialogueSystemManager.delay)
            {
                triggeredOnce = true;
                GameProgressionManagerInstance.DialogueSystemManager.delay = false;
                if (repeated)
                {
                    PushBack();
                }
                else
                {
                    GameProgressionManagerInstance.complementedOneTimeEvents.Add(name);

                    foreach (var triggeredFlag in triggeredFlags)
                    {
                        GameProgressionManagerInstance.progressionSystem.SetFlag(triggeredFlag, true);
                    }
                }
            }
            else
            {
                GameProgressionManagerInstance.DialogueSystemManager.SetVisualNovelJSONFile(dialogues[0]);
                GameProgressionManagerInstance.DialogueSystemManager.enabled = true;
                GameProgressionManagerInstance.dialogueCanvas.SetActive(true);    
            }
        }
    }

    private bool CanTalk()
    {
        foreach (var triggerPosition in triggerPositions)
        {
            if (Vector2.Distance(GameProgressionManagerInstance.lilithPosition, triggerPosition) < 0.05f  && (repeated || !triggeredOnce))
            {
                setPosition = triggerPosition;
                return true;
            }
        }
        return false;
    }

    private void PushBack()
    {
        gridMovement.overrideIsMoving = true;
        gridMovement.movementVector = pushBackDirection[gridMovement.GameProgressionManagerInstance.directionFacing].Item1;
        GameProgressionManagerInstance.directionFacing = pushBackDirection[gridMovement.GameProgressionManagerInstance.directionFacing].Item2;
        gridMovement.DetermineAnimation();
    }
}
