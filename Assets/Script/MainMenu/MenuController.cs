using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Header("การตั้งค่าเสียง")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;
    [SerializeField] private AudioSource backgroundMusic = null;
    [SerializeField] private float volumeStep = 0.1f;

    [Header("การยืนยัน")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("ปุ่มเมนู")]
    [SerializeField] private Button newGameEasyButton;
    [SerializeField] private Button newGameNomalButton;
    [SerializeField] private Button newGameHardButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button loadGameButton;
    //[SerializeField] private SaveSlotsMenu SaveSlotsMenu;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
        SetVolume(volumeSlider.value);
        DisableButtonsDependingOnData();
    }

    private void start()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }

    private void DisableButtonsDependingOnData()
    {
        //if (!DataPersistenceManager.instance.HasGameData())
        //{
        //    continueGameButton.interactable = false;
        //    loadGameButton.interactable = false;
        //}
    }

    public void OnNewGameEasyGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();

    }

    public void OnNewGameNomalGameClicked()
    {

    }

    public void OnNewGameHardGameClicked()
    {

    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void OnContinueGameClicked()
    {
        DisableMenuButtons();
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("LABzone1");
    }

    private void DisableMenuButtons()
    {
        if (newGameEasyButton != null)
            newGameEasyButton.interactable = false;

        if (newGameNomalButton != null)
            newGameNomalButton.interactable = false;

        if (newGameHardButton != null)
            newGameHardButton.interactable = false;

        if (continueGameButton != null)
            continueGameButton.interactable = false;
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
        volume = Mathf.Clamp(volume, 0, 1);
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
        volumeSlider.value = volume;

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

    public void IncreaseVolume()
    {
        SetVolume(AudioListener.volume + volumeStep);
    }

    public void DecreaseVolume()
    {
        SetVolume(AudioListener.volume - volumeStep);
    }

    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }
}
