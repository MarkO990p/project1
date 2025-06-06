﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MenuController mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    [Header("Confirmation Popup")]
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    private SaveSlot[] saveSlots;
    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
        SceneManager.sceneLoaded += OnSceneLoaded;  // Register event when scene is loaded
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // เมื่อ Scene ถูกโหลด, ให้บันทึกข้อมูล Scene ที่ถูกโหลดนั้น
        SaveCurrentSceneData(scene.name);
    }

    private void SaveCurrentSceneData(string sceneName)
    {
        // ทำการบันทึกข้อมูลทุกครั้งที่ Scene เปลี่ยน
        DataPersistenceManager.instance.SaveGame();
        Debug.Log($"Game saved for scene: {sceneName}");
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        string sceneName = saveSlot.GetSceneName();  // ดึงชื่อ Scene จาก SaveSlot
        Debug.Log("SCENENAME : " + sceneName);
        // ตรวจสอบให้แน่ใจว่า sceneName ไม่เป็นค่าว่าง
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Invalid scene name (empty string) for SaveSlot.");
            return;
        }

        // Disable all buttons
        DisableMenuButtons();

        // case - loading game
        if (isLoadingGame)
        {
            DataPersistenceManager.isNewGame = false;
            // เปลี่ยน profile ที่เลือกจาก SaveSlot
            DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            // โหลดข้อมูลเกมและฉากที่ผู้เล่นเคยบันทึกไว้
            DataPersistenceManager.instance.LoadGame();  // โหลดข้อมูลจาก SaveSlot
            SaveGameAndLoadScene(sceneName, DataPersistenceManager.isNewGame);  // ส่งชื่อ Scene ที่เลือกไป
        }
        // case - new game, but the save slot has data
        else if (saveSlot.hasData)
        {
            confirmationPopupMenu.ActivateMenu(
                $"Starting a New Game ({MenuController.selectedDifficulty}) with this slot will override the currently saved data. Are you sure?",
                () => {
                    DataPersistenceManager.isNewGame = true;
                    // เปลี่ยน profile และเริ่มเกมใหม่
                    DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.instance.NewGame();
                    SaveGameAndLoadScene(sceneName, DataPersistenceManager.isNewGame);  // ส่งชื่อ Scene ที่เลือกไป
                },
                () => {
                    // กลับไปที่เมนู SaveSlots
                    this.ActivateMenu(isLoadingGame);
                }
            );
            
        }
        // case - new game, and the save slot has no data
        else
        {
            DataPersistenceManager.isNewGame = true;
            // เริ่มเกมใหม่โดยไม่โหลดข้อมูล
            DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            DataPersistenceManager.instance.NewGame();
            SaveGameAndLoadScene(sceneName, DataPersistenceManager.isNewGame);  // ส่งชื่อ Scene ที่เลือกไป
            Debug.Log("NEW GAME!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!   : "+ sceneName);
        }
    }


    private void SaveGameAndLoadScene(string sceneName, bool isLoadGame)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Cannot load scene. Invalid scene name (empty string).");
            return;
        }

        // Save player position if player exists
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector3 currentPosition = player.transform.position;
            string currentScene = SceneManager.GetActiveScene().name;

            // Store position for the current scene
            DataPersistenceManager.instance.gameData.SetPlayerPositionForScene(currentScene, currentPosition);
            Debug.Log($"Saved position: {currentPosition} for scene: {currentScene}");
        }
        else
        {
            Debug.LogWarning("Player GameObject not found before saving game data.");
        }

        // Save the game before loading
        DataPersistenceManager.instance.SaveGame();

        // ❌ REMOVE THIS: It overwrites the just-saved data
        // DataPersistenceManager.instance.LoadGame();

        // Load the new scene
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        confirmationPopupMenu.ActivateMenu(
            "Are you sure you want to delete this saved data?",
            () => {
                DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
                DataPersistenceManager.instance.ClearSave(saveSlot.GetProfileId());
                ActivateMenu(isLoadingGame);
            },
            () => {
                ActivateMenu(isLoadingGame);
            }
        );
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame, string nameScene = null)
    {
        this.gameObject.SetActive(true);
        this.isLoadingGame = isLoadingGame;

        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        backButton.interactable = true;

        GameObject firstSelected = backButton.gameObject;
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);

            // กำหนด sceneName ถ้าไม่มี ให้ใช้ค่า default
            Debug.Log("NAME SCENEE CHECK : " + nameScene);
            string sceneName = profileData != null && !string.IsNullOrEmpty(profileData.lastSceneName)
                ? profileData.lastSceneName
                : nameScene;  // Default scene name

            // ส่งข้อมูลไปยัง SaveSlot
            saveSlot.SetData(profileData, sceneName);

            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }


        // set the first selected button
        Button firstSelectedButton = firstSelected.GetComponent<Button>();
        this.SetFirstSelected(firstSelectedButton);
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }

    public void OnContinueFromCheckpoint()
    {
        var data = DataPersistenceManager.instance.gameData;

        if (!string.IsNullOrEmpty(data.lastCheckpointScene))
        {
            SceneManager.LoadSceneAsync(data.lastCheckpointScene);
            // เมื่อโหลดเสร็จแล้ว, SetPlayerPosition() จะวาง Player ตามตำแหน่งที่บันทึกไว้
        }
        else
        {
            Debug.LogWarning("No checkpoint scene found.");
        }
    }

}
