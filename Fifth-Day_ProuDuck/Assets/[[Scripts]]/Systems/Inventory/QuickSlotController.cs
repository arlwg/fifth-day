using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    public QuickSlotNew itemSlot;
    public int quickSlotID;
    public bool isInInventoryPanel;
    public QuickSlotNew LinkedQuickSlot;

    // Start is called before the first frame update
    void Start()
    {
        itemSlot = GetComponent<QuickSlotNew>();

        if (isInInventoryPanel)
        {
            foreach (var quickSlot in FindObjectsOfType<QuickSlotController>())
            {
                if (quickSlot.quickSlotID == quickSlotID && !quickSlot.isInInventoryPanel)
                {
                    LinkedQuickSlot = quickSlot.GetComponent<QuickSlotNew>();
                    break;
                }
            }
        }
    }


    /*public void UpdateToLinkedQuickSlot()
    {
        if(isInInventoryPanel && itemSlot != null && LinkedQuickSlot!= null)
        {
            if (itemSlot.itemCount < 0 && LinkedQuickSlot.itemCount < 0)
            {
                return;
            }

            ItemSlot item = itemSlot.GetComponent<ItemSlot>();
            itemSlot.itemCount = item.Count;
            itemSlot.itemInSlot = item.item;


            LinkedQuickSlot.itemInSlot = itemSlot.itemInSlot;
            LinkedQuickSlot.itemCount = itemSlot.itemCount;

            LinkedQuickSlot.SwapButtonSprite();
        }
    }

    public void UpdateToInventoryQuickSlot()
    {
        if(isInInventoryPanel && itemSlot != null && LinkedQuickSlot!= null)
        {
            if (itemSlot.itemCount < 0 && LinkedQuickSlot.itemCount < 0)
            {
                return;
            }



            itemSlot.itemInSlot = LinkedQuickSlot.itemInSlot;
            itemSlot.itemCount = LinkedQuickSlot.itemCount;

            ItemSlot item = itemSlot.GetComponent<ItemSlot>();
            item.Count = itemSlot.itemCount;
            item.item = itemSlot.itemInSlot;


            item.UpdateGraphic();
        }
    }*/

}
