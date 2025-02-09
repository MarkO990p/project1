using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private GameObject player;

    private SpriteRenderer playerSpriteRenderer;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    private PlayerController playerController;
    private PlayerShooting playerShooting; // เพิ่มตัวแปรสำหรับ PlayerShooting
    private Health playerHealth;
    private MaleAttack playerMalee;

    private void Awake()
    {
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
            playerAnimator = player.GetComponent<Animator>();
            playerController = player.GetComponent<PlayerController>();
            playerShooting = player.GetComponent<PlayerShooting>(); // อ้างอิง PlayerShooting
            playerHealth = player.GetComponent<Health>();
            playerMalee = player.GetComponent<MaleAttack>();
        }
        else
        {
            Debug.LogError("Player GameObject is not assigned in RespawnManager!");
        }
    }

    public void RespawnPlayer()
    {
        if (player != null)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        player.transform.position = respawnPoint.position;
        playerAnimator.SetTrigger("idle");
        playerRb.bodyType = RigidbodyType2D.Dynamic;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (playerShooting != null) // ตรวจสอบและเปิดใช้งาน PlayerShooting หลังจาก Respawn
        {
            playerShooting.enabled = true;
        }

        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        if (playerMalee != null) // ตรวจสอบและเปิดใช้งาน MaleAttack หลังจาก Respawn
    {
        playerMalee.enabled = true;
    }

        playerSpriteRenderer.color = Color.white;
    }

    public void UpdateRespawnPoint(Transform newCheckpoint)
    {
        respawnPoint = newCheckpoint;
    }
}
