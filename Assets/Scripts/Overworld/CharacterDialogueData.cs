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
    private GridMovement lilithGridMovement;

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite[] faces;
    [SerializeField]
    private bool turnsFace = true;

    // TODO DELETE JUST FOR DEBUG
    public bool canTalk;

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
        lilithGridMovement = lilith.GetComponent<GridMovement>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();
    }

    void Update()
    {
        canTalk = CanTalk();

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
                    GameProgressionManagerInstance.lilithPatientNumber++;
                    characterDialoguesIndex++;
                }

                postHealingGame = false;

                if (patient == GameProgressionManagerInstance.lilithPatientNumber)
                {
                    GameProgressionManagerInstance.TransitionScene("HealingGame");
                }
            }
            else
            {
                if (turnsFace)
                {
                    // TODO
                    spriteRenderer.sprite = faces[0];
                }
                GameProgressionManagerInstance.DialogueSystemManager.SetVisualNovelJSONFile(characterDialogues[characterDialoguesIndex]);
                GameProgressionManagerInstance.DialogueSystemManager.enabled = true;
                GameProgressionManagerInstance.dialogueCanvas.SetActive(true);    
            }
        }
    }

    private bool CanTalk()
    {
        if (!adjacentLocations.Contains(lilithPosition))
        {
            return false;
        }

        Vector2 offset = (Vector2) transform.localPosition - lilithPosition;
        float epsilon = 0.05f;

        switch (true)
        {
            case true when Mathf.Abs(offset.x) < epsilon && Mathf.Abs(offset.y - 1f) < epsilon:
                return GameProgressionManagerInstance.directionFacing.Equals("up");

            case true when Mathf.Abs(offset.x) < epsilon && Mathf.Abs(offset.y + 1f) < epsilon:
                return GameProgressionManagerInstance.directionFacing.Equals("down");

            case true when Mathf.Abs(offset.x - 1f) < epsilon && Mathf.Abs(offset.y) < epsilon:
                return GameProgressionManagerInstance.directionFacing.Equals("right");

            case true when Mathf.Abs(offset.x + 1f) < epsilon && Mathf.Abs(offset.y) < epsilon:
                return GameProgressionManagerInstance.directionFacing.Equals("left");
        }

        return false;
    }
}
