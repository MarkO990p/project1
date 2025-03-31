using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Scene Information")]
    [SerializeField] private string sceneName;  // เก็บชื่อ Scene ที่เกี่ยวข้องกับ SaveSlot นี้

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI percentageCompleteText;
    [SerializeField] private TextMeshProUGUI deathCountText;

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
        // ตรวจสอบว่าชื่อ Scene ไม่เป็นค่าว่าง
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty or null for SaveSlot!");
            return;
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

            percentageCompleteText.text = data.GetPercentageComplete() + "% COMPLETE";
            deathCountText.text = "DEATH COUNT: " + data.deathCount;
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
}
