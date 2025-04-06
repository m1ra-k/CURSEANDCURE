using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealingRange
{
    public int yPosition;
    public int height;

    public HealingRange(int yPosition, int height)
    {
        this.yPosition = yPosition;
        this.height = height;
    }
}
