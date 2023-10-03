using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotList : MonoBehaviour
{
    public List<ItemSlot> slotsList;

    private bool isInitialized = false;
    
    void Awake()
    {
        var itemSlots = GetComponentsInChildren<ItemSlot>();

        foreach(var itemSlot in itemSlots)
        {
            if(itemSlot.gameObject.GetComponent<QuickSlotController>() == null)
                slotsList.Add(itemSlot);
        }
    }

    private void Start()
    {
        foreach(var itemSlot in slotsList)
        {
            itemSlot.UpdateGraphic();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!isInitialized)
        {
            gameObject.SetActive(false);
            isInitialized = true;
        }
    }

    public void CollectItem(Item newItem)
    {

        foreach(var itemSlot in slotsList)
        {
            if (itemSlot.isItemCanUse())
            {
                if (itemSlot.item.CheckIndex(newItem.itemIndex))
                {
                    itemSlot.AddSameItem(1);
                    break;
                }
            }
            else if (!itemSlot.isItemCanUse())
            {
                itemSlot.AddNewItem(newItem);
                break;
            }
        }
    }

    public bool CheckIngredient(Recipe_Temp recipe)
    {
        bool first = false;
        bool second = false;
        bool third = false;

        ItemSlot firstSlot = null, secondSlot = null, thirdSlot = null;

        foreach (var itemSlot in slotsList)
        {
            if(itemSlot.isItemCanUse() && itemSlot.item.CheckIndex(recipe.RequiredItems[0].itemIndex))
            {
                first = true;
                firstSlot = itemSlot;
            }
            else if (itemSlot.isItemCanUse() && itemSlot.item.CheckIndex(recipe.RequiredItems[1].itemIndex))
            {
                second = true;
                secondSlot = itemSlot;

            }
            else if (itemSlot.isItemCanUse() && itemSlot.item.CheckIndex(recipe.RequiredItems[2].itemIndex))
            {
                third = true;
                thirdSlot = itemSlot;

            }



        }

        if(first && second && third)
        {
            firstSlot.Count--;
            secondSlot.Count--;
            thirdSlot.Count--;

            return true;
        }
        else
        {
            return false;
        }
    }
}
