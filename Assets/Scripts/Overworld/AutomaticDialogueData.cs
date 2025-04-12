using System.Collections.Generic;
using UnityEngine;

public class AutomaticDialogueData : MonoBehaviour
{
    // TODO sort but later
    public GameProgressionManager GameProgressionManagerInstance;
    public bool repeated;
    private bool triggeredOnce;
    public List<TextAsset> characterDialogues;

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
    }

    void Update()
    {
        // lilithPosition
        lilithPosition = (Vector2) lilith.transform.position;
    }

    void LateUpdate()
    {
        if (CanTalk() && !GameProgressionManagerInstance.currentlyTalking && (!repeated || (repeated && !gridMovement.overrideIsMoving)))
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
                GameProgressionManagerInstance.DialogueSystemManager.SetVisualNovelJSONFile(characterDialogues[0]);
                // TODO: afia, now set dialogueCanvas to active and enable the DialogueSystemManager
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
