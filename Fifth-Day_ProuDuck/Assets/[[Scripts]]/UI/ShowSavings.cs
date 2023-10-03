using UnityEngine;

public class ShowSavings : MonoBehaviour
{
    public void ShowSavingsList()
    {
        DataManager.Instance.UpdateListOfSavingsInMenu();
    }
}
