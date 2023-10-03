using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCPlayerDetector : MonoBehaviour
{
    public bool PlayerDetected = false;
    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //if(!TryGetComponent<PlayerController>(out PlayerController player)) { return; }
        if(other.CompareTag("Player"))
        {
            PlayerDetected = true;
            uiManager.InteractButton.GetComponent<Button>().interactable = true;
            uiManager.SetNPCID(GetComponentInParent<DialogueTrigger>()._Dialogue.ID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.InteractButton.GetComponent<Button>().interactable = false;
            PlayerDetected = false;
        }
    }
}
