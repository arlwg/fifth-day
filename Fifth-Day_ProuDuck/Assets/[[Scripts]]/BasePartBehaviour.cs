using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePartBehaviour : MonoBehaviour
{
    [SerializeField]
    int _health = 10;
    [SerializeField]
    int _takeDamageAmount = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeDamage(int damage)
    {
        _health -= damage;
        if(_health < 0)
            Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EnemyAttack"))
        {
            TakeDamage(_takeDamageAmount);
        }
    }
}
