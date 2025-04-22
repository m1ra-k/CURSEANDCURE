using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public GameProgressionManager GameProgressionManager;

    void Awake()
    {
        GameProgressionManager = GameObject.Find("GameProgressionManager").GetComponent<GameProgressionManager>();
    }

    public void PlayGame()
    {
        GameProgressionManager.TransitionScene("Play");
    }
}