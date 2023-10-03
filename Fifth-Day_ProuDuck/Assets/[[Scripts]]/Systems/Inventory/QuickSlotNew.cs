using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuickSlotNew : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private int _count;
    public Item itemInSlot;
    public Item prevItem = null;
    public int itemCount;
    private NewPlayerWeaponSystem weaponSystem;
    private Sprite buttonSprite = null;
    private TMPro.TMP_Text countText;
    private PlayerController _player;
    
    private bool isButtonPressed = false;
    public bool isHovered = false;
    private bool wasPressedButNotActionYet = false;
    public Sprite defaultSprite;
    private float timer = 0;
    private bool gotItemFromHand = false;
    void Start()
    {
        weaponSystem = FindObjectOfType<NewPlayerWeaponSystem>();
        
        countText = (transform.Find("Count")).GetComponent<TMP_Text>();
        _player = FindObjectOfType<PlayerController>();
        defaultSprite = transform.Find("ItemSprite").GetComponent<Image>().sprite;
        buttonSprite = (itemInSlot == null) ? null : itemInSlot.icon;
        transform.Find("ItemSprite").GetComponent<Image>().sprite = buttonSprite;
    }

    void Update()
    {
        CheckItemCountText();
        UpdateItemInSlot();
        StartDragging();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPressed = true;
        wasPressedButNotActionYet = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonPressed = false;
        ClickOnSlot();
    }
    void StartDragging()
    {
        if (wasPressedButNotActionYet && FindObjectOfType<DragItem>().inAction)
        {
            wasPressedButNotActionYet = false;
        }
        if (isButtonPressed)
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f && itemInSlot != null)
            {
                var dragItem = FindObjectOfType<DragItem>();
                dragItem.inAction = true;
                dragItem.SetItemWithCount(itemInSlot, itemCount);
                dragItem.quickSlotTakenFrom = this;
                itemInSlot = null;
            }
        }
        
        if (!isButtonPressed && timer != 0)
        {
            timer = 0;
        }
    }
     void UpdateItemInSlot()
    {
        if (prevItem != itemInSlot && itemInSlot != null)
        {
            prevItem = itemInSlot;
            transform.Find("ItemSprite").GetComponent<Image>().sprite = itemInSlot.icon;
        }

        if (itemInSlot == null && transform.Find("ItemSprite").GetComponent<Image>().sprite != defaultSprite)
        {
            prevItem = null;
            transform.Find("ItemSprite").GetComponent<Image>().sprite = defaultSprite;
        }
        if (itemInSlot != null && !itemInSlot.isConsumable && itemCount > 1)
        {
            itemCount = 1;
        }
        if (itemInSlot == null)
        {
            itemCount = 0;
        }

        if (itemCount == 0 && !gotItemFromHand)
        {
            itemInSlot = null;
        }
        else if (itemCount == 0 && gotItemFromHand)
        {
            itemCount = 1;
        }

        
    }
    void ClickOnSlot()
    {
        if (timer < 1)
        {
            if (weaponSystem.itemInHand == null && itemInSlot.itemType == ItemType.WEAPON && itemInSlot != null)
            {
                weaponSystem.itemInHand = itemInSlot;
                itemInSlot = null;
                transform.Find("ItemSprite").GetComponent<Image>().sprite = defaultSprite;
                return;
            }
            if (wasPressedButNotActionYet)
            {
                if (itemInSlot == null)
                {
                    gotItemFromHand = true;
                    itemInSlot = weaponSystem.itemInHand;
                    weaponSystem.itemInHand = null;
                    SwapButtonSprite();
                    wasPressedButNotActionYet = false;
                    return;
                }
            }
            switch (itemInSlot.itemType)
            {
                case ItemType.WEAPON:
                    (itemInSlot, weaponSystem.itemInHand) = (weaponSystem.itemInHand, itemInSlot); //swap the items from slot to hand
                    SwapButtonSprite();
                    break;
                case ItemType.FOOD:
                    ConsumeItem();
                    break;
                case ItemType.INGREDIENT:
                    Debug.Log("Can't use this item, try to craft something with it");
                    break;
                case ItemType.TILE:
                    Debug.Log("Can't use this item, try to craft something with it");
                    break;
            }
        }

    }
    void ConsumeItem()
    {
        Debug.Log("Consuming Item: " + itemInSlot.name);
        if (_player == null)
            return;

        _player.health += itemInSlot.HP;
        _player.stamina += itemInSlot.Stamina;
        if (itemCount > 1)
        {
            itemCount--;
        }
        else
        {
            itemInSlot = null;
            transform.Find("ItemSprite").GetComponent<Image>().sprite = defaultSprite;
        }
    }
    
    public void SwapButtonSprite()
    {
        buttonSprite = itemInSlot.icon;
        transform.Find("ItemSprite").GetComponent<Image>().sprite = buttonSprite;
    }

    void CheckItemCountText()
    {
        if (itemInSlot != null && itemInSlot.isConsumable)
        {
            countText.text = itemCount.ToString();
        }
        else
        {
            countText.text = "";
        }
    }
}



   
