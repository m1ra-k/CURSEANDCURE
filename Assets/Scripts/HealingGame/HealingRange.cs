using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealingRange
{
    public int start;
    public int end;

    public HealingRange(int start, int end)
    {
        this.start = start;
        this.end = end;
    }
}
