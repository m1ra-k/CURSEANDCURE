using System;
using System.ComponentModel;

public enum CharacterEnum
{
    [Description("")]
    None,
    Ana,
    Boy,
    Girl,
    Lilith,
    Man,
    [Description("SYSTEM")]
    System
    [Description("Hilda")]
    TavernKeeper,
    [Description("???")]
    Unknown,
    Woman,
}

public static class EnumExtensions
{
    public static string GetParsedName(this Enum value)
    {
        var attribute = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }
}