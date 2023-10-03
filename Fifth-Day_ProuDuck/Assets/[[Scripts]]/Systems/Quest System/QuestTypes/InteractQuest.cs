using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "InteractQuest", menuName = "Quests/InteractQuest", order = 3)]
public class InteractQuest : Quest
{

    //TODO : Change this to more appropriately represent each different type of NPC.
    public int interactID;
    public bool otherQuestsComplete;
    public bool isLastQuest;
    public InteractQuest(int id, string description, GameObject questObject, QuestManager questManager) 
        : base(id, description, questObject, questManager)
    {
        
    }
    
    
    public override void StartQuest()
    {
        base.StartQuest();
    }
    
    public bool CheckCondition(int id)
    {
        
        if (interactID == id)
        {
            Debug.Log("Quest complete : " + this.name);
            complete = true;
            
            
            return true;
        }


        return false;
    }
}