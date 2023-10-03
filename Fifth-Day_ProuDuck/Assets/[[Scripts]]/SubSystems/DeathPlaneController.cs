using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlaneController : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private CharacterController _player;
    private void OnTriggerEnter(Collider other)
    {
        print("Player entered");
        _player.enabled = false;
        _player.transform.position = _spawnPoint.position;
        _player.enabled = true;
    }
}
