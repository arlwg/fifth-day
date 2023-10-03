using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour
{
    public Item item;
    public int count;
    public bool inAction = false;
    public InvetorySlotNew hoveredSlot;
    public QuickSlotNew hoveredQuickSlot;

    public List<InvetorySlotNew> inventorySlots;
    public List<QuickSlotNew> quickSlots;
    public QuickSlotNew quickSlotTakenFrom = null;
    public InvetorySlotNew slotTakenFrom = null;
    public RectTransform imageRectTransform;

    private void Start()
    {
        quickSlots = FindObjectsOfType<QuickSlotNew>().ToList();
        inventorySlots = FindObjectsOfType<InvetorySlotNew>().ToList();
    }
    void Update()
    {
        Vector3[] imageCorners = new Vector3[4];
        imageRectTransform = transform as RectTransform;
        imageRectTransform.GetWorldCorners(imageCorners);

        for (int i = 0; i < 4; i++)
        {
            var invSlot = inventorySlots.Find(slot => RectTransformUtility.RectangleContainsScreenPoint(slot.transform as RectTransform, imageCorners[i]));
            if (invSlot != null)
            {
                hoveredSlot = invSlot;
                break;
            }
            var tempQuickSlot = quickSlots.Find(slot => RectTransformUtility.RectangleContainsScreenPoint(slot.transform as RectTransform, imageCorners[i]));
            if (tempQuickSlot != null)
            {
                hoveredQuickSlot = tempQuickSlot;
                break;
            }
            if (tempQuickSlot == null && invSlot == null)
            {
                hoveredSlot = null;
                hoveredQuickSlot = null;
            }
        }
        if (!inAction)
        {
            if (item == null)
            {
                transform.position = new Vector3(-1473.75f, 0, 0);
                GetComponent<Image>().sprite = null;
                count = 0;
            }
            else
            {
                PutItem();
            }
        }
        else
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    transform.position = touch.position;
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        inAction = false;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    transform.position = Input.mousePosition;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    inAction = false;
                }
            }
        }
        SetIconSprite();
    }

    void ReturnItem()
    {
        if (slotTakenFrom != null)
        {
            slotTakenFrom.item = item;
            slotTakenFrom.count = count;
            item = null;
            slotTakenFrom = null;
        }
        if (quickSlotTakenFrom != null)
        {
            quickSlotTakenFrom.itemInSlot = item;
            quickSlotTakenFrom.itemCount = count;
            item = null;
            quickSlotTakenFrom = null;
        }
    }
    void PutItem()
    {
        if (item != null)
        {
            if (hoveredSlot == null && hoveredQuickSlot == null)
            {
               ReturnItem();
            }
            else
            {
                if (hoveredQuickSlot != null)
                {
                    if (hoveredQuickSlot.itemInSlot == null)
                    {
                        hoveredQuickSlot.itemInSlot = item;
                        hoveredQuickSlot.itemCount = count;
                    }
                    else if (hoveredQuickSlot.itemInSlot != null && hoveredQuickSlot.itemInSlot == item && hoveredQuickSlot.itemInSlot.isConsumable)
                    {
                        hoveredQuickSlot.itemCount += count;
                    }
                    else if (hoveredQuickSlot.itemInSlot != null && hoveredQuickSlot.itemInSlot == item && !hoveredQuickSlot.itemInSlot.isConsumable)
                    {
                        ReturnItem();
                    }
                    else if (hoveredQuickSlot.itemInSlot != null && hoveredQuickSlot.itemInSlot != item /*&& slotTakenFrom.type != SlotType.ENDPRODUCT && hoveredSlot.item.isConsumable*/)
                    {
                        if (slotTakenFrom != null && slotTakenFrom.type == SlotType.ENDPRODUCT)
                        {
                            ReturnItem();
                            item = null;
                            hoveredSlot = null;
                            slotTakenFrom = null;
                            quickSlotTakenFrom = null;
                            return;
                        }
                        if (slotTakenFrom != null)
                        {
                            slotTakenFrom.item = hoveredQuickSlot.itemInSlot;
                            slotTakenFrom.count = hoveredQuickSlot.itemCount;
                            hoveredQuickSlot.itemInSlot = item;
                            hoveredQuickSlot.itemCount = count;
                        }
                        if (quickSlotTakenFrom != null)
                        {
                            quickSlotTakenFrom.itemInSlot = hoveredQuickSlot.itemInSlot;
                            quickSlotTakenFrom.itemCount = hoveredQuickSlot.itemCount;
                            hoveredQuickSlot.itemInSlot = item;
                            hoveredQuickSlot.itemCount = count;
                        }
                        
                    }
                    else if ((hoveredQuickSlot.itemInSlot != null && hoveredQuickSlot.itemInSlot != item && slotTakenFrom.type == SlotType.ENDPRODUCT)
                             || (hoveredQuickSlot.itemInSlot != null && hoveredQuickSlot.itemInSlot != item && !hoveredQuickSlot.itemInSlot.isConsumable))
                    {
                        ReturnItem();
                    }
                    item = null;
                    hoveredQuickSlot = null;
                    slotTakenFrom = null;
                    quickSlotTakenFrom = null;
                    
                }
                if (hoveredSlot != null && hoveredSlot.type != SlotType.ENDPRODUCT)
                {
                   
                    /*if (hoveredSlot.item != null && hoveredSlot.item != item && quickSlotTakenFrom != null)
                    {
                        Debug.Log("1");
                    }*/
                    if (hoveredSlot.item == null)
                    {
                        hoveredSlot.item = item;
                        hoveredSlot.count = count;
                    }
                    else if (hoveredSlot.item != null && hoveredSlot.item == item && hoveredSlot.item.isConsumable)
                    {
                        hoveredSlot.count += count;
                    }
                    else if (hoveredSlot.item != null && hoveredSlot.item == item && !hoveredSlot.item.isConsumable)
                    {
                        ReturnItem();
                    }
                    else if (hoveredSlot.item != null && hoveredSlot.item != item /*&& (slotTakenFrom != null && slotTakenFrom.type != SlotType.ENDPRODUCT) && hoveredSlot.item.isConsumable*/)
                    {
                        if ((slotTakenFrom != null && slotTakenFrom.type == SlotType.ENDPRODUCT))
                        {
                            ReturnItem();
                            item = null;
                            hoveredSlot = null;
                            slotTakenFrom = null;
                            quickSlotTakenFrom = null;
                            return;
                        }
                        if (slotTakenFrom != null)
                        {
                            slotTakenFrom.item = hoveredSlot.item;
                            slotTakenFrom.count = hoveredSlot.count;
                            hoveredSlot.item = item;
                            hoveredSlot.count = count;
                        }
                        if (quickSlotTakenFrom != null)
                        {
                            quickSlotTakenFrom.itemInSlot = hoveredSlot.item;
                            quickSlotTakenFrom.itemCount = hoveredSlot.count;
                            hoveredSlot.item = item;
                            hoveredSlot.count = count;
                        }
                        
                    }
                    else if ((hoveredSlot.item != null && hoveredSlot.item != item && (slotTakenFrom != null && slotTakenFrom.type == SlotType.ENDPRODUCT))
                             ||  (hoveredSlot.item != null && hoveredSlot.item != item && !hoveredSlot.item.isConsumable))
                    {
                        ReturnItem();
                    }
                    item = null;
                    hoveredSlot = null;
                    slotTakenFrom = null;
                    quickSlotTakenFrom = null;
                }
                else if ( hoveredSlot != null && hoveredSlot.type == SlotType.ENDPRODUCT )
                {
                    ReturnItem();
                }
            }
           
        }
    }
    public void SetItemWithCount(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }

    void SetIconSprite()
    {
        if (item != null && GetComponent<Image>().sprite != item.icon)
        {
            GetComponent<Image>().sprite = item.icon;
            return;
        }
    }

   

}
