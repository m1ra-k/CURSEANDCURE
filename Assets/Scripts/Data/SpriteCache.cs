using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteCache", menuName = "ScriptableObjects/SpriteCache")]
public class SpriteCache : ScriptableObject
{
    public Dictionary<string, Sprite> sprites = new();

    void OnEnable() 
    {
        LoadSprite("Sprites/BG/", Enum.GetNames(typeof(BGSpriteEnum)));
        LoadSprite("Sprites/SpeakerSprites/", Enum.GetNames(typeof(SpeakerSpriteEnum)));
    }

    void LoadSprite(string path, string[] spriteNames) 
    {
        foreach (string spriteName in spriteNames) 
        {
            sprites[spriteName] = Resources.Load<Sprite>(path + spriteName);
        }
    }
}
