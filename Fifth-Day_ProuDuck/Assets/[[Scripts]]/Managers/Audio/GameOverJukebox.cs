using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverJukebox : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.FadeOut("FirstLevel_BGM"); 
        AudioManager.Instance.FadeIn("MainMenu_BGM", 1.0f, 0.5f);
    }
}
