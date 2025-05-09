using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Scene Information")]
    [SerializeField] private string sceneName;  // เก็บชื่อ Scene ที่เกี่ยวข้องกับ SaveSlot นี้

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI _modeText;
    [SerializeField] private TextMeshProUGUI checkPointText;

    [Header("Clear Data Button")]
    [SerializeField] private Button clearButton;

    public bool hasData { get; private set; } = false;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data, string sceneName)
    {
        // ถ้า sceneName ว่างเปล่าให้ใช้ค่า default
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is empty. Defaulting to MainMenu.");
            sceneName = "MainMenu";  // Default scene name
        }

        this.sceneName = sceneName;  // ตั้งค่า sceneName

        // ถ้าไม่มีข้อมูล
        if (data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }
        else
        {
            // ถ้ามีข้อมูล
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            _modeText.text = data.gameDifficulty.ToString();
            checkPointText.text = "Check Point : " + data.lastCheckpointId;
        }
    }


    public string GetProfileId()
    {
        return this.profileId;
    }

    public string GetSceneName()
    {
        return this.sceneName;  // คืนค่า sceneName
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
        clearButton.interactable = interactable;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // เมื่อ Scene ถูกโหลด, ให้บันทึกข้อมูล Scene ที่ถูกโหลดนั้น
        SaveCurrentSceneData(scene.name);
    }

    private void SaveCurrentSceneData(string sceneName)
    {
        // บันทึกข้อมูลทุกครั้งที่ Scene เปลี่ยน
        DataPersistenceManager.instance.gameData.lastSceneName = sceneName; // ตั้งค่า sceneName ใน GameData
        DataPersistenceManager.instance.SaveGame(); // บันทึกข้อมูลที่อัพเดต
        Debug.Log($"Game saved for scene: {sceneName}");
    }

}
