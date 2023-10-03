using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLevelJukebox : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.FadeOut("MainMenu_BGM");
        AudioManager.Instance.FadeIn("FirstLevel_BGM", 1.0f, 0.5f);
        
        AudioManager.Instance.PlayPeriodically("WindHowl", 30);
    }
}
