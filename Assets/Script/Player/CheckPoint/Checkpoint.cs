using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private string checkpointId;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // อัปเดตจุด Respawn
            var respawnManager = FindObjectOfType<RespawnManager>();
            if (respawnManager != null)
            {
                respawnManager.UpdateRespawnPoint(transform);
            }

            // ✅ เก็บจุด Checkpoint ลง GameData ผ่าน DataPersistenceManager
            var dataManager = DataPersistenceManager.instance;
            if (dataManager != null)
            {
                var gameData = dataManager.GetCurrentGameData();
                if (gameData != null)
                {
                    gameData.lastCheckpointScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    gameData.lastCheckpointId = checkpointId;
                    gameData.lastCheckpointPosition = transform.position;

                    Debug.Log($"✅ Saved checkpoint: {checkpointId} at {transform.position}");
                }

                dataManager.SaveGame(); // เซฟลงไฟล์จริงผ่าน FileDataHandler.cs
            }
        }
    }
}
