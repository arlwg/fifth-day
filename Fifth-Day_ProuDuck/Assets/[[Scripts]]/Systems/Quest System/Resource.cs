using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Resource
{
    public ResourceType resourceType;
    public int requiredAmount;
    public int currentAmount;

    public void IncreaseCurrentAmount(int amount)
    {
        currentAmount += amount;
    }

    public bool IsMet()
    {
        return currentAmount >= requiredAmount;
    }
}
