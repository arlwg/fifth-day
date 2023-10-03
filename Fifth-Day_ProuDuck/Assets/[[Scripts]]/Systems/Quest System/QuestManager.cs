using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


//TODO : Implement Item, QuestItem class. Certain objects will extend from this.
public class QuestManager : MonoBehaviour
{
    [SerializeField]
    public Transform viewer;

    public Vector3 viewerPosition;
    public Vector3 viewerPositionOld;
    public ScriptableObject[] day1Quests;
    public ScriptableObject[] day2Quests;
    public ScriptableObject[] day3Quests;
    public ScriptableObject[] day4Quests;
    public ScriptableObject[] day5Quests;
    
    public Dictionary<int, ScriptableObject[]> questMap;
    [SerializeField]
    public Quest selectedQuest;

    private int questIndex = 0;
    [SerializeField]
    private TextMeshProUGUI questText;

    private bool questsComplete = false;
    
    public Dictionary<int, Quest> questLog = new Dictionary<int, Quest>();

    public PlayerController _playerController;
    public QuestInterface _questInterface;
    public InventoryManagerNew inventory;
    public TimeController _timeController;
    private readonly float _thresholdForQuestUpdate = 5;


    public int currentDayQuests = 1;
    private void Awake()
    {
        _playerController = FindObjectOfType<PlayerController>();
        inventory = FindObjectOfType<InventoryManagerNew>();
        _timeController = FindObjectOfType<TimeController>();
        questMap = new Dictionary<int, ScriptableObject[]>();
        AddQuests();
        

    }

    private void Start()
    {

        if (_questInterface == null)
        {
            _questInterface = FindObjectOfType<QuestInterface>();

        }

        _questInterface.questTextHeader.text = "Quests Day " + currentDayQuests;
       
        PopulateQuestList(false, day1Quests);
        
        selectedQuest = questLog[0];
    }

    private void AddQuests()
    {
        questMap.Add(1, day1Quests);
        questMap.Add(2, day2Quests);
        questMap.Add(3, day3Quests);
        questMap.Add(4, day4Quests);
        questMap.Add(5, day5Quests);
    }

    public void CompleteAllQuests()
    {
        foreach (var quest in questLog)
        {
            quest.Value.complete = true;

            CompleteQuest(quest.Value);
        }
    }
    
    public void CompleteAllQuestsButOne()
    {

        _questInterface.CompleteAllButOne();
    }
    public void PopulateQuestList(bool resetList, ScriptableObject[] quests)
    {
        if (resetList)
        {
            questLog.Clear();
            _questInterface.ResetQuestList();

        }
        
        foreach (var quest in quests)
        {
            
            Quest typedQuest = quest as Quest;
            Quest questCopy = ScriptableObject.Instantiate(typedQuest); // Create a deep copy of the quest object
            questLog.Add(questCopy.id, questCopy);
        }
        
        foreach (var quest in questLog)
        {
            Debug.Log("adding quest : " + quest.Value.name);
            _questInterface.AddQuest(quest.Value);
        }
        
    }
    
    private void Update()
    {
        
        viewerPosition = _playerController.transform.position;

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > _thresholdForQuestUpdate * _thresholdForQuestUpdate)
        {
            foreach (var quest in questLog)
            {
                if (quest.Value is MovementQuest)
                {
                    quest.Value.CheckCondition(_playerController);
                }
            }
            viewerPositionOld = viewerPosition;
        }
    }

 
    
       
    public void CompleteQuest(Quest quest)
    {
        Debug.Log("Completing Quest : " + quest.name);
        
        
        GiveReward(quest);
        quest.handedIn = true;
        
        CheckOtherQuestsComplete();
        
        if (quest is InteractQuest interactQuest)
        {
            if (interactQuest.isLastQuest)
            {
                
                
                currentDayQuests += 1;
                
                _questInterface.questTextHeader.text = "Quests Day " + currentDayQuests;
                
                if (currentDayQuests > 5)
                {
                    SceneManager.LoadScene("GameOverWinScene");
                }

                if (currentDayQuests > questMap.Count)
                    return;
                
                PopulateQuestList(true, questMap[currentDayQuests]);
            }

           
        }
        
        
       
       
    }

    //Need to check if all other quests are complete before allowing player to do interact quest. -> Grey out quest on interface (if(lastquest)) -> 
    public void CheckOtherQuestsComplete()
    {
        int i = 0;
        foreach (var quest in questLog)
        {
            if (quest.Value.handedIn)
            {
                i++;
            }
        }

        if (i == questLog.Count - 1)
        {
            _questInterface.EnableLastQuest();
        }
    }

    public void GiveReward(Quest quest)
    {
        //TODO : Add pop up with rewards! Claim()?
        foreach (var reward in quest.reward.rewards)
        {
            inventory.AddItemToInventory(reward.resource, reward.amount);
        }
    }
    
    
    public void HandIn(int questID)
    {
        
        if (questLog[questID])
        {
            Quest quest = questLog[questID];
            CompleteQuest(quest);
        }
    }
    
    
}





