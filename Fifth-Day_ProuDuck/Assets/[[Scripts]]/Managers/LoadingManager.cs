/// <summary>
/// Original script written by Tarodev
/// https://www.youtube.com/watch?v=OmobsXZSRKo&ab_channel=Tarodev
/// </summary>

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Image _progressBar;
    [SerializeField] private float _progressFillSpeed = 3.0f;
    
    
    
    private float _target;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, _progressFillSpeed * Time.deltaTime);
    }


    public async void LoadScene(string sceneName)
    {
        _target = 0.0f;
        _progressBar.fillAmount = 0.0f;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);

     
        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);


    }

    public async void LoadScene(int index)
    {
        _target = 0.0f;
        _progressBar.fillAmount = 0.0f;

        var scene = SceneManager.LoadSceneAsync(index);
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(100);          // Remove once hooked up with level generation
            _target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);             // Remove once hooked up with level generation
        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
    }
}
