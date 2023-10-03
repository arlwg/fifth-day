using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100.0f;
    public float maxHealth = 100.0f;
    public Transform healthBarTransform;
    public GameObject healthBar;


    public bool isDead = false;
    public void TakeDamage(float dmg)
    {
        health -= dmg;
    }

    private void Start()
    {
     health = maxHealth;   
    }
    void Update()
    {
        if(healthBar != null)
        {
            healthBar.transform.position = healthBarTransform.position;
        }
    }
}
