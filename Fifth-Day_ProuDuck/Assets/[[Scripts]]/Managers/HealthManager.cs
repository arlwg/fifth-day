using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class HealthManager : MonoBehaviour
{
   private PlayerController _player;
   private QuestManager _questManager;
   private InventoryManagerNew _inventoryManagerNew;
   public Slider playerHpBar;
   private Slider playerStaminaBar;
   private EnemyHealth [] enemiesTempArray;
   public List<EnemyHealth> enemies;

   public ItemListAmount[] possibleDrops;
   private void Start()
   {
       _questManager = FindObjectOfType<QuestManager>();
       _inventoryManagerNew = FindObjectOfType<InventoryManagerNew>();
       _player = FindObjectOfType<PlayerController>();
   }
   
    public void FindAll()
    {
       FindPlayerAndSetStats();
       FindAllEnemies();
    }
   void FindPlayerAndSetStats()
   {
       if (FindObjectOfType<PlayerController>() != null)
       {
            _player = FindObjectOfType<PlayerController>();
       }
       playerHpBar = GameObject.Find("Player_HP").GetComponent<Slider>();
       playerHpBar.maxValue = _player.maxHealth;
       playerHpBar.value = _player.maxHealth;    
       playerStaminaBar = GameObject.Find("Player_Stamina").GetComponent<Slider>(); 
       playerStaminaBar.maxValue = _player.maxStamina;
       playerStaminaBar.value = _player.maxStamina;    
   }
   void FindAllEnemies()
   {
       enemies = new List<EnemyHealth>();
       enemiesTempArray = FindObjectsOfType<EnemyHealth>();
       foreach (var enemy in enemiesTempArray)
       {
           Instantiate(Resources.Load("Prefabs/EnemyHPtransform"),enemy.transform);
           if(enemy.gameObject.transform.Find("EnemyHPtransform(Clone)"))
           {
            enemy.healthBarTransform = enemy.gameObject.transform.Find("EnemyHPtransform(Clone)");
            if (enemy.gameObject.GetComponent<EnemyBehaviour>() != null)
            {
                switch(enemy.gameObject.GetComponent<EnemyBehaviour>().m_monsterMode)
                {
                    case MonsterMode.DayMode:
                        enemy.healthBarTransform.position += new Vector3(0,enemy.gameObject.transform.localScale.y * 1.3f,0);
                        break;
                    case MonsterMode.NightMode:
                        enemy.healthBarTransform.position += new Vector3(0,enemy.gameObject.transform.localScale.y * 3.3f,0);
                        break;
                }
            }
            else
            {
                enemy.healthBarTransform.position += new Vector3(0,enemy.gameObject.transform.localScale.y * 1.3f,0);
            }

            Instantiate(Resources.Load("Prefabs/EnemyHealthBarCanvas"),enemy.transform);
            enemy.healthBar = enemy.gameObject.transform.Find("EnemyHealthBarCanvas(Clone)").gameObject.transform.Find("enemyHPBar").gameObject;
            enemy.healthBar.GetComponent<Slider>().maxValue = enemy.maxHealth;
            enemy.healthBar.GetComponent<Slider>().value = enemy.health;
           }
           enemies.Add(enemy);
       }
   }

   private void Update()
   {
       CheckPlayerHealth();
       CheckEnemyHealth();
   }
   
   private void CheckPlayerHealth()
   {
       if (playerHpBar == null || _player == null || playerStaminaBar == null)
       {
           //Debug.Log("Something is wrong, player or playerHPbar is null!");
           return;
       }
           
       
        playerHpBar.value = _player.health;
        playerStaminaBar.value = _player.stamina;
       if (_player.health <= 0)
       {
           _player.health = 0.0f;
           Debug.Log("player died");
           //TODO: make pause with animation of player death here
           LevelManager.Instance.LoadSceneByIndex(2);
       }

       if (_player.health > _player.maxHealth)
       {
           _player.health = _player.maxHealth;
       }
       
       if (_player.stamina > _player.maxStamina)
       {
           _player.stamina = _player.maxStamina;
       }
   }

   private void CheckEnemyHealth()
   {
       foreach (EnemyHealth enemy in enemies)
       {
           enemy.healthBar.GetComponent<Slider>().value = enemy.health;
           enemy.healthBar.transform.LookAt(_player.transform);
           if (enemy.health <= 0 && !enemy.isDead)
           {

               foreach (var drop in drops())
               {
                   _inventoryManagerNew.AddItemToInventory(drop.resource, Random.Range(1, 10));
               }
               foreach (var quest in _questManager.questLog)
               {
                   if (quest.Value is KillQuest killQuest)
                   {
                       killQuest.IncreaseKillCount();
                   }
               }
               //_questManager.IncreaseAmountCount(1, ResourceType.Enemy);
               Debug.Log(enemy.transform.name + " is dead");
               enemy.health = 0;
                StartCoroutine(enemy.GetComponent<EnemyBehaviour>().DeathRoutine());
                if (enemy != null)
                   enemies.Remove(enemy);
                //Destroy(enemy.gameObject);
                enemy.isDead = true;
               break;
           }
       }
   }

   public ItemListAmount[] drops()
   {
       ItemListAmount[] items = new ItemListAmount[3];

       for (int i = 0; i < 3; i++)
       {
           items[i] = possibleDrops[Random.Range(0, possibleDrops.Length - 1)];
       }

       return items;
   }
}
