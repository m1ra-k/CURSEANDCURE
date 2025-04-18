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

    [Header("[Healing Game]")]
    private HealingGameManager healingGameManager;

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
        progressionSystem.Init();

        if (GameProgressionManagerInstance == null)
        {
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
                dialogueCanvas.SetActive(false);
                break;

            case "HealingGame":
                healingGameManager = FindObjectOfType<HealingGameManager>();
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
        if (Input.GetKeyDown(KeyCode.Y))
        {
            progressionSystem.SetFlag("again", true);
        }
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

        // not true always tho hmm
        if (possibleFlag.Equals("play"))
        {
            sceneNumber += 1;
            // sceneType = sceneProgressionLookup[sceneNumber][0];
        }
        else if (possibleFlag.Equals("lost"))
        {
            sceneType = "GameOver";
        }
        else if (possibleFlag.Equals("retry"))
        {
            fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "HealingGame");
            transitioning = true;
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
                transitioning = true;
                break;
                
            case "HealingGame":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "HealingGame");
                transitioning = true;
                break;

            case "GameOver":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "GameOver");
                transitioning = true;
                break;

            case "EndScreen":
                fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "EndScreen");
                transitioning = true;
                break;
        }
    }

    // BUTTONS - TODO MOVE
    public void PlayGame()
    {
        TransitionScene("play");
    } 
}
