using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayDisplay : MonoBehaviour
{
    [SerializeField]
    private TimeController timeController;

    public GameObject dayNight;

    public GameObject minimap;

    [SerializeField]
    private TMPro.TextMeshProUGUI dayDisplay;

    private float daysPassed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        daysPassed = timeController.daysPassed;
        dayDisplay.text = "DAY " + daysPassed.ToString();
    }

    public void whenButtonClicked()
    {
        if (dayNight.activeInHierarchy == true)
        {
            dayNight.SetActive(false);
            minimap.SetActive(true);
        }
    }
}
