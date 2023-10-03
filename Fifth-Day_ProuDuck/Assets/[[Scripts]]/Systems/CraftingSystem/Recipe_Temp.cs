using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe_Temp", menuName = "Recipes/New Recipe_Temp")]
public class Recipe_Temp : Item
{
    
    public List<Item> RequiredItems = new List<Item>(4);

    //public Item CraftedItem;

}
