using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;

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

    private int saveSlot;

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
            Debug.Log("SAVE ?????? "+gameData.currentArmor);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        dataHandler.Save(gameData, selectedProfileId);
        ExportGameDataToJson(selectedProfileId);
        Debug.Log("Game saved successfully.");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
    public void ExportGameDataToJson(string slotNumber)
    {
        if (gameData == null)
        {
            Debug.LogWarning("No game data to export.");
            return;
        }

        string json = JsonUtility.ToJson(gameData, true);

        // Define export path for the game folder (outside Application.dataPath)
        string gameFolder = Directory.GetParent(Application.dataPath).FullName;  // Game folder (parent of data)
        string difficultyName = gameData.gameDifficulty.ToString(); // "Easy"
        string fileName = $"GameDataExport_{difficultyName}_Slot{slotNumber}.json";
        string exportPath = Path.Combine(gameFolder, fileName);

        File.WriteAllText(exportPath, json);
        Debug.Log("Game data exported to JSON at: " + exportPath);

        Debug.Log("SAVE SLOT NUMBER " + slotNumber);
    }

    public void ClearSave(string slot)
    {
        // Delete internal save file (your own save system path)
        string saveFileName = $"Save_{gameData.gameDifficulty}_Slot{slot}.dat"; // Example save file
        string savePath = Path.Combine(Application.persistentDataPath, saveFileName); // Save path

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log($"Deleted save file: {savePath}");
        }

        // Delete exported JSON file
        string gameFolder = Directory.GetParent(Application.dataPath).FullName; // Game folder (parent of data)
        string exportFileName = $"GameDataExport_{gameData.gameDifficulty}_Slot{slot}.json";
        string exportPath = Path.Combine(gameFolder, exportFileName);

        if (File.Exists(exportPath))
        {
            File.Delete(exportPath);
            Debug.Log($"Deleted exported data file: {exportPath}");
        }
        else
        {
            Debug.LogWarning($"Exported data file not found: {exportPath}");
        }

        Debug.Log($"Save and exported data for Slot {slot} cleared.");

        Debug.Log("DELETED SLOT NUMBER " + slot);
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