using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementQuest", menuName = "Quests/MovementQuest", order = 2)]
public class MovementQuest : Quest
{
    public Vector3 targetPosition;
    
    public int completionRadius;
    
    [HideInInspector]
    public string targetTag;
    
    public MovementQuest(int id, string description, GameObject questObject, QuestManager questManager)
        : base(id, description,  questObject, questManager)
    {
      
    }

    public override void StartQuest()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        targetPosition = targetObject.transform.position;
        
        base.StartQuest();
    }
    
    public override bool CheckCondition(PlayerController player)
    {
        if (targetPosition == null)
        {
            return false;
        }

        if ((Vector3.Distance(player.gameObject.transform.position, targetPosition) < completionRadius))
        {
            Debug.Log("Quest complete : " + this.name);
            complete = true;
            return true;
        }


        return false;
    }
    
    
}
