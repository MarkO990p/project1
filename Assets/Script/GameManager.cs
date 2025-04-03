//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class GameManager : MonoBehaviour
//{

//    public static GameManager instance;
//    public GameObject playerPrefab;

//    public GameData gameData { get; private set; }

//    private GameObject playerInstance;

//    //public Vector2 GetLastCheckpoint() => lastCheckpointPosition;


//    [Header("Checkpoint System")]
//    private Vector2 lastCheckpointPosition;

//    private void Awake()
//    {
//        if (instance != null)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        instance = this;
//        DontDestroyOnLoad(this.gameObject);

//        if (playerInstance == null)
//        {
//            playerInstance = Instantiate(playerPrefab);
//            DontDestroyOnLoad(playerInstance);
//        }
//    }

//    public GameObject GetPlayer()
//    {
//        if (playerInstance == null)
//        {
//            playerInstance = Instantiate(playerPrefab);
//            DontDestroyOnLoad(playerInstance);
//        }

//        return playerInstance;
//    }


//    private void OnEnable()
//    {
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    private void OnDisable()
//    {
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }

//    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        // เรียกใช้งานอื่นๆ ที่ต้องการเมื่อโหลด Scene
//        if (playerInstance != null)
//        {
//            playerInstance.transform.position = lastCheckpointPosition;
//        }
//    }

//    public void SetCheckpoint(Vector2 position)
//    {
//        lastCheckpointPosition = position;
//        Debug.Log($"Checkpoint saved at: {position}");
//    }

//    private Vector3 lastCheckpoint;

//    public void SetCheckpoint(Vector3 checkpointPosition)
//    {
//        lastCheckpoint = checkpointPosition;
//    }

//    public Vector3 GetLastCheckpoint()
//    {
//        return lastCheckpoint;
//    }


//}

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;

    public GameData gameData { get; private set; }

    private GameObject playerInstance;

    [Header("Checkpoint System")]
    private Vector3 lastCheckpoint;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
            DontDestroyOnLoad(playerInstance);
            lastCheckpoint = playerInstance.transform.position; // จุดเริ่มต้น
        }
    }

    public GameObject GetPlayer()
    {
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
            DontDestroyOnLoad(playerInstance);
        }

        return playerInstance;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerInstance != null)
        {
            playerInstance.transform.position = lastCheckpoint;
        }
    }

    // เรียกจาก Checkpoint Object
    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        lastCheckpoint = checkpointPosition;
        Debug.Log($"Checkpoint saved at: {checkpointPosition}");
    }

    public Vector3 GetLastCheckpoint()
    {
        return lastCheckpoint;
    }

    // เรียกจาก Health หรือ GameOver
    public void RespawnPlayer()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance); // 🔥 ลบตัวเก่าออกก่อน
        }

        // สร้างตัวใหม่ที่ตำแหน่ง checkpoint
        playerInstance = Instantiate(playerPrefab, lastCheckpoint, Quaternion.identity);
        DontDestroyOnLoad(playerInstance); // ถ้ายังต้องการให้รอดข้าม scene ได้

        var health = playerInstance.GetComponent<Health>();
        if (health != null)
        {
            health.ResetHealthAfterRespawn(50);
        }

        Debug.Log("Player respawned at: " + lastCheckpoint);
    }




}
