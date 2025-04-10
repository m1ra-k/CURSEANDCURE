using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogueData : MonoBehaviour
{
    public List<TextAsset> characterDialogues = new();
    public GameProgressionManager GameProgressionManager;
    private GameObject[] npcs;
    private Dictionary<GameObject,HashSet<Vector2>> npcLocations=  new();

    void Awake()
    { 
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();
        HashSet<Vector2> locations = new();
        npcs=GameObject.FindGameObjectsWithTag("NPC");
        foreach(GameObject npc in npcs)
        {
            Vector2 position=npc.transform.position;
            Vector2 above=position+Vector2.up;
            Vector2 below=position+Vector2.down;
            Vector2 left=position+Vector2.left;
            Vector2 right=position+Vector2.right;
            locations.Add(above);
            locations.Add(below);
            locations.Add(left);
            locations.Add(right);
            Debug.Log("location "+npc.name+above+below+left+right);
            npcLocations.Add(npc,locations);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.KeypadEnter)&&CanTalk())
        {
            Debug.Log("Can talk"+GetNPC(transform.position));
            GameProgressionManager.DialogueSystemManager.SetVisualNovelJSONFile(null);    
        }
    }

    // TODO: afia
    bool CanTalk()
    {
        Debug.Log("Can talk called");
        Vector2 lilithPos=transform.position;
        Debug.Log("Lilith is at "+lilithPos);
        Debug.Log("npcLocations count: " + npcLocations.Count);
        foreach(KeyValuePair<GameObject, HashSet<Vector2>> entry in npcLocations)
        {
            GameObject npc=entry.Key;
            HashSet<Vector2> npcPositions = entry.Value;
            Debug.Log("Check talk locations"+npcPositions);
            if(npcPositions.Contains(lilithPos)){
                return true;
            }
        }
        return false;
    }

    string GetNPC(Vector2 position){
        foreach(KeyValuePair<GameObject, HashSet<Vector2>> entry in npcLocations)
        {
            GameObject npc=entry.Key;
            HashSet<Vector2> npcPositions = entry.Value;
            Debug.Log("Check talk locations"+npcPositions);
            if(npcPositions.Contains(position))
            {
                Debug.Log("Can talk to"+npc.name);
                return npc.name;
            }
        }
        return "";
    }
}
