using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<EnemyBehaviour>(out EnemyBehaviour enemy)) { return; }
        Debug.Log($"{enemy.name} is nearby.");
        AudioManager.Instance.CrossFade("FirstLevel_BGM", "Combat_BGM", 1.0f);

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<EnemyBehaviour>(out EnemyBehaviour enemy)) { return; }
        Debug.Log($"{enemy.name} has left the area.");
        AudioManager.Instance.CrossFade("Combat_BGM", "FirstLevel_BGM", 1.0f);
    }
}
