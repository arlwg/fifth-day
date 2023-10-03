using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
[CreateAssetMenu(fileName = "Reward", menuName = "Rewards/Reward", order = 1)]
public class Reward : ScriptableObject
{
    public int rewardID;
    
    public ItemListAmount[] rewards;
    

    

}
