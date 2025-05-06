using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    [Header("Auto Saving Configuration")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    public GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";

    private Coroutine autoSaveCoroutine;

    public static DataPersistenceManager instance { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        InitializeSelectedProfileId();

        if (gameData == null)
        {
            gameData = new GameData();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            Debug.LogError("No data persistence objects found in the scene.");
            return;
        }

        if (gameData == null)
        {
            Debug.LogError("GameData is not loaded.");
            return;
        }

        LoadGame();

        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSave());

        StartCoroutine(DelayedSaveSceneInfo(scene.name));

        SetPlayerPosition();
    }

    private IEnumerator DelayedSaveSceneInfo(string sceneName)
    {
        yield return null; // wait 1 frame
        SetCurrentScene(sceneName);
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        LoadGame();
    }

    public void DeleteProfileData(string profileId)
    {
        dataHandler.Delete(profileId);
        InitializeSelectedProfileId();
        LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        this.gameData.gameDifficulty = MenuController.selectedDifficulty;
        
        gameData.playerPosition = new Vector3(0, 0, 0);
        gameData.currentHealth = 100;
        gameData.currentArmor = 100;
        gameData.lastSceneName = "MainMenu";
        gameData.completedScenes = new List<string>();
        gameData.deathCount = 0;
    }

    public void LoadGame()
    {
        if (disableDataPersistence)
        {
            return;
        }

        this.gameData = dataHandler.Load(selectedProfileId);

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded game data: " + gameData.playerPosition);
        Debug.Log("Loaded game Difficulty: " + gameData.gameDifficulty);

        SetPlayerPosition();
    }

    public void SaveGame()
    {
        if (disableDataPersistence)
        {
            return;
        }

        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            NewGame();
        }

        if (string.IsNullOrEmpty(selectedProfileId))
        {
            Debug.LogError("Selected profile ID is not set. Cannot save game data.");
            return;
        }

        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            Debug.LogError("No data persistence objects found.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        dataHandler.Save(gameData, selectedProfileId);
        Debug.Log("Game saved successfully.");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            //SaveGame();
            Debug.Log("Auto Saved Game");
        }
    }

    public void SetCurrentScene(string sceneName)
    {
        if (gameData != null)
        {
            gameData.lastSceneName = sceneName;
            MarkSceneComplete(sceneName);

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                gameData.SetPlayerPositionForScene(sceneName, player.transform.position);
                Debug.Log($"[SAVE] Saved player position for {sceneName}: {player.transform.position}");
            }
        }
    }

    public string GetLastSceneName()
    {
        return gameData?.lastSceneName;
    }

    public void MarkSceneComplete(string sceneName)
    {
        gameData?.MarkSceneCompleted(sceneName);
    }

    public bool IsSceneCompleted(string sceneName)
    {
        return gameData != null && gameData.IsSceneCompleted(sceneName);
    }

    public void SetPlayerPosition()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && gameData != null)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            // ✅ ถ้าโหลดจาก Checkpoint
            if (gameData.lastCheckpointScene == currentSceneName && gameData.lastCheckpointPosition != Vector3.zero)
            {
                player.transform.position = gameData.lastCheckpointPosition;
                Debug.Log($"[LOAD] Loaded player from checkpoint: {gameData.lastCheckpointPosition}");
            }
            else
            {
                Vector3 scenePosition = gameData.GetPlayerPositionForScene(currentSceneName);
                player.transform.position = scenePosition;
                Debug.Log($"[LOAD] Loaded player from scene position: {scenePosition}");
            }
        }
    }


    public Dictionary<string, GameData> GetAllProfilesGameDataFromFile()
    {
        return dataHandler.LoadAllProfiles();
    }

    public GameData GetCurrentGameData()
    {
        return gameData;
    }

 

}