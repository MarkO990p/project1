using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int hp;  // 🩸 เซฟค่าพลังชีวิตของผู้เล่นในรูปแบบ int
}

public class SaveLoadSystem : MonoBehaviour
{
    private string savePath;

    void Start()
    {
        // กำหนดตำแหน่งที่ต้องการเซฟไฟล์ JSON
        savePath = @"D:\Project Game 2D\GameNumberS\Assets\Save\savegame.json";

        // ตรวจสอบว่าตำแหน่งโฟลเดอร์มีอยู่หรือไม่ หากไม่มีให้สร้าง
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log("Directory Created at: " + directoryPath);
        }

        LoadGame(); // ✅ โหลด HP อัตโนมัติเมื่อเริ่มเกม
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // บันทึกค่า HP จาก Health System
        Health playerHealth = FindObjectOfType<Health>();
        if (playerHealth != null)
        {
            data.hp = Mathf.RoundToInt(playerHealth.GetCurrentHealth());
        }

        // เขียนข้อมูลลง JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game Saved at: " + savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Health playerHealth = FindObjectOfType<Health>();
            if (playerHealth != null)
            {
                playerHealth.SetCurrentHealth(data.hp);
            }

            Debug.Log($"Game Loaded! HP: {data.hp}");
        }
        else
        {
            Debug.LogWarning("No Save File Found at: " + savePath);
        }
    }
}
