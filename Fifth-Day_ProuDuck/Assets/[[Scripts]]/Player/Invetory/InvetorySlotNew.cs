using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public enum SlotType
{
    INVETORY,
    INGRIDIENT,
    ENDPRODUCT
}

public class InvetorySlotNew : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SlotType type;
    public Item item;
    public Item prevItem = null;
    public int count;
    public TMP_Text countText;
    public Sprite defaultSprite;

    public GameObject slotWhereWeGotTheItem;
    public GameObject border;
    public bool selected = false;
    public bool isButtonPressed = false;
    public bool isHovered = false;
    public float timer = 0;
    public PlayerController _player;
    public NewPlayerWeaponSystem weaponSystem;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        weaponSystem = FindObjectOfType<NewPlayerWeaponSystem>();
        border = transform.Find("Border").gameObject;
        border.SetActive(false);
        countText = transform.Find("Count").GetComponent<TMP_Text>();
        defaultSprite = transform.Find("ItemSprite").GetComponent<Image>().sprite;
    }
    void Update()
    {
        StartDragging();
        UpdateItemInSlot();
        setCountText();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonPressed = false;
        ClickOnSlotAction();
    }

    protected void UpdateItemInSlot()
    {
        if (prevItem != item && item != null)
        {
            prevItem = item;
            transform.Find("ItemSprite").GetComponent<Image>().sprite = item.icon;
        }

        if (item == null && GetComponent<Button>().image.sprite != defaultSprite)
        {
            prevItem = null;
            transform.Find("ItemSprite").GetComponent<Image>().sprite = defaultSprite;
        }
        
        if (item == null)
        {
            count = 0;
            countText.text = "";
        }

        if (count == 0 && type != SlotType.ENDPRODUCT)
        {
            item = null;
        }
    }

    protected void setCountText()
    {
        if (item != null && (item.isConsumable || (!item.isConsumable && type == SlotType.ENDPRODUCT))  && countText.text != count.ToString())
        {
            countText.text = count.ToString();
            return;
        }

        if ((item == null || !item.isConsumable || count == 0) && countText.text != "" && type != SlotType.ENDPRODUCT)
        {
            countText.text = "";
            return;
        }
    }


    protected void StartDragging()
    {
        if (isButtonPressed)
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f && item != null && (count > 0 || (!item.isConsumable && type != SlotType.ENDPRODUCT) ))
            {
                var dragItem = FindObjectOfType<DragItem>();
                dragItem.inAction = true;
                dragItem.SetItemWithCount(item, count);
                dragItem.slotTakenFrom = this;
                item = null;
                timer = 0;
            }
        }
        if (!isButtonPressed && timer != 0)
        {
            timer = 0;
        }
    }

    protected void ClickOnSlotAction()
    {
        if (timer < 1)
        {
            if (item != null && type == SlotType.INVETORY)
            {
                switch (item.itemType)
                {
                    case ItemType.WEAPON:
                        Debug.Log("WEAPON");
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

            if (type == SlotType.ENDPRODUCT)
            {
                FindObjectOfType<CraftSystem>().ConsumeIngredientsAndAddItem(this);
            }
            
            
        }
    }
    public void SwapButtonSprite()
    {
        GetComponent<Button>().image.sprite = item.icon;
    }

    protected void ConsumeItem()
    {
        Debug.Log("Consuming Item: " + item.name);
        if (_player == null)
            return;

        _player.health += item.HP;
        _player.stamina += item.Stamina;
        if (count > 1)
        {
            count--;
        }
        else
        {
            item = null;
            transform.Find("ItemSprite").GetComponent<Image>().sprite = defaultSprite;
        }
    }
}
