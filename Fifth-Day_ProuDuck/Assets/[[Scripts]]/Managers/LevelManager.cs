using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
 private static LevelManager instance = null;
    public static LevelManager Instance
    {
         get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

    }
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PrevScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void LoadSceneByName(string name)
    {
        //SceneManager.LoadScene(name);
        LoadingManager.Instance.LoadScene(name);
    }
    public void LoadSceneByIndex(int index)
    {
        //SceneManager.LoadScene(index);
        LoadingManager.Instance.LoadScene(index);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("FirstLevelScene");
    }
}
