using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    public List<InvetorySlotNew> listOfIngredients;
    public List<InvetorySlotNew> listOfPossibleCrafts;
    public List<RecipeScript> listOfRecipes;
    void Start()
    {
        foreach (var slot in FindObjectsOfType<InvetorySlotNew>())
        {
           if(slot.type == SlotType.INGRIDIENT )
            {
                listOfIngredients.Add(slot);
            }
        }

        for (int i = 0; i < GameObject.Find("EndItemsPanel").transform.childCount; i++)
        {
            listOfPossibleCrafts.Add(GameObject.Find("EndItemsPanel").transform.GetChild(i).GetComponent<InvetorySlotNew>());
        }
        //listOfPossibleCrafts.Reverse();

        //TODO: Load list of recipes from resources
        var tempArray = Resources.LoadAll<RecipeScript>("Recipes");
        listOfRecipes = new List<RecipeScript>();
        listOfRecipes = tempArray.ToList();

    }

    void Update()
    {
        CheckIngredientSlotsByRecipes();
    }

    void CheckIngredientSlotsByRecipes()
    {
        var ingSlotWith0Count = listOfIngredients.Find(slot => slot.count <= 0);
        if (ingSlotWith0Count != null)
        {
            ingSlotWith0Count.item = null;
        }
        
        foreach (var recipe in listOfRecipes)
        {
            if (recipe.ingredientList.All(ingredient => listOfIngredients.Exists(requiredItem => requiredItem.item == ingredient.item && requiredItem.count >= ingredient.count)))
            {
                AddPossibleCraft(recipe.endProduct);
            }
            else
            {
                DeleteImpossibleCraft(recipe.endProduct);
            }
        }
    }

    void AddPossibleCraft(Item item)
    {
        if (listOfPossibleCrafts.All(slot => slot.item != item))
        {
            var emptySlot = listOfPossibleCrafts.Find(slot => slot.item == null);
            if (emptySlot != null)
            {
                emptySlot.item = item;
            }
        }
    }

    void DeleteImpossibleCraft(Item item)
    {
        var notEmptySlot = listOfPossibleCrafts.Find(slot => slot.item == item);
        if (notEmptySlot == null || notEmptySlot.count > 0) return;
        notEmptySlot.item = null;
        notEmptySlot.count = 0;
        notEmptySlot.transform.Find("Count").GetComponent<TMPro.TMP_Text>().text = "0";
    }
    
    public void ConsumeIngredientsAndAddItem(InvetorySlotNew slot)
    {
        var foundSlot = listOfPossibleCrafts.Find(equalSlot => equalSlot == slot);
        if (foundSlot.item == null)
        {
            Debug.Log(foundSlot.transform);
            return;
        }
        //listOfRecipes.Any(recipe1 => recipe1.ingredientList.All(ingredient => listOfIngredients.Exists(requiredItem => requiredItem.item == ingredient.item && requiredItem.count >= ingredient.count)
        var tempRecipe = listOfRecipes.Find(recipe1 => recipe1.ingredientList.All(ingredient => listOfIngredients.Exists(requiredItem => requiredItem.item == ingredient.item && requiredItem.count >= ingredient.count) && foundSlot.item == recipe1.endProduct));
        if ((tempRecipe != null) && (FindObjectOfType<DragItem>() != null && !FindObjectOfType<DragItem>().inAction))
        {
            Debug.Log(tempRecipe.endProduct);
            if (foundSlot.item == tempRecipe.endProduct)
            { 
                if (foundSlot.item.isConsumable)
                {
                    foundSlot.count++;
                }
                else if(!foundSlot.item.isConsumable && foundSlot.count < 1)
                {
                   
                    foundSlot.count = 1;
                }
                else if (!foundSlot.item.isConsumable && foundSlot.count >= 1)
                {
                    return;
                }
                //consume ingredients
                var foundRecipe = listOfRecipes.Find(recipe => recipe.endProduct == foundSlot.item);
                if (foundRecipe == null) return;


                foreach (var ingredient in foundRecipe.ingredientList)
                {
                    var temp = listOfIngredients.Find(tempSlot => tempSlot.item == ingredient.item);
                    if (temp != null)
                    {
                        temp.count -= ingredient.count;
                    }
                }
            }
        }
    }
}
