using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class GameProgressionManager : MonoBehaviour
{
    [Header("[State]")]
    public static GameProgressionManager GameProgressionManagerInstance;
    public ProgressionSystem progressionSystem;
    public string currentScene;
    public int sceneNumber;
    public int eventNumber;
    public bool transitioning;
    public string previousScene;
    public int lilithPatientNumber = 0;
    
    [Header("[Overworld]")]
    public GameObject lilith;
    public Vector2 lilithPosition;
    public GridMovement lilithGridMovement;
    public string directionFacing = "down";

    public DialogueSystemManager DialogueSystemManager;
    public GameObject dialogueCanvas;
    public HashSet<string> complementedOneTimeEvents = new();
    public bool currentlyTalking;
    public string lastTalkedNPC;

    public CharacterDialogueData patientCharacterDialogueData;
    public bool finishedCurrentRound;
    public bool healedPatient;

    public string currentLocation;

    private GameObject[] eveningObjects;
    private GameObject[] nightObjects;
    private GameObject[] lateNightObjects;
    // SUPER HARDCODED TODO REMOVE
    public GameObject noOneHere;
    private GameObject ana;

    public GameObject tutorial;
    private GameObject[] tutorialTexts;
    private bool unlockedTutorial;

    [Header("[Healing Game]")]
    private HealingGameManager HealingGameManager;
    
    [Header("[Game Over]")]
    public Button retryButton;

    [Header("[End Screen]")]
    public bool fadedInTheEnd;

    [Header("[Music]")]
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSourceBGM;
    public int currentTrack = 1;

    [Header("[References]")]  
    public FadeEffect fadeEffect;
    public GameObject blackTransition;

    void Awake()
    {                
        if (GameProgressionManagerInstance == null)
        {
            progressionSystem.Init();

            Application.targetFrameRate = 60;
            GameProgressionManagerInstance = this;
            DontDestroyOnLoad(gameObject);

            fadeEffect = GetComponent<FadeEffect>();
        }
        else
        {            
            Destroy(gameObject);
            return;
        }

        audioSourceBGM = GetComponent<AudioSource>();
        // currentTrack = -1;

        SceneManager.sceneLoaded += OnSceneLoaded;

        // DEBUG ONLY
        // progressionSystem.SetFlag("firstHealed", true);
        // progressionSystem.SetFlag("secondHealed", true);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        
        blackTransition = GameObject.Find("Canvas").transform.Find("BlackTransition").gameObject;

        fadeEffect.FadeOut(blackTransition, 0.5f);

        transitioning = false;

        switch (currentScene)
        {
            case "Overworld":
                dialogueCanvas = GameObject.FindWithTag("Dialogue");
                DialogueSystemManager = dialogueCanvas.GetComponentInChildren<DialogueSystemManager>();
                DialogueSystemManager.GameProgressionManagerInstance = this;
                dialogueCanvas.SetActive(false);
                
                lilith = GameObject.FindWithTag("Player");
                lilith.transform.position = lilithPosition;
                lilithGridMovement = lilith.GetComponent<GridMovement>();
                
                foreach (var complementedOneTimeEvent in complementedOneTimeEvents)
                {
                    GameObject.Find(complementedOneTimeEvent).SetActive(false);
                }

                if (healedPatient)
                {
                    patientCharacterDialogueData = GameObject.Find(lastTalkedNPC).GetComponent<CharacterDialogueData>();
                    patientCharacterDialogueData.characterDialoguesIndex++;
                    patientCharacterDialogueData.postHealingGame = true;
                }

                eveningObjects = GameObject.FindGameObjectsWithTag("Evening");
                nightObjects = GameObject.FindGameObjectsWithTag("Night");
                lateNightObjects = GameObject.FindGameObjectsWithTag("LateNight");
                ana = GameObject.FindWithTag("Midnight");

                foreach (GameObject obj in lilithPatientNumber < 2 ? nightObjects : eveningObjects) obj.SetActive(false);

                foreach (GameObject obj in lateNightObjects) obj.SetActive(progressionSystem.GetFlag("HildaNightTavern"));

                ana.SetActive(lilithPatientNumber == 3);

                tutorial = GameObject.FindWithTag("Tutorial");
                tutorialTexts = GameObject.FindGameObjectsWithTag("TutorialText");
                tutorialTexts = tutorialTexts.OrderBy(go => go.name).ToArray();

                for (int i = 0; i < tutorialTexts.Length; i++)
                {
                    if (i < lilithPatientNumber)
                    {
                        TextMeshProUGUI tmp = tutorialTexts[i].GetComponent<TextMeshProUGUI>();
                        tmp.text = $"<s>{tmp.text}</s>";
                    }
                }
                tutorial.SetActive(false);

                if (lilithPatientNumber == 3) StartCoroutine(GameProgressionManagerInstance.PlayMusic(-1, fadeSpeed: 1));

                CheckFlagsSet();

                break;

            case "HealingGame":
                if (currentTrack == -1) StartCoroutine(PlayMusic(1, pitch: 0.875f));

                tutorial = GameObject.FindWithTag("Tutorial");

                tutorial.SetActive(false);

                HealingGameManager = FindObjectOfType<HealingGameManager>();

                // TODO CLEAN
                if (lilithPatientNumber == 0)
                {
                    tutorial.SetActive(true);
                    HealingGameManager.resultText.gameObject.SetActive(false);
                }

                break;

            case "GameOver":
                StopMusic();
                retryButton = GameObject.FindWithTag("Button").GetComponent<Button>();

                break;
        }     
    }

    void Update()
    {
        unlockedTutorial = unlockedTutorial || complementedOneTimeEvents.Contains("HeyHilda");

        if (unlockedTutorial)
        {
            if (((currentScene == "Overworld" && !currentlyTalking && !lilithGridMovement.startStep && !lilithGridMovement.currentlyDoorTransitioning) || (currentScene == "HealingGame" && !HealingGameManager.startedGame)) && Input.GetKeyDown(KeyCode.Escape))
            {
                tutorial.SetActive(!tutorial.activeSelf);

                if (currentScene.Equals("HealingGame"))
                {
                    HealingGameManager.resultText.gameObject.SetActive(!HealingGameManager.resultText.gameObject.activeSelf);
                }
            }
        }
        
    }

    public void StopMusic()
    {
        audioSourceBGM.Stop();
        currentTrack = -1;
    }

    public void CheckMusicChange()
    {
        if (lilithPatientNumber > 1 && currentTrack != 2)
        {
            StartCoroutine(PlayMusic(2));
        }
    }

    public void CheckFlagsSet()
    {
        // TODO FUTURE IMPLEMTATION, SUPER HARDCODED
        if (progressionSystem.GetFlag("HildaNightTavern"))
        {
            foreach (GameObject obj in lateNightObjects) obj.SetActive(true);
            // SUPER HARDCODED
            GameObject.Find("NoOneHere")?.SetActive(false);
        }   
    }

    public IEnumerator PlayMusic(int index, float waitTime = 0.5f, GameObject gameObjectToDeactivate = null, float gameWaitTime = 0f, float pitch = 0.95f, float fadeSpeed = 0.25f)
    {
        float startVolume = audioSourceBGM.volume;

        for (float t = 0; t < fadeSpeed; t += Time.deltaTime)
        {
            audioSourceBGM.volume = Mathf.Lerp(startVolume, 0, t / fadeSpeed);
            yield return null;
        }

        audioSourceBGM.volume = 0;
        audioSourceBGM.Pause();

        yield return new WaitForSeconds(waitTime);

        if (gameObjectToDeactivate)
        {
            gameObjectToDeactivate.SetActive(false);
        }

        if (index != -1)
        {
            audioSourceBGM.volume = 0.6f;
            audioSourceBGM.UnPause();

            audioSourceBGM.pitch = pitch;

            yield return new WaitForSeconds(gameWaitTime);

            if (index != currentTrack)
            {
                audioSourceBGM.clip = audioClips[index];
                audioSourceBGM.Play();

                currentTrack = index;
            }
        }   
    }

    // TODO: mira; this will need to be reworked a bit
    public void TransitionScene(string possibleFlag = "")
    {
        string sceneType = "";

        transitioning = true;

        switch (possibleFlag)
        {
            case "Won":
                sceneType = "Overworld";
                healedPatient = true;
                break;

            case "Retry":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "HealingGame");
                break;

            default:
                sceneType = possibleFlag;
                break;
        }
        
        switch (sceneType)
        {
            case "Overworld":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "Overworld");
                break;
                
            case "HealingGame":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "HealingGame");
                break;

            case "GameOver":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "GameOver");
                break;

            case "EndMenu":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "EndMenu");
                break;
        }
    }
}
