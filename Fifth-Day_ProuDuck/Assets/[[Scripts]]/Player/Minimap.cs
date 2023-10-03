using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;

    public GameObject dayNight;

    public GameObject minimap;

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }

    public void whenButtonClicked()
    {
        if (minimap.activeInHierarchy == true)
        {
            minimap.SetActive(false);
            dayNight.SetActive(true);
        }
    }
}
