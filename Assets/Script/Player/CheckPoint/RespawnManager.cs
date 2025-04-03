using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public void RespawnPlayer()
    {
        Time.timeScale = 1f; // ให้เกมเล่นต่อ
        GameManager.instance.RespawnPlayer();
    }
}
