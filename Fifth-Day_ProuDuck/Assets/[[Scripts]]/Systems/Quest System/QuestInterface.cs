using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestInterface : MonoBehaviour
{
    public GameObject questInterface;
    private QuestManager _questManager;
    public Transform parentQuestList;
    public GameObject parentGameObject;

    public TextMeshProUGUI questTextHeader;
    public QuestDisplayItem prefab;


    private List<QuestDisplayItem> quests = new List<QuestDisplayItem>();

    public Sprite defaultIcon;

    private void Awake()
    {
        _questManager = FindObjectOfType<QuestManager>();
        questInterface = this.gameObject;
    }

    public void AddQuest(Quest quest)
    {
        QuestDisplayItem newPrefabInstance = Instantiate(prefab, parentQuestList);
        newPrefabInstance.Initialize(quest.name, quest.questIcon, quest, _questManager);

        if (newPrefabInstance == null)
        {
            Debug.Log("Null instance");
            return;
        }
        else if (quests == null)
        {
            Debug.Log("Null qust list");
        }
        quests.Add(newPrefabInstance);
    }

    public void ResetQuestList()
    {
        quests.Clear();
        
        
        QuestDisplayItem[] questObjects = parentGameObject.GetComponentsInChildren<QuestDisplayItem>();

        foreach (var q in questObjects)
        {
            Destroy(q.gameObject);
        }
    }
    
    public void EnableLastQuest()
    {
        foreach (var quest in quests)
        {
            if (quest.quest is InteractQuest interactQuest)
            {
                quest.GreyOut();
            }
        }
    }

    public void CompleteAllButOne()
    {
        for (int i = 0; i < quests.Count - 1; i++)
        {
            quests[i].quest.complete = true;
            quests[i].HandIn();
        }
    }


    public void EnableQuestInterface()
    {
        this.gameObject.SetActive(true);
        

        foreach (var q in quests)
        {
            q.quest.CheckCondition(_questManager._playerController);
            
            if (q.quest.complete && !q.quest.handedIn)
            {
                q.UnGreyOut();
            }
        }
        
        
        
    }

    public void DisableQuestInterface()
    {
        this.gameObject.SetActive(false);
    }
}
