using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealingGameManager : MonoBehaviour
{
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;
    // just lilith for now
    public string mode = "lilith";
    private int round;
    public bool startedGame;
    public bool finishedGame;

    [Header("[Gauge]")]
    [SerializeField]    
    private Slider healingGauge;
    private float healingGaugeValue = 0f;
    private float healingSpeed = 0.5f;

    [Header("[Range]")]
    // TODO eventually take off serializefield, just for debug
    [SerializeField]    
    private HealingRangeList healingRangeList;
    [SerializeField]    
    private RectTransform rangeRectTransform;

    [Header("[UI]")]
    private GameObject[] healingGameUI;
    [SerializeField]
    private AnimationClip[] lilithHealingAnimations;
    // REMOVE
    [SerializeField]
    private Sprite[] lilithHealingAnimationStills;

    private Coroutine flashingStartTextCoroutine;
    public int score;
    private string cachedScore = "";
    [SerializeField]
    private TMP_FontAsset retroFont;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timeText;
    private int frameCount;
    private int time = 30;
    private float shakeDuration = 0.5f;
    private GameObject minigameArt;
    private Sprite minigameArtSprite;
    private SpriteRenderer minigameArtSpriteRenderer;
    private Vector3 minigameArtPositionInitial;
    private int[] shakeAmounts = { 30, -60, 60, -60, 60 };
    private float flashInterval = 0.65f;
    private float flashDuration = 0.2f;
    private float timeSinceLastFlash = 0.65f;
    private float redTimer = 0f;
    private bool isRed = false;

    void Start()
    {
        // TODO REMOVE THIS IS JUST FOR DEBUG
        StartCoroutine(DisplayWon());

        // state
        GameProgressionManagerInstance = FindObjectOfType<GameProgressionManager>();

        // range
        healingRangeList = JsonUtility.FromJson<HealingRangeList>(Resources.Load<TextAsset>($"Patients/{mode}_patient_" + GameProgressionManagerInstance.lilithPatientNumber).text);

        // UI
        flashingStartTextCoroutine = StartCoroutine(FlashingStartText());

        minigameArt = GameObject.FindWithTag("Image");
        minigameArtSpriteRenderer = minigameArt.GetComponent<SpriteRenderer>();
        minigameArtSprite = minigameArtSpriteRenderer.sprite;
        minigameArtPositionInitial = minigameArt.transform.position;

        healingGameUI = GameObject.FindGameObjectsWithTag("UI");
        foreach (var hGUI in healingGameUI) hGUI.SetActive(false);
    }

    void Update()
    {
        if (startedGame && !finishedGame)
        {
            AdjustHealingGauge();
            
            if (frameCount == 60)
            {
                time--;
                timeText.text = $"00:{time:D2}";
                frameCount = 0;
            }
            frameCount++;
        }

        if (!startedGame && !GameProgressionManagerInstance.tutorial.activeSelf && (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Return)))
        {
            StopCoroutine(flashingStartTextCoroutine);
            resultText.text = "";
            StartCoroutine(AdjustForAllHealingRange());
            startedGame = true;
            minigameArtSpriteRenderer.sprite = lilithHealingAnimationStills[1];

            foreach (var hGUI in healingGameUI) hGUI.SetActive(true);
        }

        if ($"SCORE: {score}" != cachedScore && score <= 100)
        {
            scoreText.text = $"SCORE: {score}";
            cachedScore = $"{score}";
        }
    }

    private IEnumerator FlashingStartText()
    {
        resultText.text = "START?";

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (resultText.text == "START?")
            {
                resultText.text = "";
            }
            else
            {
                resultText.text = "START?";
            }
        }
    }

    private void AdjustHealingGauge()
    {
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Return))
        {
            healingGaugeValue += healingSpeed * Time.deltaTime;
        }
        else
        {
            healingGaugeValue -= healingSpeed/2 * Time.deltaTime;
        }

        healingGaugeValue = Mathf.Clamp01(healingGaugeValue);

        healingGauge.value = healingGaugeValue;
    }

    private IEnumerator AdjustForAllHealingRange()
    {
        while (true)
        {
            if (round == 3)
            {
                finishedGame = true;

                timeText.text = "00:00";

                StartCoroutine(score < 80 ? DisplayLost() : DisplayWon());

                break;
            }

            AdjustHealingRange();
            
            yield return new WaitForSeconds(10f);
        }
    }

    private void AdjustHealingRange()
    {
        StartCoroutine(SmoothAdjustHealingRange());
    }

    private IEnumerator SmoothAdjustHealingRange()
    {
        Vector2 startPos = rangeRectTransform.anchoredPosition;
        float startY = startPos.y;

        float startHeight = rangeRectTransform.sizeDelta.y;
        float currentWidth = rangeRectTransform.sizeDelta.x;

        BoxCollider2D boxCollider = rangeRectTransform.GetComponent<BoxCollider2D>();
        float startColliderHeight = boxCollider.size.y;

        float targetY = healingRangeList.healingRanges[round].yPosition;
        float targetHeight = healingRangeList.healingRanges[round].height;

        float duration = 0.5f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            rangeRectTransform.anchoredPosition = new Vector2(startPos.x, Mathf.Lerp(startY, targetY, t));

            rangeRectTransform.sizeDelta = new Vector2(currentWidth, Mathf.Lerp(startHeight, targetHeight, t));

            Vector2 colliderSize = boxCollider.size;
            colliderSize.y = Mathf.Lerp(startColliderHeight, targetHeight, t);
            boxCollider.size = colliderSize;

            boxCollider.offset = new Vector2(boxCollider.offset.x, 0f);

            yield return null;
        }

        rangeRectTransform.anchoredPosition = new Vector2(startPos.x, targetY);
        rangeRectTransform.sizeDelta = new Vector2(currentWidth, targetHeight);
        boxCollider.size = new Vector2(boxCollider.size.x, targetHeight);

        round++;
    }

    private IEnumerator DisplayLost()
    {
        resultText.text = "FAIL!";

        Color originalColor = minigameArtSpriteRenderer.color;

        foreach (int shakeAmount in shakeAmounts)
        {
            float elapsedTime = 0f;
            while (elapsedTime < shakeDuration)
            {
                float shakeOffset = Mathf.Sin(elapsedTime * Mathf.PI * 2) * shakeAmount / 50;
                minigameArt.transform.position = minigameArtPositionInitial + new Vector3(shakeOffset, 0f, 0f);

                elapsedTime += Time.deltaTime;
                timeSinceLastFlash += Time.deltaTime;

                if (!isRed && timeSinceLastFlash >= flashInterval)
                {
                    minigameArtSpriteRenderer.color = Color.red;
                    isRed = true;
                    redTimer = 0f;
                    timeSinceLastFlash = 0f;
                }

                if (isRed)
                {
                    redTimer += Time.deltaTime;
                    if (redTimer >= flashDuration)
                    {
                        minigameArtSpriteRenderer.color = originalColor;
                        isRed = false;
                    }
                }

                yield return null;
            }
        }

        minigameArt.transform.position = minigameArtPositionInitial;
        minigameArtSpriteRenderer.color = originalColor;

        GameProgressionManagerInstance.TransitionScene("GameOver");
    }

    private IEnumerator DisplayWon()
    {
        resultText.text = "GOOD!";

        yield return new WaitForSeconds(2.5f);

        switch (GameProgressionManagerInstance.lilithPatientNumber)
        {
            case 0:
                // healed first patient, set flag
                GameProgressionManagerInstance.progressionSystem.SetFlag("firstHealed", true);
                break;
            case 1:
                // healed second patient, set flag
                GameProgressionManagerInstance.progressionSystem.SetFlag("secondHealed", true);
                break;
            case 2:
                // healed third patient, set flag
                GameProgressionManagerInstance.progressionSystem.SetFlag("thirdHealed", true);
                break;
        }

        GameProgressionManagerInstance.TransitionScene("Won");

        GameProgressionManagerInstance.lilithPatientNumber++;
    }
}
