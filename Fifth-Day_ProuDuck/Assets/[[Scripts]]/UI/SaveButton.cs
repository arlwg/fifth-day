using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
  private void Start()
  {
    gameObject.GetComponent<Button>().onClick.AddListener(PressedSaveButton);
  }  

  void PressedSaveButton()
  {
    foreach(var save in DataManager.Instance.savings)
    {
        if(save.savingDate == gameObject.transform.Find("DateText").GetComponent<TMPro.TMP_Text>().text)
        {
            DataManager.Instance.selectedSave = save;
            LevelManager.Instance.LoadSceneByIndex(1);
        }
    }
  }
}
