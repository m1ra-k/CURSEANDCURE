using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealingGameManager : MonoBehaviour
{
    // just lilith for now
    public string mode = "Lilith";
    
    [SerializeField]    
    private Slider healingGauge;
    private float healingSpeed = 0.5f;
    private float healingGaugeValue = 0f;

    void Start()
    {
        
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
