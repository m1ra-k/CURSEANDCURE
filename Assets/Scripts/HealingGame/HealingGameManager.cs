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
    // TODO eventually take off serializefield, just for debug
    [SerializeField]    
    private HealingRangeList healingRangeList;
    
    [Header("[Gauge]")]
    [SerializeField]    
    private Slider healingGauge;
    private float healingGaugeValue = 0f;
    private float healingSpeed = 0.5f;
    

    void Start()
    {
        GameProgressionManagerInstance = new GameProgressionManager();
        healingRangeList = JsonUtility.FromJson<HealingRangeList>(Resources.Load<TextAsset>($"Patients/{mode}_patient_" + GameProgressionManagerInstance.lilithPatientNumber).text);
    }

    void Update()
    {
        AdjustHealingGauge();
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
}
