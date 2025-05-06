using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float maxHealth;
    public float maxArmor;

    public MenuController.GameDifficulty gameDifficulty;

    public string lastCheckpointScene;
    public string lastCheckpointId;
    public Vector3 lastCheckpointPosition;

    public Vector3 playerPosition;
    public long lastUpdated;
    public int deathCount;
    public SerializableDictionary<string, Vector3> scenePositions;  // เก็บตำแหน่งของผู้เล่นในแต่ละ Scene
    public SerializableDictionary<string, bool> coinsCollected;
    public float currentHealth;
    public float currentArmor;
    public AttributesData playerAttributesData;

    // เพิ่มข้อมูลสำหรับการบันทึก Scene
    public string lastSceneName;
    public List<string> completedScenes;

    // ค่า default ตอนเริ่มเกมใหม่
    public GameData()
    {
        playerPosition = Vector3.zero;
        this.deathCount = 0;
        scenePositions = new SerializableDictionary<string, Vector3>();  // ใช้ SerializableDictionary เพื่อเก็บตำแหน่ง
        coinsCollected = new SerializableDictionary<string, bool>();
        currentHealth = 100;
        currentArmor = 100;
        gameDifficulty = MenuController.selectedDifficulty;
        playerAttributesData = new AttributesData();

        lastSceneName = "Forestzone1";
        completedScenes = new List<string>();
    }

    // เมธอดคำนวณความคืบหน้า
    public int GetPercentageComplete()
    {
        int totalCollected = 0;
        foreach (bool collected in coinsCollected.Values)
        {
            if (collected)
                totalCollected++;
        }

        int percentageCompleted = -1;
        if (coinsCollected.Count != 0)
        {
            percentageCompleted = (totalCollected * 100 / coinsCollected.Count);
        }
        return percentageCompleted;
    }

    // บันทึกว่าผ่านฉากนี้แล้ว
    public void MarkSceneCompleted(string sceneName)
    {
        if (!completedScenes.Contains(sceneName))
        {
            completedScenes.Add(sceneName);
        }
    }

    // ตรวจสอบว่าผ่านฉากนี้แล้วหรือยัง
    public bool IsSceneCompleted(string sceneName)
    {
        return completedScenes.Contains(sceneName);
    }

    // บันทึกตำแหน่งของผู้เล่นในแต่ละ Scene
    public void SetPlayerPositionForScene(string sceneName, Vector3 position)
    {
        // ถ้ามีตำแหน่งใน Scene นี้แล้ว, อัปเดตมัน
        if (scenePositions.ContainsKey(sceneName))
        {
            scenePositions[sceneName] = position;
        }
        else
        {
            scenePositions.Add(sceneName, position);  // ถ้าไม่มี, ให้เพิ่มเข้าไป
        }
        Debug.Log($"Player position for {sceneName} set to: {position}");
    }

    // โหลดตำแหน่งผู้เล่นจากแต่ละ Scene
    public Vector3 GetPlayerPositionForScene(string sceneName)
    {
        // หากตำแหน่งของ Scene นั้นมีใน `scenePositions`
        if (scenePositions.ContainsKey(sceneName))
        {
            return scenePositions[sceneName];
        }
        Debug.Log($"No saved player position for {sceneName}, returning default position.");
        return Vector3.zero;  // คืนค่าตำแหน่งเริ่มต้นหากไม่มีการบันทึกตำแหน่งใน Scene นี้
    }
}
