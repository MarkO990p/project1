using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntranceScene : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public string entranceId;
        public Transform spawnTransform;
    }

    public List<SpawnPoint> spawnPoints;

    void Start()
    {
        string entranceId = DataPersistenceManager.instance.gameData.lastCheckpointId;

        foreach (var point in spawnPoints)
        {
            if (point.entranceId == entranceId)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position = point.spawnTransform.position;
                break;
            }
        }
    }
}
