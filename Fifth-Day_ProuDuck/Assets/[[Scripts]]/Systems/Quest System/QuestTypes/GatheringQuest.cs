using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GatheringQuest", menuName = "Quests/GatheringQuest", order = 3)]
public class GatheringQuest : Quest
{
    public ItemListAmount[] ResourceAmount;
    
    public GatheringQuest(int id, string description, GameObject questObject, QuestManager questManager) 
        : base(id, description, questObject, questManager)
    {
        
    }
    
    
    public override void StartQuest()
    {
        
        base.StartQuest();
    }
    
    public override bool CheckCondition(PlayerController player)
    {
        if (questManager == null)
        {
            questManager = FindObjectOfType<QuestManager>();
        }
        foreach (var resource in ResourceAmount)
        {
     
            if (questManager.inventory == null)
            {
                Debug.Log("Quest manager was null");
                questManager.inventory = FindObjectOfType<InventoryManagerNew>();
            }
            if (questManager.inventory.FindCountOfCertainItem(resource.resource) >= resource.amount)
            {
                continue;
            }

            return false;
        }


        complete = true;
        return true;
    }
}
