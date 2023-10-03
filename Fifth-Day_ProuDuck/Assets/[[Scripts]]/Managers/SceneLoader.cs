using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;
    public MonsterManager MonsterManager;
    public PlacementGenerator PlacementGenerator;
    public QuestManager QuestManager;
    public GameObject loadingScreen;
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    
    public int totalEventsToWaitFor = 3;
    public int eventsCompleted = 0;


    public GameObject playButton;

    public TextMeshProUGUI loadingText;
    
    [SerializeField] private Image _progressBar;
    [SerializeField] private float _progressFillSpeed = 1.0f;
    
    private float _currentProgressPercentage;
    private float _currentDisplayedPercentage;

    private bool doneLoading = false;

    private void Awake()
    {
        // If not assigned in the inspector, find the TerrainGenerator object in the scene
        if (terrainGenerator == null)
        {
            terrainGenerator = FindObjectOfType<TerrainGenerator>();
        }

        if (MonsterManager == null)
        {
            MonsterManager = FindObjectOfType<MonsterManager>();
        }

        if (PlacementGenerator == null)
        {
            PlacementGenerator = FindObjectOfType<PlacementGenerator>();
        }

        if (QuestManager == null)
        {
            QuestManager = FindObjectOfType<QuestManager>();
        }

        terrainGenerator.OnMapGenerationCompleted += LoadGame;
        //terrainGenerator.OnNavMeshGenerationCompleted += LoadGame;
        MonsterManager.OnEnemyGenerationCompleted += LoadGame;
        PlacementGenerator.OnOutpostSpawned += LoadGame;
    }

    private void Update()
    {
        if (doneLoading)
            return;
        
        _progressBar.fillAmount = Mathf.Lerp(_progressBar.fillAmount, _currentProgressPercentage, Time.deltaTime * _progressFillSpeed);
        _currentDisplayedPercentage = Mathf.Lerp(_currentDisplayedPercentage, _currentProgressPercentage, Time.deltaTime * _progressFillSpeed);
        loadingText.text = Mathf.RoundToInt(_currentDisplayedPercentage * 100) + "%";

        
        //Debug.Log(_currentDisplayedPercentage);

        if (Mathf.Abs(_currentDisplayedPercentage - 1) <= 0.01)
        {
            DoneLoading();
            doneLoading = true;
        }
           
    }

    private void Start()
    {
        

        //playerPrefab.SetActive(false);
     
        loadingScreen.SetActive(true);
        
        
        Debug.Log("Loading");
    }

    private void LoadGame()
    {
        eventsCompleted++;
        _currentProgressPercentage = Mathf.Max((float)eventsCompleted, totalEventsToWaitFor) / totalEventsToWaitFor;

        Debug.Log("Events Completed : " + eventsCompleted + "Progress L " + _currentProgressPercentage);
        
        
  
    }


    private void DoneLoading()
    {
        if (eventsCompleted >= totalEventsToWaitFor)
        {
            
            playButton.SetActive(true);
            loadingText.gameObject.SetActive(false);
            // Instantiate the player at the playerSpawnPoint
            //Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            terrainGenerator.OnNavMeshGenerationCompleted -= LoadGame;
            terrainGenerator.OnMapGenerationCompleted -= LoadGame;
            MonsterManager.OnEnemyGenerationCompleted -= LoadGame;
            PlacementGenerator.OnOutpostSpawned -= LoadGame;
            
            //QuestManager.questLog[0].StartQuest();
            FindObjectOfType<HealthManager>().FindAll();
        }
    }


    public void DeActivate()
    {
        playerPrefab.transform.position = playerSpawnPoint.position;
        playerPrefab.SetActive(true);
        // Deactivate the loading screen
        loadingScreen.SetActive(false);
    }
}
