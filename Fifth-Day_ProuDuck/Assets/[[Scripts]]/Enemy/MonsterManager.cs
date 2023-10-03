using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterManager : MonoBehaviour
{
    [SerializeField]
    MonsterMode _monsterMode;
    [SerializeField]
    Vector3 m_basePosition = new Vector3(0,0,0);
    public GameObject m_monster;
    List<GameObject> m_monsters = new List<GameObject>();
    GameObject m_monsterParent;

    [SerializeField]
    int _numberOfMonsters = 50;
    [SerializeField]
    float _closestDistanceToBase = 20;
    [SerializeField]
    float _farthestDistanceToBase = 100;


    private TerrainGenerator _terrainGenerator;
    public event Action OnEnemyGenerationCompleted;
    private void Awake()
    {
        _terrainGenerator = FindObjectOfType<TerrainGenerator>();
        
        _terrainGenerator.OnMapGenerationCompleted += StartGenerateMonsters;
        m_monster = Resources.Load<GameObject>("Prefabs/JellyMonster");
        m_monsterParent = new GameObject("Monster Parent");
    }

    void Start()
    {
        
        

        

        //MonsterFactory(_numberOfMonsters);
      //  LocateMonsters();
         
        SwitchMonstersMode(_monsterMode);
    }
 

/*    void LocateMonsters()
    {
        float spawnAreaRange = _farthestDistanceToBase - _closestDistanceToBase;
        float eachStepBetweenMonsters = spawnAreaRange / _numberOfMonsters;

        float spawnRadius = _closestDistanceToBase;
        
        foreach(GameObject monster in m_monsters)
        {
            Vector3 position = new Vector3(spawnRadius, monster.transform.position.y, 0) ;

            position = Quaternion.Euler(0f, Random.Range(0, 360), 0.0f) *  position + m_basePosition;
            position.y = monster.transform.position.y;

            monster.transform.position = position;

            spawnRadius += eachStepBetweenMonsters;
        }

    }*/

    public void StartGenerateMonsters()
    {
        Debug.Log("Generating monsters now");
        MonsterFactory(_numberOfMonsters);
    }
    void MonsterFactory(int numOfMonster)
    {
        if (m_monster == null)
        {
            Debug.LogError("m_monster is null. Make sure it is assigned correctly in the inspector or loaded from Resources.");
            return;
        }
        
        float spawnAreaRange = _farthestDistanceToBase - _closestDistanceToBase;
        float eachStepBetweenMonsters = spawnAreaRange / _numberOfMonsters;

        float spawnRadius = _closestDistanceToBase;

 
        for (int i = 0; i < numOfMonster; i++)
        {
            Vector3 position = new Vector3(spawnRadius, m_monster.transform.position.y, 10);

            position = Quaternion.Euler(0f, Random.Range(0, 360), 0.0f) * position + m_basePosition;
            position.y = m_monster.transform.position.y;

            m_monster.transform.position = position;

            spawnRadius += eachStepBetweenMonsters;

            m_monsters.Add(Instantiate(m_monster, m_monsterParent.transform));
        }
        
        Debug.Log("Enemies invoked");
        OnEnemyGenerationCompleted?.Invoke();
        _terrainGenerator.OnMapGenerationCompleted -= StartGenerateMonsters;
    }

    public void SwitchMonstersMode(MonsterMode mode)
    {
        Debug.Log("Enemy mode changed to " + mode.ToString());

        foreach (GameObject monster in m_monsters)
        {
            monster.GetComponent<EnemyBehaviour>().SetMonsterMode(mode);
        }

    }
    
}
