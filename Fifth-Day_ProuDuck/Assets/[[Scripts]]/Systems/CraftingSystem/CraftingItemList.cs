using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingItemList : MonoBehaviour
{
    public List<ItemSlot> craftingItemList;

    private bool isInitialized = false;

    void Awake()
    {
        var itemSlots = GetComponentsInChildren<ItemSlot>();

        foreach (var itemSlot in itemSlots)
        {
            if (itemSlot.isInCraftPanel)
            {
                craftingItemList.Add(itemSlot);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var itemSlot in craftingItemList)
        {
            itemSlot.UpdateGraphic();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized)
        {
            gameObject.SetActive(false);
            isInitialized = true;
        }

    }
}
