using Michsky.MUIP;
using TMPro;
using UnityEngine;

public class QuestDisplayItem : MonoBehaviour
{
    public TextMeshProUGUI questName;
    public CanvasGroup canvasGroup;
    
    public QuestManager questManager;
    public ButtonManager handInButton;

    
    
    private Sprite questIcon;
    public Quest quest;
    
    public float greyedOutAlpha = 0.5f;
    private float alpha = 1;
    private void Awake()
    {
       
       
        canvasGroup = GetComponent<CanvasGroup>();
        handInButton = GetComponentInChildren<ButtonManager>();
        questManager = FindObjectOfType<QuestManager>();
        
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        //ONCE OTHER QUESTS COMPLETE, CALL FUNCTION TO ENABLE LAST QUEST.
    }

    public void Initialize(string questName, Sprite questIcon, Quest quest, QuestManager questManager)
    {
        this.quest = quest;
        this.questIcon = questIcon;
        this.questName.text = questName;
        this.questManager = questManager;
        
        handInButton.onClick.AddListener(HandIn);
        GreyOut();

        if (quest is InteractQuest)
        {
            canvasGroup.alpha = 0;
        }
        //quest.StartQuest();
    }


    public void HandIn()
    {
        if (quest == null)
                return;
        
        
        // Check the current Quest variable and perform actions
        Debug.Log($"Handing in quest: {quest.description}");
            
        questManager.HandIn(quest.id);
        CompleteGrey();


    }
    
    public void GreyOut()
    {
        canvasGroup.alpha = greyedOutAlpha;
        handInButton.Interactable(false);
    }
    
    public void CompleteGrey()
    {
        canvasGroup.alpha = 0.1f;
        handInButton.Interactable(false);
    }

    public void UnGreyOut()
    {
        canvasGroup.alpha = 1;
        handInButton.Interactable(true);
    }
    
    
    
    
}
