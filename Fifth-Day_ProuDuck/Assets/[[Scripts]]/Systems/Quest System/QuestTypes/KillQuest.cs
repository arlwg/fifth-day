using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "KillQuest", menuName = "Quests/KillQuest", order = 4)]
public class KillQuest : Quest
{

    public int currentKills;
    public int requiredKills;
    
    public KillQuest(int id, string description, GameObject questObject, QuestManager questManager) 
        : base(id, description, questObject, questManager)
    {
        
    }
    
    
    public override void StartQuest()
    {
        
        base.StartQuest();
    }
    
    public override bool CheckCondition(PlayerController player)
    {
        
        
        if (currentKills >= requiredKills)
        {
            Debug.Log("Quest complete : " + this.name);
            complete = true;
            return true;
        }


        return false;
    }

    public void IncreaseKillCount()
    {
        
        
        currentKills++;
        CheckCondition(FindObjectOfType<QuestManager>()._playerController);
    }
}
