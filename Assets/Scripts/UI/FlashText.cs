using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private string originalText;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        originalText = tmp.text;

        StartCoroutine(FlashingStartText());
    }

    private IEnumerator FlashingStartText()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            tmp.text = tmp.text.Equals(originalText) ? "" : originalText;
        }
    }
}