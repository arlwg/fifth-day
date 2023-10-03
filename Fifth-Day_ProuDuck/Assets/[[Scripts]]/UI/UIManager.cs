using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject
        _attackButton,
        _interactButton,
        _inventoryButton,
        _characterButton,
        _minimapButton,
        _dayNightButton,
        _minimapParent,
        _dayNightParent,
        _quickSlot1,
        _quickSlot2,
        _quickSlot3,
        _quickSlot4,
        _inventoryPanel,
        _inventoryBox0,
        _inventoryBox1,
        _inventoryBox2,
        _inventoryBox3,
        _inventoryBox4,
        _inventoryBox5,
        _inventoryBox6,
        _inventoryBox7,
        _inventoryBox8,
        _inventoryBox9,
        _inventoryBox10,
        _inventoryBox11,
        _inventoryBox12,
        _inventoryBox13,
        _inventoryBox14,
        _inventoryBox15,
        _inventoryBox16,
        _inventoryBox17,
        _inventoryBox18,
        _inventoryBox19,
        _inventoryQuickSlot0,
        _inventoryQuickSlot1,
        _inventoryQuickSlot2,
        _inventoryQuickSlot3,
        _inventoryBackButton,
        _inventoryCraftingButton;

    private GameObject
        _craftingPanel,
        _craftingInventoryButton,
        _craftingBackButton,
        _alchemyPanel,
        _alchemyButton,
        _alchemyButtonPressed,
        _weaponsPanel,
        _weaponsButton,
        _weaponsButtonPressed,
        _defensesPanel,
        _defensesButton,
        _defensesButtonPressed;

    private GameObject
        _pauseMenu,
        _continueGameButton,
        _saveGameButton,
        _optionsButton,
        _returnToMainMenuButton,
        _pausePanel,
        _pauseText,
        _optionsPanel,
        _optionsBackButton;

    private GameObject
        _dialogueParent;

    private GameObject
        loadingText;
    private GameObject
        _player;
    
    public Sprite
        _craftingAlchemyImage,
        _craftingWeaponsImage,
        _craftingDefensesImage;


    private SceneLoader loader;
    private QuestManager _questManager;

    private bool isLoading = true;
    private bool isPaused = false;

    private int npcID;

    public bool IsPaused
    {
        get => isPaused;
        set => isPaused = value;
    }

    private int SaveFileIndex = 0;

    private const int CharacterPositionSaveDataSignifier = 0;

    public List<DialogueTrigger> DialogueTriggers = new List<DialogueTrigger>();

    public GameObject InteractButton { get { return _interactButton; } }

    private void Start()
    {
        Initialize();
        loader = FindObjectOfType<SceneLoader>();
        _questManager = FindObjectOfType<QuestManager>();
    }

    private void FixedUpdate()
    {
        if (!isLoading)
            return;


        //loadingText.GetComponent<TextMeshProUGUI>().text = "Loading Game : " + loader.eventsCompleted / loader.totalEventsToWaitFor * 100 + "%";


        /*if (loader.eventsCompleted / loader.totalEventsToWaitFor == 1)
        {
            isLoading = false;
        }*/
    }

    private void Initialize()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true);
        foreach (GameObject go in allObjects)
        {
            if(go.name == "AttackButton")
            {
                _attackButton = go;
            }
            else if(go.name == "InteractButton")
            {
                _interactButton = go;
            }
            else if(go.name == "InventoryButton")
            {
                _inventoryButton = go;
            }
            else if (go.name == "CharacterButton")
            {
                _characterButton = go;
            }
            else if(go.name == "MinimapParent")
            {
                _minimapParent = go;
            }
            else if(go.name == "Day&NightParent")
            {
                _dayNightParent = go;
            }
            else if (go.name == "LoadingText")
            {
                loadingText = go;
            }
            else if(go.name == "MinimapButton")
            {
                _minimapButton = go;
            }
            else if(go.name == "Day&NightButton")
            {
                _dayNightButton = go;
            }
            else if(go.name == "QuickSlot (1)")
            {
                _quickSlot1 = go;
            }
            else if (go.name == "QuickSlot (2)")
            {
                _quickSlot2 = go;
            }
            else if (go.name == "QuickSlot (3)")
            {
                _quickSlot3 = go;
            }
            else if (go.name == "QuickSlot (4)")
            {
                _quickSlot4 = go;
            }
            #region Inventory Slot Initialization
            else if (go.name == "InventoryPanelNew")
            {
                _inventoryPanel = go;
            }
            else if(go.name == "InventoryBox (0)")
            {
                _inventoryBox0 = go;
            }
            else if (go.name == "InventoryBox (1)")
            {
                _inventoryBox1 = go;
            }
            else if (go.name == "InventoryBox (2)")
            {
                _inventoryBox2 = go;
            }
            else if (go.name == "InventoryBox (3)")
            {
                _inventoryBox3 = go;
            }
            else if (go.name == "InventoryBox (4)")
            {
                _inventoryBox4 = go;
            }
            else if (go.name == "InventoryBox (5)")
            {
                _inventoryBox5 = go;
            }
            else if (go.name == "InventoryBox (6)")
            {
                _inventoryBox6 = go;
            }
            else if (go.name == "InventoryBox (7)")
            {
                _inventoryBox7 = go;
            }
            else if (go.name == "InventoryBox (8)")
            {
                _inventoryBox8 = go;
            }
            else if (go.name == "InventoryBox (9)")
            {
                _inventoryBox9 = go;
            }
            else if (go.name == "InventoryBox (10)")
            {
                _inventoryBox10 = go;
            }
            else if (go.name == "InventoryBox (11)")
            {
                _inventoryBox11 = go;
            }
            else if (go.name == "InventoryBox (12)")
            {
                _inventoryBox12 = go;
            }
            else if (go.name == "InventoryBox (13)")
            {
                _inventoryBox13 = go;
            }
            else if (go.name == "InventoryBox (14)")
            {
                _inventoryBox14 = go;
            }
            else if (go.name == "InventoryBox (15)")
            {
                _inventoryBox15 = go;
            }
            else if (go.name == "InventoryBox (16)")
            {
                _inventoryBox16 = go;
            }
            else if (go.name == "InventoryBox (17)")
            {
                _inventoryBox17 = go;
            }
            else if (go.name == "InventoryBox (18)")
            {
                _inventoryBox18 = go;
            }
            else if (go.name == "InventoryBox (19)")
            {
                _inventoryBox19 = go;
            }
            #endregion
            #region Inventory Quick Slot Initialization
            else if (go.name == "InventoryQuickSlot (0)")
            {
                _inventoryQuickSlot0 = go;
            }
            else if(go.name == "InventoryQuickSlot (1)")
            {
                _inventoryQuickSlot1 = go;
            }
            else if (go.name == "InventoryQuickSlot (2)")
            {
                _inventoryQuickSlot2 = go;
            }
            else if (go.name == "InventoryQuickSlot (3)")
            {
                _inventoryQuickSlot3 = go;
            }
            #endregion
            else if (go.name == "BackButton")
            {
                _inventoryBackButton = go;
            }
            else if(go.name == "CraftingButton")
            {
                _inventoryCraftingButton = go;
            }
            else if(go.name == "CraftingPanel")
            {
                _craftingPanel = go;
            }
            else if(go.name == "CraftingInventoryButton")
            {
                _craftingInventoryButton = go;
            }
            else if(go.name == "CraftingBackButton")
            {
                _craftingBackButton = go;
            }
            else if (go.name == "AlchemyCraftingBoxes")
            {
                _alchemyPanel = go;
            }
            else if(go.name == "AlchemyButton")
            {
                _alchemyButton = go;
            }
            else if(go.name == "AlchemyButton_Pressed")
            {
                _alchemyButtonPressed = go;
            }
            else if (go.name == "WeaponsCraftingBoxes")
            {
                _weaponsPanel = go;
            }
            else if(go.name == "WeaponsButton")
            {
                _weaponsButton = go;
            }
            else if(go.name == "WeaponsButton_Pressed")
            {
                _weaponsButtonPressed = go;
            }
            else if (go.name == "DefensesCraftingBoxes")
            {
                _defensesPanel = go;
            }
            else if(go.name == "DefensesButton")
            {
                _defensesButton = go;
            }
            else if(go.name == "DefensesButton_Pressed")
            {
                _defensesButtonPressed = go;
            }
            else if(go.name == "PauseMenu")
            {
                _pauseMenu = go;
            }
            else if(go.name == "PauseTextImage")
            {
                _pauseText = go;
            }
            else if(go.name == "ContinueButton")
            {
                _continueGameButton = go;
            }
            else if(go.name == "SaveButton")
            {
                _saveGameButton = go;
            }
            else if(go.name == "OptionsButton")
            {
                _optionsButton = go;
            }
            else if(go.name == "MainMenuButton")
            {
                _returnToMainMenuButton = go;
            }
            else if(go.name == "OptionsPanel")
            {
                _optionsPanel = go;
            }
            else if(go.name == "PausePanel")
            {
                _pausePanel = go;
            }
            else if(go.name == "OptionsBackButton")
            {
                _optionsBackButton = go;
            }
            else if (go.name == "DialogueParent")
            {
                _dialogueParent = go;
            }
            else if(go.name == "Player")
            {
                if(go != null)
                {
                    _player = go;
                }
            }

        }

        // PLAYER HUD

        _attackButton.GetComponent<Button>().onClick.AddListener(AttackButtonPressed);
        _interactButton.GetComponent<Button>().onClick.AddListener(InteractButtonPressed);
        _inventoryButton.GetComponent<Button>().onClick.AddListener(InventoryButtonPressed);
        _characterButton.GetComponent<Button>().onClick.AddListener(CharacterButtonPressed);
        //_dayNightButton.GetComponent<Button>().onClick.AddListener(DayNightButtonPressed);
        //_minimapButton.GetComponent<Button>().onClick.AddListener(MinimapButtonPressed);
        /*_quickSlot1.GetComponent<Button>().onClick.AddListener(QuickSlot1Pressed);
        _quickSlot2.GetComponent<Button>().onClick.AddListener(QuickSlot2Pressed);
        _quickSlot3.GetComponent<Button>().onClick.AddListener(QuickSlot3Pressed);
        _quickSlot4.GetComponent<Button>().onClick.AddListener(QuickSlot4Pressed);*/

        // PAUSE MENU
        _continueGameButton.GetComponent<Button>().onClick.AddListener(ContinueGameButtonPressed);
        _saveGameButton.GetComponent<Button>().onClick.AddListener(SaveGameButtonPressed);
        _optionsButton.GetComponent<Button>().onClick.AddListener(OptionsButtonPressed);
        _returnToMainMenuButton.GetComponent<Button>().onClick.AddListener(ReturnToMainMenuButtonPressed);
        _optionsBackButton.GetComponent<Button>().onClick.AddListener(OptionsBackButtonPressed);

        // INVENTORY 
        _inventoryQuickSlot0.GetComponent<Button>().onClick.AddListener(InventoryQuickSlot0Pressed);
        _inventoryQuickSlot1.GetComponent<Button>().onClick.AddListener(InventoryQuickSlot1Pressed);
        _inventoryQuickSlot2.GetComponent<Button>().onClick.AddListener(InventoryQuickSlot2Pressed);
        _inventoryQuickSlot3.GetComponent<Button>().onClick.AddListener(InventoryQuickSlot3Pressed);
        _inventoryBackButton.GetComponent<Button>().onClick.AddListener(InventoryBackButtonPressed);
        _inventoryCraftingButton.GetComponent<Button>().onClick.AddListener(InventoryCraftingButtonPressed);

        // CRAFTING
        _craftingInventoryButton.GetComponent<Button>().onClick.AddListener(CraftingInventoryButtonPressed);
        _craftingBackButton.GetComponent<Button>().onClick.AddListener(CraftingBackButtonPressed);
        _alchemyButton.GetComponent<Button>().onClick.AddListener(AlchemyButtonPressed);
        _weaponsButton.GetComponent<Button>().onClick.AddListener(WeaponsButtonPressed);
        _defensesButton.GetComponent<Button>().onClick.AddListener(DefensesButtonPressed);




    }

    #region Player HUD

    private void AttackButtonPressed()
    {
        FindObjectOfType<NewPlayerWeaponSystem>().useWeaponAction();
        // Debug.Log("Attack button was pressed. Remove this if the attack button is used elsewhere.");
    }

    private void InteractButtonPressed()
    {
        Debug.Log("Interact button was pressed.");
        NPCPlayerDetector nearestNPC = FindObjectOfType<NPCPlayerDetector>();
        NPCPlayerDetector[] npcPlayerDetectors = FindObjectsOfType<NPCPlayerDetector>();
        foreach (NPCPlayerDetector npc in npcPlayerDetectors)
        {
            if(npc.PlayerDetected)
            {
                foreach (DialogueTrigger dialogueTrigger in DialogueTriggers)
                {
                    if (dialogueTrigger._Dialogue.ID == npcID)
                    {
                        foreach (var quest in _questManager.questLog)
                        {
                            if (quest.Value is InteractQuest)
                            {
                                InteractQuest interactQuest = quest.Value as InteractQuest;
                                interactQuest.CheckCondition(npcID);
                            }
                        }
                        
                        dialogueTrigger.TriggerDialogue();
                        _dialogueParent.SetActive(true);
                        break;
                        
                    }
                }
                break;
               
            }
            else
            {
                _dialogueParent.SetActive(false);
            }
        }

    }

    public void SetNPCID(int npcID)
    {
        this.npcID = npcID;
    }

    private void InventoryButtonPressed()
    {
        Debug.Log("Inventory button was pressed.");
        _inventoryPanel.SetActive(true);
        //FindObjectOfType<Inventory>().UpdateItemSlotsGraphics();
    }

    private void CharacterButtonPressed()
    {
        Debug.Log("Character button was pressed. Changing TimeScale to 0.0f to pause game.");

        _pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;

    }

    //private void DayNightButtonPressed()
    //{
    //    Debug.Log("Day&Night button was pressed.");

    //    _dayNightParent.SetActive(false);
    //    _minimapParent.SetActive(true);
    //}

    //private void MinimapButtonPressed()
    //{
    //    Debug.Log("Minimap button was pressed.");
    //    _minimapParent.SetActive(true);
    //    _dayNightParent.SetActive(false);
    //}

    private void QuickSlot1Pressed()
    {
        Debug.Log("Quick Slot 1 Pressed.");
    }

    private void QuickSlot2Pressed()
    {
        Debug.Log("Quick Slot 2 Pressed.");
    }

    private void QuickSlot3Pressed()
    {
        Debug.Log("Quick Slot 3 Pressed.");
    }

    private void QuickSlot4Pressed()
    {
        Debug.Log("Quick Slot 4 Pressed.");
    }

    private void ContinueGameButtonPressed()
    {
        Debug.Log("Continue game button was pressed. Changing TimeScale to 1.0f to unpause game.");
        _pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;

        isPaused = false;
    }

    private void SaveGameButtonPressed()
    {
        Debug.Log("Save game button was pressed. Saving game data.");
        //StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + SaveFileIndex + ".txt");
        //SaveFileIndex++;
        //sw.WriteLine(CharacterPositionSaveDataSignifier + "," +
        //    _player.transform.position.x + "," +
        //    _player.transform.position.y + "," +
        //    _player.transform.position.z);


        //sw.Close();

        DataManager.Instance.NewSave();
    }
    private void OptionsButtonPressed()
    {
        Debug.Log("Options button was pressed.");
        _optionsPanel.gameObject.SetActive(true);
        _pausePanel.gameObject.SetActive(false);
        _pauseText.gameObject.SetActive(false);

    }

    private void OptionsBackButtonPressed()
    {
        Debug.Log("Options back button was pressed.");

        _optionsPanel.gameObject.SetActive(false);
        _pausePanel.gameObject.SetActive(true);
        _pauseText.gameObject.SetActive(true);
    }

    private void ReturnToMainMenuButtonPressed()
    {
        Debug.Log("Main Menu Button was pressed.");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenuScene");

    }

    #endregion

    #region Inventory

    private void InventoryQuickSlot0Pressed()
    {
        Debug.Log("Inventory Quick Slot 0 Pressed.");
    }
    private void InventoryQuickSlot1Pressed()
    {
        Debug.Log("Inventory Quick Slot 1 Pressed.");

    }
    private void InventoryQuickSlot2Pressed()
    {
        Debug.Log("Inventory Quick Slot 2 Pressed.");
    }
    private void InventoryQuickSlot3Pressed()
    {
        Debug.Log("Inventory Quick Slot 3 Pressed.");
    }
    private void InventoryBackButtonPressed()
    {
        Debug.Log("Inventory Back Button Pressed.");
        _inventoryPanel.gameObject.SetActive(false);
    }
    private void InventoryCraftingButtonPressed()
    {
        Debug.Log("Inventory Crafting Button Pressed.");
        _inventoryPanel.gameObject.SetActive(false);

        _craftingPanel.gameObject.SetActive(true);

        // Alchemy Page
        _alchemyButton.gameObject.SetActive(false);
        _alchemyButtonPressed.gameObject.SetActive(true);
        _craftingPanel.GetComponent<Image>().sprite = _craftingAlchemyImage;

        _alchemyPanel.SetActive(true);
        _weaponsPanel.SetActive(false);
        _defensesPanel.SetActive(false);

        // Weapons Button
        _weaponsButton.gameObject.SetActive(true);
        _weaponsButtonPressed.gameObject.SetActive(false);

        //Defenses Button
        _defensesButton.gameObject.SetActive(true);
        _defensesButtonPressed.gameObject.SetActive(false);
    }
    #endregion

    #region Crafting

    private void CraftingInventoryButtonPressed()
    {
        Debug.Log("Crafting Inventory Button Pressed");
        _craftingPanel.SetActive(false);
        _inventoryPanel.SetActive(true);

    }

    private void CraftingBackButtonPressed()
    {
        Debug.Log("Crafting Back Button Pressed.");

        // Alchemy Page
        _alchemyButton.gameObject.SetActive(false);
        _alchemyButtonPressed.gameObject.SetActive(true);
        _craftingPanel.GetComponent<Image>().sprite = _craftingAlchemyImage;

        // Weapons Button
        _weaponsButton.gameObject.SetActive(true);
        _weaponsButtonPressed.gameObject.SetActive(false);

        //Defenses Button
        _defensesButton.gameObject.SetActive(true);
        _defensesButtonPressed.gameObject.SetActive(false);

        _craftingPanel.SetActive(false);
    }

    private void AlchemyButtonPressed()
    {
        Debug.Log("Alchemy Button Pressed.");
        // Alchemy Page
        _alchemyButton.gameObject.SetActive(false);
        _alchemyButtonPressed.gameObject.SetActive(true);
        _craftingPanel.GetComponent<Image>().sprite = _craftingAlchemyImage;

        _alchemyPanel.SetActive(true);
        _weaponsPanel.SetActive(false);
        _defensesPanel.SetActive(false);

        // Weapons Button
        _weaponsButton.gameObject.SetActive(true);
        _weaponsButtonPressed.gameObject.SetActive(false);

        //Defenses Button
        _defensesButton.gameObject.SetActive(true);
        _defensesButtonPressed.gameObject.SetActive(false);



    }

    private void WeaponsButtonPressed()
    {
        Debug.Log("Weapons Button Pressed.");
        // Weapons Page
        _weaponsButton.gameObject.SetActive(false);
        _weaponsButtonPressed.gameObject.SetActive(true);
        _craftingPanel.GetComponent<Image>().sprite = _craftingWeaponsImage;

        _alchemyPanel.SetActive(false);
        _weaponsPanel.SetActive(true);
        _defensesPanel.SetActive(false);

        // Alchemy Button
        _alchemyButton.gameObject.SetActive(true);
        _alchemyButtonPressed.gameObject.SetActive(false);

        //Defenses Button
        _defensesButton.gameObject.SetActive(true);
        _defensesButtonPressed.gameObject.SetActive(false);

    }

    private void DefensesButtonPressed()
    {
        Debug.Log("Weapons Button Pressed.");

        // Defenses Page
        _defensesButton.gameObject.SetActive(false);
        _defensesButtonPressed.gameObject.SetActive(true);
        _craftingPanel.GetComponent<Image>().sprite = _craftingDefensesImage;

        _alchemyPanel.SetActive(false);
        _weaponsPanel.SetActive(false);
        _defensesPanel.SetActive(true);

        // Weapons Button
        _weaponsButton.gameObject.SetActive(true);
        _weaponsButtonPressed.gameObject.SetActive(false);


        // Alchemy Button
        _alchemyButton.gameObject.SetActive(true);
        _alchemyButtonPressed.gameObject.SetActive(false);
    }


    #endregion




}
