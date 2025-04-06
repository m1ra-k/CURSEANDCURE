using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealingGameManager : MonoBehaviour
{
    [Header("[State]")]
    public GameProgressionManager GameProgressionManagerInstance;
    // just lilith for now
    public string mode = "lilith";
    private int round;

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
    
    void Start()
    {
        // state
        GameProgressionManagerInstance = new GameProgressionManager();
    
        // range
        healingRangeList = JsonUtility.FromJson<HealingRangeList>(Resources.Load<TextAsset>($"Patients/{mode}_patient_" + GameProgressionManagerInstance.lilithPatientNumber).text);
    }

    void Update()
    {
        AdjustHealingGauge();

        // auto does this every 10 secs later on
        AdjustHealingRange();
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

        // print($"handle is at y = {healingGauge.handleRect.position.y}");
    }

    private void AdjustHealingRange()
    {
        if (Input.GetKeyDown(KeyCode.A) && round < 2)
        {
            // change y position
            Vector2 anchoredPos = rangeRectTransform.anchoredPosition;
            anchoredPos.y = healingRangeList.healingRanges[round].yPosition;
            rangeRectTransform.anchoredPosition = anchoredPos;

            // change height
            float currentWidth = rangeRectTransform.sizeDelta.x;
            float newHeight = healingRangeList.healingRanges[round].height;
            rangeRectTransform.sizeDelta = new Vector2(currentWidth, newHeight);

            // update box collider
            BoxCollider2D boxCollider = rangeRectTransform.GetComponent<BoxCollider2D>();
            Vector2 colliderSize = boxCollider.size;
            colliderSize.y = newHeight;
            boxCollider.size = colliderSize;
            Vector2 colliderOffset = boxCollider.offset;
            colliderOffset.y = 0f;
            boxCollider.offset = colliderOffset;

            round++;
        }
    }
}
