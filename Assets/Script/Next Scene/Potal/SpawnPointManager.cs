using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public string entranceId;
        public Transform spawnTransform;
    }

    public SpawnPoint[] spawnPoints;

    void Start()
    {
        string entranceId = PlayerPrefs.GetString("EntranceID", "");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        foreach (var point in spawnPoints)
        {
            if (point.entranceId == entranceId)
            {
                player.transform.position = point.spawnTransform.position;
                return;
            }
        }

        Debug.LogWarning("No spawn point matched entrance ID: " + entranceId);
    }
}
