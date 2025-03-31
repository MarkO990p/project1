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
            // เริ่มต้นข้อมูลเกมใหม่หากไม่มีข้อมูล
            gameData = new GameData();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  // Register event when scene is loaded
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // Deregister event when scene is unloaded
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);  // Stop auto-save when game is closed or scene changes
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ตรวจสอบการกำหนดค่า dataPersistenceObjects
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            Debug.LogError("No data persistence objects found in the scene.");
            return;  // หยุดการทำงานของฟังก์ชันนี้หากไม่มี dataPersistenceObjects
        }

        // ตรวจสอบว่า gameData ถูกโหลดหรือไม่
        if (gameData == null)
        {
            Debug.LogError("GameData is not loaded.");
            return;  // หากไม่มี gameData, หยุดการทำงาน
        }

        LoadGame();  // โหลดข้อมูลเกมเมื่อ Scene ถูกโหลด

        // หยุด Coroutine ที่กำลังทำงานอยู่
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSave());  // เริ่ม Coroutine สำหรับการบันทึกอัตโนมัติ

        // บันทึกฉากที่ผ่าน
        SetCurrentScene(scene.name);

        // ตั้งค่าตำแหน่งของผู้เล่น
        SetPlayerPosition();
    }




    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        LoadGame();
    }

    public void DeleteProfileData(string profileId)
    {
        // เรียกฟังก์ชัน Delete ใน FileDataHandler เพื่อลบข้อมูล
        dataHandler.Delete(profileId);
        InitializeSelectedProfileId();  // รีเซ็ตโปรไฟล์ที่เลือก
        LoadGame();  // โหลดเกมใหม่หลังจากลบข้อมูล
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

        // ตั้งค่าข้อมูลเริ่มต้น
        gameData.playerPosition = new Vector3(0, 0, 0);  // ตำแหน่งเริ่มต้นของผู้เล่น
        gameData.currentHealth = 100;  // พลังชีวิตเริ่มต้น
        gameData.currentArmor = 100;   // เกราะเริ่มต้น
        gameData.lastSceneName = "MainMenu";  // สมมุติว่าเริ่มต้นจาก MainMenu
        gameData.completedScenes = new List<string>();  // ฉากที่ผ่านไปแล้ว
        gameData.deathCount = 0;  // จำนวนครั้งที่ตาย
    }


    public void LoadGame()
    {
        if (disableDataPersistence)
        {
            return;
        }

        this.gameData = dataHandler.Load(selectedProfileId);  // โหลดข้อมูลจากไฟล์

        // หากไม่มีข้อมูล (gameData) จะต้องเริ่มต้นเกมใหม่
        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();  // เริ่มต้นเกมใหม่ (กำหนดค่าเริ่มต้นให้กับ gameData)
        }

        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;  // ถ้าไม่มีข้อมูลจะไม่ทำการโหลดต่อ
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);  // โหลดข้อมูลจาก gameData
        }

        Debug.Log("Loaded game data: " + gameData.playerPosition);  // ตรวจสอบค่าตำแหน่งผู้เล่นที่โหลด

        // ตั้งค่าตำแหน่งของผู้เล่นจากข้อมูลที่โหลด
        SetPlayerPosition();
    }



    public void SaveGame()
    {
        if (disableDataPersistence)
        {
            return;
        }

        // ตรวจสอบว่า gameData ถูกกำหนดค่าแล้ว
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            NewGame();  // เริ่มต้นเกมใหม่ถ้า gameData เป็น null
        }

        // ตรวจสอบว่า selectedProfileId ถูกกำหนดค่าหรือไม่
        if (string.IsNullOrEmpty(selectedProfileId))
        {
            Debug.LogError("Selected profile ID is not set. Cannot save game data.");
            return;  // หากไม่มี selectedProfileId, หยุดการบันทึก
        }

        // ตรวจสอบว่า dataPersistenceObjects ถูกกำหนดค่าแล้ว
        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            Debug.LogError("No data persistence objects found.");
            return;  // หากไม่มี dataPersistenceObjects, หยุดการบันทึก
        }

        // ทำการบันทึกข้อมูล
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);  // บันทึกข้อมูลไปยังอ็อบเจ็กต์ที่เกี่ยวข้อง
        }

        // บันทึกเวลาที่ข้อมูลถูกอัปเดตล่าสุด
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // ทำการบันทึกข้อมูลไปยังไฟล์
        dataHandler.Save(gameData, selectedProfileId);
        Debug.Log("Game saved successfully.");
    }




    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // ค้นหา objects ที่ implement IDataPersistence ใน Scene
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
            SaveGame();
            Debug.Log("Auto Saved Game");
        }
    }

    // ✅ เพิ่มการบันทึกฉากล่าสุด
    public void SetCurrentScene(string sceneName)
    {
        if (gameData != null)
        {
            gameData.lastSceneName = sceneName;  // บันทึกชื่อ Scene ล่าสุด
            MarkSceneComplete(sceneName);  // บันทึกว่า Scene นี้ผ่านแล้ว

            // บันทึกตำแหน่งของผู้เล่นในเกม
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                gameData.playerPosition = player.transform.position;  // บันทึกตำแหน่งของผู้เล่นใน Scene
            }
        }
    }



    public string GetLastSceneName()
    {
        return gameData?.lastSceneName;
    }

    // ✅ เพิ่มการบันทึก/ตรวจสอบฉากที่ผ่านแล้ว
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
        GameObject player = null;
        float timeout = 5f;  // เวลารอให้ Player โหลด (หน่วยเป็นวินาที)
        float elapsedTime = 0f;

        // ตรวจสอบว่าพบ Player หรือยัง
        while (player == null && elapsedTime < timeout)
        {
            player = GameObject.FindWithTag("Player");
            elapsedTime += Time.deltaTime;
        }

        if (player != null && gameData != null)
        {
            player.transform.position = gameData.playerPosition;  // ตั้งค่าตำแหน่งของผู้เล่น
            Debug.Log("Loaded player position: " + gameData.playerPosition);  // แสดงตำแหน่งที่โหลดจาก gameData
        }
        else
        {
            if (player == null)
            {
                Debug.LogError("Player GameObject not found in the scene! Ensure the Player GameObject has the 'Player' tag.");
            }

            if (gameData == null)
            {
                Debug.LogError("gameData is null! Ensure that gameData is loaded correctly before setting the player's position.");
            }
        }
    }




    // เปลี่ยนชื่อฟังก์ชัน GetAllProfilesGameData ใน DataPersistenceManager.cs
    public Dictionary<string, GameData> GetAllProfilesGameDataFromFile()
    {
        return dataHandler.LoadAllProfiles();  // ใช้ฟังก์ชัน LoadAllProfiles ใน FileDataHandler
    }



}
