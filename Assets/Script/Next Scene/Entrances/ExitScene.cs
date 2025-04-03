using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScene : MonoBehaviour
{
    public string destinationScene;
    public string entranceId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DataPersistenceManager.instance.gameData.lastCheckpointId = entranceId;
            SceneManager.LoadSceneAsync(destinationScene);
        }
    }
}
