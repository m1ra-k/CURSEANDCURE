// BURGER

using System;
using System.Collections.Generic;

[Serializable]
public class BaseDialogueStruct
{
    public VNTypeEnum vnType;
    public CharacterEnum character;
    public List<SpeakerSpriteEnum> speakerSprite;
    public BGSpriteEnum bgSprite;
    public CGSpriteEnum cgSprite;
    public string dialogue;
    public float textSpeed;

    public override string ToString()
    {
        return "VNType: " + vnType.ToString() +
                " | CharacterEnum: " + character.ToString() +
                " | SpeakerSpriteEnum: " + speakerSprite.ToString() +
                " | BGSpriteEnum: " + bgSprite.ToString() +
                " | CGSpriteEnum: " + cgSprite.ToString() +
                " | dialogue: " + dialogue;
    }
}
