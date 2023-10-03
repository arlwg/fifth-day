using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest", order = 1)]
public class Quest : ScriptableObject
{
    public int id;
    public string name;
    public string description;
    public Sprite questIcon;
    
    public QuestCondition questCondition;
    public QuestManager questManager;
    
    public Reward reward;
    public bool active;
    public bool complete;
    public bool handedIn = false;
    
    
    
    
    public Quest(int id, string description, GameObject questObject, QuestManager questManager)
    {
        this.id = id;
        this.description = description;
        this.questManager = questManager;
    }

    public QuestCondition GetQuestCondition()
    {
        return questCondition;
    }


    public virtual void StartQuest()
    {
        active = true;
        questManager = FindObjectOfType<QuestManager>();
    }
    public virtual bool CheckCondition(PlayerController player)
    {
        return true;
    }


    public virtual void GiveRewards()
    {
        
    }

}


[Serializable]
public struct QuestCondition
{

    public bool isHandIn;
    public bool isTimed;

    public int questTimerValue;

}

[Serializable]
public class ItemListAmount
{
    public Item resource;
    public int amount;

    public ItemListAmount(Item resource, int amount)
    {
        this.resource = resource;
        this.amount = amount;
    }
}

public enum ResourceType
{
    Wood,
    Stone,
    Iron,
    Enemy,
    Radio,
    Slime
}

public enum QuestType
{
    GATHERING = 0,
    KILL = 1,
    MOVE = 2,
    CRAFT
}