using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Attribute which allows right click->Create
[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class Item : ScriptableObject //Extending SO allows us to have an object which exists in the project, not in the scene
{
    [Header("Main Item Properties")]
    public Sprite icon;
    public string description = "";
    public bool isConsumable = false;
    public ItemType itemType;
    public int itemIndex;
    [Header("Weapon Properties")]
    public GameObject prefabOfAnObject;
    public float damage;
    public float range;
    public float FireFrequency;
    [Header("Food Properties")]
    public float HP;
    public float Stamina;
    public void Use()
    {
        Debug.Log("Used item: " + name + " - " + description);
    }

    public bool CheckIndex(int index)
    {
        return index == itemIndex ? true : false;
    }
}
