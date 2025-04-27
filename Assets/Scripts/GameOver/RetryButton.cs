using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetryButton : MonoBehaviour
{
    public GameProgressionManager GameProgressionManager;

    private Button retryButton;

    void Awake()
    {
        GameProgressionManager = GameObject.Find("GameProgressionManager").GetComponent<GameProgressionManager>();
    }

    void Start()
    {
        retryButton = GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            retryButton.onClick.Invoke();
        }
    }

    public void RetryGame()
    {
        GameProgressionManager.TransitionScene("Retry");
    }
}