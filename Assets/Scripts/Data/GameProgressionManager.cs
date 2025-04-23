using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("[Start Screen]")]
    private Button playButton;
    
    [Header("[Overworld]")]
    public GameObject tutorial;
    public GameObject dialogueCanvas;
    public DialogueSystemManager DialogueSystemManager;
    public bool currentlyTalking;
    public bool finishedCurrentRound;
    public GameObject lilith;
    public Vector2 lilithPosition;
    public List<string> complementedOneTimeEvents;
    public string lastTalkedNPC;
    public bool healedPatient;
    public CharacterDialogueData patientCharacterDialogueData;

    [Header("[Healing Game]")]
    private HealingGameManager HealingGameManager;
    [Header("[Game Over]")]
    public Button retryButton;

    [Header("[End Screen]")]
    public bool fadedInTheEnd;

    [Header("[Music]")]
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSourceBGM;
    private int currentTrack;

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
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;

        blackTransition = GameObject.Find("Canvas").transform.Find("BlackTransition").gameObject;

        fadeEffect.FadeOut(blackTransition, 0.5f);

        transitioning = false;

        switch (currentScene)
        {
            case "StartScreen":
                playButton = GameObject.FindWithTag("Button").GetComponent<Button>();
                break;

            case "Overworld":
                dialogueCanvas = GameObject.FindWithTag("Dialogue");
                DialogueSystemManager = dialogueCanvas.GetComponentInChildren<DialogueSystemManager>();
                DialogueSystemManager.GameProgressionManagerInstance = this;
                dialogueCanvas.SetActive(false);
                tutorial = GameObject.FindWithTag("Tutorial");
                tutorial.SetActive(false);

                lilith = GameObject.FindWithTag("Player");
                lilith.transform.position = lilithPosition;

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

                break;

            case "HealingGame":
                tutorial = GameObject.FindWithTag("Tutorial");
                tutorial.SetActive(false);
                HealingGameManager = FindObjectOfType<HealingGameManager>();
                break;

            case "GameOver":
                StopMusic();
                retryButton = GameObject.FindWithTag("Button").GetComponent<Button>();
                break;
        }     
    }

    void Start()
    {

    }

    void Update()
    {
        // TODO GAME PROGRESSION SYSTEM this is just mock event completion but this approach works
        // just follow this format for actual events
        if (Input.GetKeyDown(KeyCode.U))
        {
            progressionSystem.SetFlag("goBack2", true);
            progressionSystem.SetFlag("wakeUp2", true);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            progressionSystem.SetFlag("goBackDONE", true);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            progressionSystem.SetFlag("goBack2DONE", true);
        }
        if(currentScene == "Overworld" || (currentScene == "HealingGame" && HealingGameManager.startedGame ))
        {
             if (Input.GetKeyDown(KeyCode.Escape))
                {
                    tutorial.SetActive(!tutorial.activeSelf);
                }
        }
    }

    public void StopMusic()
    {
        audioSourceBGM.Stop();
        currentTrack = -1;
    }

    public IEnumerator PlayMusic(int index, float waitTime = 0.5f, GameObject gameObjectToDeactivate = null, float cookingGameWaitTime = 0f)
    {
        float startVolume = audioSourceBGM.volume;

        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            audioSourceBGM.volume = Mathf.Lerp(startVolume, 0, t / 0.25f);
            yield return null;
        }

        audioSourceBGM.volume = 0;
        audioSourceBGM.Pause();

        yield return new WaitForSeconds(waitTime);

        if (gameObjectToDeactivate)
        {
            gameObjectToDeactivate.SetActive(false);
        }

        audioSourceBGM.volume = 1;
        audioSourceBGM.UnPause();

        yield return new WaitForSeconds(cookingGameWaitTime);

        if (index != currentTrack)
        {
            audioSourceBGM.clip = audioClips[index];
            audioSourceBGM.Play();

            currentTrack = index;
        }
    }

    // TODO: mira; this will need to be reworked a bit
    public void TransitionScene(string possibleFlag = "")
    {
        string sceneType = "";

        transitioning = true;

        // TODO - CONVERT TO SWITCH STATEMENT LOL
        if (possibleFlag.Equals("Play"))
        {
            print("flag was play");
            sceneType = "Overworld";
        }
        else if (possibleFlag.Equals("Won"))
        {
            print("flag was won");
            sceneType = "Overworld";

            healedPatient = true;
        }
        else if (possibleFlag.Equals("Lost"))
        {
            print("flag was lost");
            sceneType = "GameOver";
        }
        else if (possibleFlag.Equals("Retry"))
        {
            print("flag was retry");
            fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "HealingGame");
            return;
        }
        else
        {
            sceneType = possibleFlag;
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

            case "EndScreen":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "EndScreen");
                break;
        }
    }

    // BUTTONS - TODO MOVE
    public void PlayGame()
    {
        TransitionScene("Play");
    } 
}
