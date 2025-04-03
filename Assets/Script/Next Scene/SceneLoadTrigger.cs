using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] private SceneField[] sceneToLoad;
    [SerializeField] private SceneField[] sceneToUnLoad;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("ไม่พบผู้เล่นใน Scene หลังจากโหลด");
        }
        else
        {
            if (player.transform.parent == null && player.scene.name != "DontDestroyOnLoad")
            {
                DontDestroyOnLoad(player);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("Player entered trigger!");
            LoadScenes();
            UnloadScenes();
        }
    }

    private void LoadScenes()
    {
        foreach (var scene in sceneToLoad)
        {
            if (!IsSceneAlreadyLoaded(scene.SceneName))
            {
                SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
        }
    }

    private void UnloadScenes()
    {
        foreach (var scene in sceneToUnLoad)
        {
            if (IsSceneAlreadyLoaded(scene.SceneName))
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    private bool IsSceneAlreadyLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Characters");

        if (player != null)
        {
            Debug.Log("Player successfully found after scene load.");

            // ✅ ย้าย Player ไปยัง Scene ที่โหลดมา
            foreach (var s in sceneToLoad)
            {
                if (scene.name == s.SceneName)
                {
                    SceneManager.MoveGameObjectToScene(player, scene);
                    Debug.Log($"✅ Player moved to scene: {scene.name}");
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Player not found after scene load.");
        }
    }
}
