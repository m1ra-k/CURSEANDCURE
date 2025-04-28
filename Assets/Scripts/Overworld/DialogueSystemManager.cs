using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueSystemManager : MonoBehaviour
{
    [Header("[Data]")]
    public GameProgressionManager GameProgressionManagerInstance;
    public SpriteCache spriteCache;

    [Header("[Images]")]
    public GameObject[] currentActiveSpeaker;
    public RectTransform[] currentActiveSpeakerRectTransforms;
    public GameObject[] oldActiveSpeaker;
    public GameObject currentActiveBG;
    public GameObject oldActiveBG;
    private GameObject currentActiveCG;
    private GameObject oldActiveCG;
    public GameObject normalBackground;
    private GameObject cgBackground;
    private GameObject cgStartTransition;
    
    [Header("[Dialogue Box]")]
    public GameObject normalDialogue;
    public RectTransform normalDialogueRectTransform;
    private float targetNormalDialogueWidth;
    public GameObject normalCharacterName;
    public GameObject normalIndicator;
    private GameObject cgDialogue;
    private GameObject cgCharacterName;
    public TextAsset visualNovelJSONFile;
    private List<DialogueStruct> dialogueList = new List<DialogueStruct>();

    [Header("[Voices]")]
    public List<AudioClip> voices;
    private AudioSource audioSource;
    private int characterNumber;

    [Header("[State]")]
    public bool transitioningScene;
    public bool finishedDialogue;
    public bool advanceDisabled;
    public bool delay;

    [Header("[IGNORE - Choice Boxes]")]
    public GameObject choiceBoxes;
    public bool choiceClicked = false;
    public int choiceMapping = -1;

    private DialogueStruct currentDialogue;
    private BaseDialogueStruct currentBaseDialogue;
    private Coroutine typeWriterCoroutine;
    private int dialogueIndex = -1;
    private int skippedFromIndex = -1;
    private int jumpToIndex = -1;
    private string dialogueOnDisplay;
    private bool typeWriterInEffect = false;

    private Dictionary<int, Vector2[]> NPCPositions = new()
    {
        { 1, new Vector2[] { new Vector2(0f, 0f) } },
        { 2, new Vector2[] { new Vector2(-225f, 0f), new Vector2(225f, 0f) } },
        { 3, new Vector2[] { new Vector2(0f, 0f), new Vector2(450f, 0f), new Vector2(-450f, 0f) } }
    };

    private Vector2[] targetPositions;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName.Equals("MainMenu"))
        {
            // GameProgressionManager.PlayMusic(1);
        }
        else if (sceneName.Equals("Overworld"))
        {
            // GameProgressionManager.PlayMusic(1);
        }
        else if (sceneName.Equals("HealingGame"))
        {
            // GameProgressionManager.PlayMusic(1);
        }
    }

    public void SetVisualNovelJSONFile(TextAsset characterDialogue)
    {
        visualNovelJSONFile = characterDialogue;
    }

    void OnEnable() 
    {
        LoadVNDialogueFromJSON();

        ProgressMainVNSequence(isStartDialogue: true);

        GameProgressionManagerInstance.currentlyTalking = true;
    }

    void Update()
    {
        // TODO: make sure to support saving on choice menu
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || choiceClicked) && !advanceDisabled)
        { 
            if (currentDialogue.endOfScene)
            {
                GameProgressionManagerInstance.currentlyTalking = false;
                finishedDialogue = false;
                dialogueIndex = -1;
                delay = true;
                enabled = false;

                if (GameProgressionManagerInstance.lilithPatientNumber != 3)
                {
                    StartCoroutine(Fade(currentActiveSpeaker[0], spriteCache.sprites["Transparent"], 0, 1));
                    GameProgressionManagerInstance.dialogueCanvas.SetActive(false);
                }
            }
            else if (!currentDialogue.endOfScene && !typeWriterInEffect && !finishedDialogue) 
            {
                ProgressMainVNSequence();
            }
            choiceClicked = false;
        }
        else if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) && typeWriterInEffect)
        {
            SkipTypeWriterEffect(inCG: currentDialogue.baseDialogue.vnType == VNTypeEnum.CG);
        }
    }

    public class DialogueStructContainer
    {
        public List<DialogueStruct> dialogues;
    }

    void LoadVNDialogueFromJSON()
    {
        string jsonWithRoot = visualNovelJSONFile.text;

        string processedJson = ProcessJSONForEnums(jsonWithRoot);

        DialogueStructContainer container = JsonUtility.FromJson<DialogueStructContainer>(processedJson);

        dialogueList = new List<DialogueStruct>(container.dialogues);

        foreach (var dialogue in dialogueList)
        {
            // foreach (var npcSprite in dialogue.baseDialogue.npcSprite)
            // {
            //     Debug.Log($"NPC Sprite: {npcSprite}");
            // }
            // Debug.Log($"VN Type: {dialogue.baseDialogue.vnType}");
            // Debug.Log($"Character: {dialogue.baseDialogue.character}");
            // Debug.Log($"NPC Sprite: {dialogue.baseDialogue.speakerSprite}");
            // Debug.Log($"MC Sprite: {dialogue.baseDialogue.mcSprite}");
            // Debug.Log($"CG Sprite: {dialogue.baseDialogue.cgSprite}");
            // Debug.Log($"Dialogue: {dialogue.baseDialogue.dialogue}");
            // Debug.Log($"Text Speed: {dialogue.baseDialogue.textSpeed}");

            // foreach (var choice in dialogue.conditionalChoices)
            // {
            //     Debug.Log($"Choice Mapping: {choice.choiceMapping}, Choice Dialogue: {choice.choiceDialogue}");
            // }

            // Debug.Log($"Jump To Index: {dialogue.jumpToIndex}");
            // Debug.Log("-----------------------");
        }
    }

    private string ProcessJSONForEnums(string json)
    {
        foreach (VNTypeEnum e in Enum.GetValues(typeof(VNTypeEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        foreach (CharacterEnum e in Enum.GetValues(typeof(CharacterEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        foreach (SpeakerSpriteEnum e in Enum.GetValues(typeof(SpeakerSpriteEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        foreach (BGSpriteEnum e in Enum.GetValues(typeof(BGSpriteEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        foreach (CGSpriteEnum e in Enum.GetValues(typeof(CGSpriteEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        return json;
    }

    void ProgressMainVNSequence(bool isStartDialogue = false, int skipToIndex = -1) 
    {
        StartCoroutine(DisableSpaceInput());
      
        if (skipToIndex != -1) 
        {
            skippedFromIndex = dialogueIndex;
            dialogueIndex = skipToIndex;
        }
        else 
        {
            dialogueIndex++;
        }
        currentDialogue = dialogueList[dialogueIndex];
        currentBaseDialogue = currentDialogue.baseDialogue;

        // set sprite
        SetSprite(currentBaseDialogue, dialogueIndex);
        
        // set dialogue
        SetDialogue(currentBaseDialogue);

        // set choices
        // if Choice available
        // would have to put this outside the loop to account for loading in to a scene
        switch (currentBaseDialogue.vnType) 
        {
            case VNTypeEnum.Choice:
            case VNTypeEnum.CGAndChoice:
                var temp = choiceBoxes.GetComponent<ChoiceMappingManager>();
                temp.SetChoiceMapping(this, currentDialogue.conditionalChoices);
                break;
        }

        if (currentDialogue.jumpToIndex != -1) 
        {
            jumpToIndex = currentDialogue.jumpToIndex;
        }

        if (dialogueIndex == dialogueList.Count - 1) 
        {
            finishedDialogue = true;
        }
    }
    
    void SetSprite(BaseDialogueStruct baseDialogue, int dialogueIndexToUse) 
    {
        bool atStart = dialogueIndexToUse == 0;

        if (baseDialogue.vnType != VNTypeEnum.CG && baseDialogue.vnType != VNTypeEnum.CGAndChoice) 
        {
            for (int i = 0; i < baseDialogue.speakerSprite.Count; i++)
            {
                Sprite oldActiveNPCSprite = currentActiveSpeaker[i].GetComponent<Image>().sprite;
                Sprite newActiveNPCSprite = spriteCache.sprites[baseDialogue.speakerSprite[0].ToString()];

                if (!oldActiveNPCSprite.ToString().Equals(newActiveNPCSprite.ToString())) 
                {
                    oldActiveSpeaker[i].GetComponent<Image>().sprite = currentActiveSpeaker[i].GetComponent<Image>().sprite;
                    StartCoroutine(Fade(currentActiveSpeaker[i], newActiveNPCSprite, 0, 1));
                    StartCoroutine(Fade(oldActiveSpeaker[i], oldActiveNPCSprite, 1, -1));
                }
            }

            targetPositions = NPCPositions.TryGetValue(baseDialogue.speakerSprite.Count, out var positions)
                                        ? positions
                                        : Array.Empty<Vector2>();

            for (int i = 0; i < currentActiveSpeaker.Length; i++)
            {
                bool npcIsActive = i < baseDialogue.speakerSprite.Count;

                if (currentActiveSpeaker[i].activeSelf != npcIsActive)
                {
                    oldActiveSpeaker[i].GetComponent<Image>().sprite = currentActiveSpeaker[i].GetComponent<Image>().sprite;
                    StartCoroutine(Fade(currentActiveSpeaker[i], spriteCache.sprites["Transparent"], 0, 1));
                    StartCoroutine(Fade(oldActiveSpeaker[i], oldActiveSpeaker[i].GetComponent<Image>().sprite, 1, -1));
                }

                if (npcIsActive && i < targetPositions.Length)
                {
                    currentActiveSpeakerRectTransforms[i].anchoredPosition = targetPositions[i];
                }
            }
            
            Sprite oldActiveBGSprite = currentActiveBG.GetComponent<Image>().sprite;
            Sprite newActiveBGSprite = spriteCache.sprites[baseDialogue.bgSprite.ToString()];

            if (!oldActiveBGSprite.ToString().Equals(newActiveBGSprite.ToString())) 
            {
                if (atStart)
                {
                    oldActiveBG.GetComponent<Image>().sprite = newActiveBGSprite;
                    currentActiveBG.GetComponent<Image>().sprite = newActiveBGSprite;
                }
                else
                {
                    oldActiveBG.GetComponent<Image>().sprite = currentActiveBG.GetComponent<Image>().sprite;
                    StartCoroutine(Fade(currentActiveBG, newActiveBGSprite, 0, 1));
                    StartCoroutine(Fade(oldActiveBG, oldActiveBGSprite, 1, -1));
                }
            }
        }

        skippedFromIndex = -1;
    }

    void SetDialogue(BaseDialogueStruct baseDialogue) 
    {
        dialogueOnDisplay = baseDialogue.dialogue;

        // set character name
        normalCharacterName.GetComponent<TextMeshProUGUI>().text = !baseDialogue.character.GetParsedName().Equals("TIP!") 
                                                                    ? baseDialogue.character.GetParsedName()
                                                                    : "";

        if (normalDialogueRectTransform.sizeDelta.x != targetNormalDialogueWidth)
        {
            normalDialogueRectTransform.sizeDelta = new Vector2(targetNormalDialogueWidth, normalDialogueRectTransform.sizeDelta.y);
        }

        // set dialogue
        typeWriterCoroutine = StartCoroutine(TypeWriterEffect(baseDialogue.character.ToString(), baseDialogue.dialogue));

        // set indicator, if needed
        normalIndicator.GetComponent<TextMeshProUGUI>().text = baseDialogue.character.GetParsedName().Equals("TIP!") 
                                                                ? baseDialogue.character.GetParsedName()
                                                                : ""; 
    }

    void SkipTypeWriterEffect(bool inCG = false) 
    {
        StopCoroutine(typeWriterCoroutine);
        TextMeshProUGUI normalDialogueTextMeshProUGUI = inCG ? cgDialogue.GetComponent<TextMeshProUGUI>() : normalDialogue.GetComponent<TextMeshProUGUI>();
        normalDialogueTextMeshProUGUI.text = dialogueOnDisplay;
        typeWriterInEffect = false;
    }

    float GetTextSpeed() 
    {
        if (currentBaseDialogue.textSpeed != 0f) 
        {
            return currentBaseDialogue.textSpeed; // modified | can also be different for different CharacterEnum
        }
        return 0.005f; // default
    }

    IEnumerator TypeWriterEffect(string character, string dialogue, bool inCG = false)
    {
        typeWriterInEffect = true;

        TextMeshProUGUI normalDialogueTextMeshProUGUI = inCG ? cgDialogue.GetComponent<TextMeshProUGUI>() : normalDialogue.GetComponent<TextMeshProUGUI>();
        
        for (int i = 0; i <= dialogue.Length; i++) 
        {
            normalDialogueTextMeshProUGUI.text = dialogue[..i];
            
            if (!dialogue.Equals("..."))
            {
                switch (character)
                {
                    case "Lilith":
                        characterNumber = 0;
                        audioSource.pitch = 1.275f;
                        break;
                    case "Ana":
                        characterNumber = 3;
                        audioSource.pitch = 1.85f;
                        break;
                    case "TavernKeeper":
                        characterNumber = 0;
                        audioSource.pitch = 1.20f;
                        break;
                    case "Man":
                        characterNumber = 2;
                        audioSource.pitch = 0.75f;
                        break;
                    case "Woman":
                        characterNumber = 1;
                        audioSource.pitch = 0.95f;
                        break;
                    case "Boy":
                        characterNumber = 1;
                        audioSource.pitch = 1.1f;
                        break;
                    case "Girl":
                        characterNumber = 3;
                        audioSource.pitch = 2f;
                        break;
                    case "System":
                        characterNumber = 2;
                        audioSource.pitch = 0.6f;
                        break;
                    default:
                        characterNumber = 0;
                        audioSource.pitch = 1.20f;
                        break;
                    // need to do this for everyone
                }

                audioSource.PlayOneShot(voices[characterNumber]);
            }

            yield return new WaitForSeconds(GetTextSpeed()); // make diff speeds
        }

        typeWriterInEffect = false;
    }
    
    // if sprite is null, indicates that it is an imageless graphic i.e. text only
    public IEnumerator Fade(GameObject currentCharacterDisplay, Sprite sprite, double fadeFrom, double fadeTo, float speed = 0.1f, bool isUIElement = false, bool getTextFromChild = false) 
    {
        double desiredOpacity = fadeTo;

        // image component
        Image characterDisplayImage = currentCharacterDisplay.GetComponent<Image>();
        Color imageColor = Color.red;
        if (characterDisplayImage != null) 
        {
            imageColor = characterDisplayImage.color;
            characterDisplayImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, imageColor.a);
            characterDisplayImage.sprite = sprite;
        }
        
        // text component
        TextMeshProUGUI characterDisplayText = currentCharacterDisplay.GetComponent<TextMeshProUGUI>();
        if (getTextFromChild) 
        {
            characterDisplayText = currentCharacterDisplay.GetComponentInChildren<TextMeshProUGUI>();
        }
        Color textColor = Color.red;
        if (characterDisplayText != null)
        {
            textColor = characterDisplayText.color;
        }

        Func<double, double, bool> lte = (a, b) => a <= b;
        Func<double, double, bool> gte = (a, b) => b <= a;

        double increment = speed;
        double until = desiredOpacity + increment;
        Func<double, double, bool> comparisonToUse = lte;

        // fading out (fading in = default)
        if (fadeFrom > fadeTo) {
            increment = -0.1f;
            until = desiredOpacity;
            comparisonToUse = gte;
        }

        // as UI elements can have varied opacity
        if (isUIElement) 
        {
            fadeFrom = imageColor.a;
        }

        for (double i = fadeFrom; comparisonToUse(i, until); i += increment) 
        {
            if (characterDisplayImage != null)
            {
                if ((isUIElement && i < increment / 2)|| !isUIElement)
                {
                    characterDisplayImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, (float) i);
                }
            }
            if (characterDisplayText != null) 
            {
                characterDisplayText.color = new Color(textColor.r, textColor.g, textColor.b, (float) i);
            }
            yield return null;
        }
    }

    public int GetDialogueIndex()
    {
        return dialogueIndex;
    }

    private IEnumerator DisableSpaceInput()
    {
        advanceDisabled = true;

        yield return new WaitForSeconds(0.60f); // TODO find the lower bound of this but for now, it works for VN glitches

        advanceDisabled = false;
    }
}
