using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LinkedList<EnemyBehaviour> EnemiesOnMap = new LinkedList<EnemyBehaviour>();

    private PlayerController _player;

    private bool _musicChanged = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GetDynamicObjectsInMap();
    }

    private void GetDynamicObjectsInMap()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true);

        foreach (GameObject go in allObjects)
        {
            if (go != null)
            {
                if (go.TryGetComponent<EnemyBehaviour>(out EnemyBehaviour enemy))
                {
                    EnemiesOnMap.AddLast(enemy);
                }
                else if(go.TryGetComponent<PlayerController>(out PlayerController player))
                {
                    _player = player;
                }
                // ADD REQUIRED OBJECTS BELOW


                // END REQUIRED OBJECTS
                else
                {
                    continue;
                }
            }
        }

        Debug.Log("Enemies on map: " + EnemiesOnMap.Count);
        Debug.Log($"{_player.gameObject.name} has spawned.");
    }

}
