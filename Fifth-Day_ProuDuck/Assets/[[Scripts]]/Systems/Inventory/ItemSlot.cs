using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Holds reference and count of items, manages their visibility in the Inventory panel
public class ItemSlot : MonoBehaviour
{
    public Item item = null;
    public Recipe_Temp recipeItem = null;

    public bool isInInventoryPanel;

    [SerializeField]
    private TMP_Text inventoryDescriptionText;
    [SerializeField]
    private TMPro.TMP_Text inventoryNameText;
    [SerializeField]
    private TMPro.TMP_Text craftDescriptionText;
    [SerializeField]
    private TMPro.TMP_Text craftNameText;

    [SerializeField]
    private int count = 0;

    public int Count
    {
        get { return count; }
        set
        {
            count = value;
            UpdateGraphic();
        }
    }

    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private Image inventoryDescriptionIcon;
    [SerializeField]
    private Image craftDescriptionIcon;
    [SerializeField]
    private Sprite emptyItemIcon;
    [SerializeField]
    private TextMeshProUGUI itemCountText;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Color backgroundOriginColor;

    public bool isInCraftPanel;
    public bool isFunctional = true;

    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        background = GetComponent<Image>();
        backgroundOriginColor = background.color;

        if(isInInventoryPanel)
        {
            inventoryDescriptionIcon = FindObjectOfType<InventoryDescriptionItem>().gameObject.GetComponent<Image>();
            inventoryDescriptionText = FindObjectOfType<InventoryDescriptionText>().gameObject.GetComponent<TMPro.TMP_Text>();
            inventoryNameText = FindObjectOfType<InventoryDescriptionName>().gameObject.GetComponent<TMPro.TMP_Text>();

        }
        else if(isInCraftPanel)
        {
            craftDescriptionIcon = FindObjectOfType<CraftingDescriptionIcon>().gameObject.GetComponent<Image>();
            craftDescriptionText = FindObjectOfType<CraftingDescriptionText>().gameObject.GetComponent<TMPro.TMP_Text>();
            craftNameText = FindObjectOfType<CraftingDescriptionName>().gameObject.GetComponent<TMPro.TMP_Text>();
        }
        UpdateGraphic();
    }

    void Update()
    {

    }

    //Change Icon and count
    public void UpdateGraphic()
    {
        if (item != null)
        {
            if (count < 1)
            {

                if (isFunctional)
                {
                    item = null;

                    itemIcon.sprite = emptyItemIcon;
                }
                else if (isInCraftPanel)
                {
                    if (item != null)
                    {
                        itemIcon.sprite = item.icon;
                        itemIcon.gameObject.SetActive(true);
                    }
                    else if (recipeItem != null)
                    {
                        itemIcon.sprite = recipeItem.icon;
                        itemIcon.gameObject.SetActive(true);
                        itemCountText.gameObject.SetActive(false);

                    }
                    else
                    {
                        item = null;
                        itemIcon.sprite = emptyItemIcon;
                        itemCountText.gameObject.SetActive(false);

                    }
                    craftDescriptionIcon.sprite = item.icon;
                    craftDescriptionText.text = item.description;
                    craftNameText.text = item.name;
                }

                itemCountText.gameObject.SetActive(false);

                if(isInInventoryPanel)
                {
                    inventoryDescriptionIcon.sprite = emptyItemIcon;
                    inventoryDescriptionText.text = "";
                    inventoryNameText.text = "";
                }

            }
            else
            {
                //set sprite to the one from the itemjie

                if (isFunctional)
                {
                    itemIcon.sprite = item.icon;
                    itemIcon.gameObject.SetActive(true);
                }
                itemCountText.gameObject.SetActive(true);
                itemCountText.text = count.ToString();

                if (isInInventoryPanel)
                {
                    inventoryDescriptionIcon.sprite = item.icon;
                    inventoryDescriptionText.text = item.description;
                    inventoryNameText.text = item.name;
                }
            }
        }
        else if (isInCraftPanel)
        {

            itemCountText.gameObject.SetActive(false);

            if(item != null)
            {
                craftDescriptionIcon.sprite = item.icon;
                craftDescriptionText.text = item.description;
                craftNameText.text = item.name;

                itemIcon.sprite = item.icon;
                itemIcon.gameObject.SetActive(true);
            }
            else if(recipeItem != null)
            {
                craftDescriptionIcon.sprite = recipeItem.icon;
                craftDescriptionText.text = recipeItem.description;
                craftNameText.text = recipeItem.name;

                itemIcon.sprite = recipeItem.icon;
                itemIcon.gameObject.SetActive(true);
                itemCountText.gameObject.SetActive(false);

            }
            else
            {
                item = null;
                itemIcon.sprite = emptyItemIcon;
                itemCountText.gameObject.SetActive(false);

            }

            if(item != null || recipeItem != null)
            {

            }

        }
        else
        {
            item = null;
            itemIcon.sprite = emptyItemIcon;
            itemCountText.gameObject.SetActive(false);

        }
    }

    public bool isItemCanUse()
    {
        return (item != null && count > 0);
    }


    public void OnClickEnter()
    {
        UpdateGraphic();

        if (!inventory.IsDragging() && (isFunctional || recipeItem != null))
        {
            if(recipeItem != null)
            {
                if(inventory.itemSlotList.CheckIngredient(recipeItem))
                {
                    inventory.itemSlotList.CollectItem(recipeItem);

                    Debug.Log("Crafting Complete");
                }
            }

            if (isItemCanUse())
            {
                if(GetComponent<QuickSlotController>() != null && !isInInventoryPanel)
                {
                    if (item.itemType == ItemType.FOOD)
                    {
                        Count--;
                        Debug.Log("Eat Item");
                    }
                }
                else
                {
                    if (!inventory.isSelectingItem())
                    {
                        inventory.selectItem = item;
                        inventory.selectItemSlot = this;

                        ToggleBackgroundColor();
                    }
                    else
                    {
                        if (inventory.selectItemSlot == this)
                        {
                            if (item.itemType == ItemType.FOOD)
                            {
                                Count--;
                                Debug.Log("Eat Item");
                            }
                        }
                        else if (inventory.selectItem == item)
                        {
                            if (inventory.selectItemSlot.isItemCanUse())
                            {
                                inventory.selectItemSlot.Count--;
                                item = inventory.selectItem;
                                Count++;
                            }

                            if (!inventory.selectItemSlot.isItemCanUse())
                            {
                                inventory.selectItemSlot.ToggleBackgroundColor();

                                inventory.RemoveSelecting();
                            }
                        }
                        else
                        {
                            inventory.selectItemSlot.ToggleBackgroundColor();

                            inventory.selectItem = item;
                            inventory.selectItemSlot = this;
                            ToggleBackgroundColor();

                        }
                    }
                }
            }
            else
            {
                if (inventory.isSelectingItem())
                {
                    if (inventory.selectItemSlot.isItemCanUse())
                    {
                        inventory.selectItemSlot.Count--;
                        item = inventory.selectItem;
                        Count++;
                    }

                    if (!inventory.selectItemSlot.isItemCanUse())
                    {
                        inventory.selectItemSlot.ToggleBackgroundColor();

                        inventory.RemoveSelecting();
                    }
                }
            }
        }
    }

    public void ToggleBackgroundColor()
    {
        if(background.color != backgroundOriginColor)
        {
            background.color = backgroundOriginColor;
        }
        else
        {
            background.color = Color.green;
        }
    }

    public void SwitchItem(ref ItemSlot newItemSlot)
    {
        ItemSlot itemSlot = new ItemSlot();
        itemSlot.count = newItemSlot.count;
        itemSlot.item = newItemSlot.item;

        newItemSlot.item = item;
        newItemSlot.Count = Count;

        item = itemSlot.item;
        Count = itemSlot.Count;
    }

    public void AddNewItem(Item newItem)
    {
        Count = 1;
        item = newItem;
    }

    public void AddSameItem(int count)
    {
        Count += count;
    }
}
