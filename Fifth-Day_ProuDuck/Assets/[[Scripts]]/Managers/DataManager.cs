using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
/// <summary>
/// struct for save file, what will the file contain
/// </summary>
[Serializable]
public struct Savings
{
    public string savingDate;
    public Vector3 playerPos;

    public int seed;
}
/// <summary>
/// This wrapper used for proper pasing into JSON file.
/// </summary>
[Serializable]
public class SavingsWrapper
{
    public List<Savings> savings = new List<Savings>();
}

public class DataManager : MonoBehaviour
{
#region variables
    private GameObject _player;
    private GameObject _camera;
    private string filePath = "";
    public List<Savings> savings = new List<Savings>();
    private MapGenerator _mapGenerator;
    [SerializeField] private string savingFileName = "savings.json";
    public Savings selectedSave;
#endregion

#region Instance
    private static DataManager instance = null;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset references and MapGenerator here
        if (FindObjectOfType<PlayerController>() != null)
        {
            _player = FindObjectOfType<PlayerController>().gameObject;
            _camera = GameObject.Find("First_Person_Camera");
        }
        else
        {
            _player = null;
            _camera = null;
        }

        if (FindObjectOfType<MapGenerator>() != null)
        {
            _mapGenerator = FindObjectOfType<MapGenerator>();
        }
        else
        {
            _mapGenerator = null;
        }
    }
    /// <summary>
    /// Destroys extra data managers on loaded scene
    /// </summary>
    void DestroyExtraDataManagers()
    {
        DataManager original = Instance;

        DataManager[] saverClones = FindObjectsOfType<DataManager>();
        foreach (DataManager saver in saverClones)
        {
            if (saver != original)
            {
                Destroy(saver.gameObject);
            }
        }
    }

#endregion

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        selectedSave.seed = -1;
        DestroyExtraDataManagers();
        if(FindObjectOfType<PlayerController>() != null)
        {
        _player = FindObjectOfType<PlayerController>().gameObject;
        _camera = GameObject.Find("First_Person_Camera");
        }
        if(FindObjectOfType<MapGenerator>() != null )
        {
            _mapGenerator = FindObjectOfType<MapGenerator>();
        }
        
        filePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + savingFileName;
       
        SetSavingsFromJson();
        UpdateListOfSavingsInMenu();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.L))
        {
            NewSave();
        }
    }
    /// <summary>
    /// Creates new saving inside of the JSON file.
    /// </summary>
    public void NewSave()
    {
        Savings newSaving = new Savings();
        newSaving.savingDate = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        if(_player == null)
        {
            if(FindObjectOfType<PlayerController>())
            {
                _player = FindObjectOfType<PlayerController>().gameObject;
                newSaving.playerPos = _player.transform.position;
            }
            else
            return;
        }
        else
        {
            newSaving.playerPos = _player.transform.position;
        }
        
        if(_mapGenerator == null)
        {
            _mapGenerator = FindObjectOfType<MapGenerator>();
            newSaving.seed = _mapGenerator.noiseData.seed;
        }
        else
        {
            newSaving.seed = _mapGenerator.noiseData.seed;  
        }
        
        if (SavingExists(newSaving))
        {
            Debug.Log("This saving already exists");
            return;
        }
        savings.Add(newSaving);
        string json = GetJsonOfSavings();
        File.WriteAllText(filePath, json);
        Debug.Log("Saved to : " + filePath);
    }
    /// <summary>
    /// Checks if the saving with the exact date already exists.
    /// </summary>
    bool SavingExists(Savings savingForCheck)
    {
        foreach (var saving in savings)
        {
            if (savingForCheck.savingDate == saving.savingDate)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Proper parsing of our saving list into string for JSON
    /// </summary>
    string GetJsonOfSavings()
    {
        return JsonUtility.ToJson(new SavingsWrapper { savings = this.savings }, true);
    }
    /// <summary>
    /// Fills the List of savings with info from JSON file, if the JSON file exists.
    /// </summary>
    void SetSavingsFromJson()
    {
        if (!File.Exists(filePath))
            return;
        string json = File.ReadAllText(filePath);
        this.savings = JsonUtility.FromJson<SavingsWrapper>(json).savings;
        Debug.Log(this.savings.Count);
    }

    public void UpdateListOfSavingsInMenu()
    {
        if(GameObject.Find("SavingsGrid") != null)
        {
            List<GameObject> gameObjectsToDelete = new List<GameObject>();
            foreach (Transform child in GameObject.Find("SavingsGrid").transform)
            {
                gameObjectsToDelete.Add(child.gameObject);
            }
            foreach(var x in gameObjectsToDelete)
            {
                Destroy(x);
            }
           
            foreach(var save in savings)
            {
                GameObject newSave = (GameObject)Instantiate(Resources.Load("Prefabs/Save_b"),GameObject.Find("SavingsGrid").transform);
                newSave.transform.Find("DateText").GetComponent<TMP_Text>().text = save.savingDate;
            }
        }
    }
}

