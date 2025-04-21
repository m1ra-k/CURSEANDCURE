using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProgressionSystem", menuName = "Progression System")]
public class ProgressionSystem : ScriptableObject
{
    private Dictionary<string, bool> flags;

    public void Init()
    {
        flags = new Dictionary<string, bool> { 
            { "firstHealed", false }, 
            { "secondHealed", false },
            { "calledForHelp", false }, 
            { "thirdHealed", false }
        };
    }

    public bool GetFlag(string key)
    {
        return flags.TryGetValue(key, out bool value) && value;
    }

    public void SetFlag(string key, bool value)
    {
        flags[key] = value;
        Debug.Log($"SetFlag: {key} = {value}");
    }

}
