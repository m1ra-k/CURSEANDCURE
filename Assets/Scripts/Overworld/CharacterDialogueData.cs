using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogueData : MonoBehaviour
{
    public List<TextAsset> characterDialogues = new();
    public GameProgressionManager GameProgressionManager;

    void Awake()
    {
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.KeypadEnter) && CanTalk())
        {
            // TODO: afia
            GameProgressionManager.dialogueSystemManager.SetVisualNovelJSONFile(null);    
        }
    }

    // TODO: afia
    bool CanTalk()
    {
        return false;
    }
}
