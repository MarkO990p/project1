using UnityEngine;

public class PlayerData : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Health playerHealth; // อ้างอิง Health เดียว

    public void LoadData(GameData data)
    {
        transform.position = data.playerPosition;

        // ใช้เมธอดที่เหมาะสมแทนการเข้าถึงโดยตรง
        playerHealth.SetCurrentHealth(data.currentHealth);
        playerHealth.SetArmor(data.currentArmor);
    }

    public void SaveData(GameData data)
    {
        data.playerPosition = transform.position; 
        data.currentHealth = playerHealth.GetCurrentHealth();
        data.currentArmor = playerHealth.GetCurrentArmor();
    }
}