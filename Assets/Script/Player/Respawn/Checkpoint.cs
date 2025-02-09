using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // อัปเดตจุดเกิดใหม่
            RespawnManager respawnManager = FindObjectOfType<RespawnManager>();
            if (respawnManager != null)
            {
                respawnManager.UpdateRespawnPoint(transform);
            }

            // ✅ บันทึกค่า HP ของผู้เล่น
            SaveLoadSystem saveLoadSystem = FindObjectOfType<SaveLoadSystem>();
            if (saveLoadSystem != null)
            {
                saveLoadSystem.SaveGame();
                Debug.Log("Checkpoint Reached! Game Saved.");
            }
        }
    }
}
