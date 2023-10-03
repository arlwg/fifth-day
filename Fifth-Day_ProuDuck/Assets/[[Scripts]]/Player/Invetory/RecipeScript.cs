using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Recipe", menuName = "New Recipe")]
public class RecipeScript : ScriptableObject
{
    public List<ItemWithCount> ingredientList;
    public Item endProduct;
}
[Serializable]
public class ItemWithCount
{
    public Item item;
    public int count;

    public ItemWithCount(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }
}
