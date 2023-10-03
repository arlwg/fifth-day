using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryManagerNew : MonoBehaviour
{
   public List<InvetorySlotNew> listOfInventorySlots;
   public List<QuickSlotNew> listOfQuickSlots;
   private GameObject inventoryPanel;
   private GameObject craftPanel;


   private QuestManager _questManager;
   private void Start()
   {
      inventoryPanel = GameObject.Find("InventoryPanelNew");
      craftPanel = GameObject.Find("CraftPanel");
      Debug.Log("PANEL " + inventoryPanel);
      listOfInventorySlots = new List<InvetorySlotNew>();
      _questManager = FindObjectOfType<QuestManager>();
      
      foreach (var x in FindObjectsOfType<InvetorySlotNew>())
      {
         if (x.type == SlotType.INVETORY)
         {
            listOfInventorySlots.Add(x);
         }
      }
      
      listOfQuickSlots = FindObjectsOfType<QuickSlotNew>().ToList();
      StartCoroutine(DelayBeforeDisableOnStart());
   }

   IEnumerator DelayBeforeDisableOnStart()
   {
      yield return new WaitForSeconds(0.2f);
      inventoryPanel.gameObject.SetActive(false);
      craftPanel.gameObject.SetActive(false);
   }


   public void AddItemToInventory(Item item, int count)
   {
      var sameItem = listOfInventorySlots.Find(slot => slot.item == item);
      if (sameItem != null && sameItem.item.isConsumable)
      {
         sameItem.count += count;
         item = null;
         count = 0;
      }
      else
      {
         bool foundEmptySlot = false;
         for (int i = 0; i < listOfInventorySlots.Count - 1; i++)
         {
            if (listOfInventorySlots[i].item == null)
            {
               foundEmptySlot = true;
               listOfInventorySlots[i].item = item;
               listOfInventorySlots[i].count = count;
               item = null;
               count = 0;
               break;
            }
         }
         if (!foundEmptySlot)
         {
            Debug.Log("No empty slot found");
         }
      }
      
      
      foreach (var quest in _questManager.questLog)
      {
         if (quest.Value is GatheringQuest gatheringQuest)
         {
            gatheringQuest.CheckCondition(_questManager._playerController);
         }
      }
   }

   public int FindCountOfCertainItem(Item item)
   {
      var amount = 0;
      foreach (var slot in listOfInventorySlots)
      {
         if (slot.item == item)
         {
            amount += slot.count;
         }
      }
      foreach (var slot in listOfQuickSlots)
      {
         if (slot.itemInSlot == item)
         {
            amount += slot.itemCount;
         }
      }
      return amount;
   }
}
