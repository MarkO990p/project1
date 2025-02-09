using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("การตั้งค่าเสียง")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;
    [SerializeField] private AudioSource backgroundMusic = null; // เพิ่ม AudioSource สำหรับเสียงเพลง
    [SerializeField] private float volumeStep = 0.1f;  // กำหนดค่าการเพิ่มหรือลดเสียง

    [Header("การยืนยัน")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("เลเวลที่จะโหลด")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    void Start()
    {
        // ตั้งค่าเริ่มต้นของ Slider ให้เป็นค่าเสียงที่บันทึกไว้หรือค่าเริ่มต้น
        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
        SetVolume(volumeSlider.value); // ปรับระดับเสียงเมื่อเริ่มเกม
    }

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void SetVolume(float volume)
    {
        // จำกัดค่าเสียงให้อยู่ระหว่าง 0 และ 1
        volume = Mathf.Clamp(volume, 0, 1);
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
        volumeSlider.value = volume;
        
        // ปรับระดับเสียงของ AudioSource ที่กำลังเล่นเพลง
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
    }

    // เมธอดสำหรับเพิ่มระดับเสียง
    public void IncreaseVolume()
    {
        SetVolume(AudioListener.volume + volumeStep);  // เพิ่มระดับเสียงทีละ step
    }

    // เมธอดสำหรับลดระดับเสียง
    public void DecreaseVolume()
    {
        SetVolume(AudioListener.volume - volumeStep);  // ลดระดับเสียงทีละ step
    }

    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
}
