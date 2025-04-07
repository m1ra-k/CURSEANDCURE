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
    private bool startedGame;
    private bool finishedGame;

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

    [Header("[References]")]
    public TextMeshProUGUI resultText;

    void Start()
    {
        // state
        GameProgressionManagerInstance = new GameProgressionManager();
    
        // range
        healingRangeList = JsonUtility.FromJson<HealingRangeList>(Resources.Load<TextAsset>($"Patients/{mode}_patient_" + GameProgressionManagerInstance.lilithPatientNumber).text);
    }

    void Update()
    {
        if (!finishedGame)
        {
            AdjustHealingGauge();
        }

        if (!startedGame && (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Return)))
        {
            StartCoroutine(AdjustForAllHealingRange());
            startedGame = true;
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
                resultText.text = "DONE!";
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
}
