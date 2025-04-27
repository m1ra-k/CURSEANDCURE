using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterDialogueData : MonoBehaviour
{
    public GameProgressionManager GameProgressionManagerInstance;

    [Header("[EVENT]")]
    public int patient = -1;
    public bool postHealingGame;
    public List<string> flags;
    public int characterDialoguesIndex;
    public List<TextAsset> characterDialogues;
    
    private GameObject lilith;
    private Vector2 lilithPosition;

    public SpriteRenderer spriteRenderer;
    private Sprite originalFace;
    public Sprite[] faces;
    private int talkingDirection;
    [SerializeField]
    private bool turnsFace = true;

    // TODO DELETE JUST FOR DEBUG
    public bool canTalk;

    void Awake()
    { 
        // lilith
        lilith = GameObject.FindWithTag("Player");

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        originalFace = spriteRenderer.sprite;
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
        if (((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) && CanTalk() && !GameProgressionManagerInstance.currentlyTalking && !GameProgressionManagerInstance.transitioning && !GameProgressionManagerInstance.tutorial.activeSelf) || postHealingGame)
        {
            if (GameProgressionManagerInstance.DialogueSystemManager.delay)
            {
                GameProgressionManagerInstance.DialogueSystemManager.delay = false;

                GameProgressionManagerInstance.lastTalkedNPC = gameObject.name;

                // other dialogue after getting healed (usually). plays after thank you cutscene.
                if (postHealingGame)
                {
                    characterDialoguesIndex++;
                    if (GameProgressionManagerInstance.lilithPatientNumber == 3)
                    {
                        GameProgressionManagerInstance.TransitionScene("EndMenu");
                    }
                }

                postHealingGame = false;

                if (patient == GameProgressionManagerInstance.lilithPatientNumber)
                {
                    GameProgressionManagerInstance.TransitionScene("HealingGame");
                }
                else if (turnsFace) 
                {
                    spriteRenderer.sprite = originalFace;
                }
            }
            else
            {
                if (turnsFace)
                {
                    spriteRenderer.sprite = faces[talkingDirection];
                }

                GameProgressionManagerInstance.DialogueSystemManager.SetVisualNovelJSONFile(characterDialogues[characterDialoguesIndex]);
                GameProgressionManagerInstance.DialogueSystemManager.enabled = true;
                GameProgressionManagerInstance.dialogueCanvas.SetActive(true);    
            }
        }
    }

    private bool CanTalk()
    {
        Vector2 offset = (Vector2) transform.localPosition - lilithPosition;
        float epsilon = 0.05f;

        switch (true)
        {
            case true when Mathf.Abs(offset.x) < epsilon && Mathf.Abs(offset.y - 1f) < epsilon:
                talkingDirection = 1;
                return GameProgressionManagerInstance.directionFacing.Equals("up");

            case true when Mathf.Abs(offset.x) < epsilon && Mathf.Abs(offset.y + 1f) < epsilon:
                talkingDirection = 0;
                return GameProgressionManagerInstance.directionFacing.Equals("down");

            case true when Mathf.Abs(offset.x + 1f) < epsilon && Mathf.Abs(offset.y) < epsilon:
                talkingDirection = 3;
                return GameProgressionManagerInstance.directionFacing.Equals("left");

            case true when Mathf.Abs(offset.x - 1f) < epsilon && Mathf.Abs(offset.y) < epsilon:
                talkingDirection = 2;
                return GameProgressionManagerInstance.directionFacing.Equals("right");
        }

        return false;
    }
}
