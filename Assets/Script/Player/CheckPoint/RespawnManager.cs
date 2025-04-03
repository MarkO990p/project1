using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory; // ✅ สำหรับ InventoryController

public class RespawnManager : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private List<Transform> respawnPoints = new List<Transform>();
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private GameObject player;

    [Header("Inventory Reference")]
    [SerializeField] private InventoryController inventoryController; // ✅ เพิ่มการเชื่อม Inventory

    private Transform currentRespawnPoint;

    private SpriteRenderer playerSpriteRenderer;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    private PlayerController playerController;
    private PlayerShooting playerShooting;
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
            playerShooting = player.GetComponent<PlayerShooting>();
            playerHealth = player.GetComponent<Health>();
            playerMalee = player.GetComponent<MaleAttack>();
        }
        else
        {
            Debug.LogError("Player GameObject is not assigned in RespawnManager!");
        }

        if (respawnPoints.Count > 0)
        {
            currentRespawnPoint = respawnPoints[0]; // ตั้งค่า default
        }

        if (inventoryController == null)
        {
            Debug.LogWarning("InventoryController is not assigned in RespawnManager.");
        }
    }

    public void UpdateRespawnPoint(Transform newCheckpoint)
    {
        if (respawnPoints.Contains(newCheckpoint))
        {
            currentRespawnPoint = newCheckpoint;
        }
        else
        {
            Debug.LogWarning("Checkpoint ที่ส่งมาไม่ได้อยู่ในรายการ Respawn Points");
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

        if (currentRespawnPoint == null)
        {
            Debug.LogError("Current respawn point is not set!");
            yield break;
        }

        // ย้ายผู้เล่นกลับไปยังตำแหน่ง checkpoint
        player.transform.position = currentRespawnPoint.position;
        playerAnimator.SetTrigger("idle");
        playerRb.bodyType = RigidbodyType2D.Dynamic;

        // เปิดใช้งานคอนโทรลต่างๆ
        if (playerController != null) playerController.enabled = true;
        if (playerShooting != null) playerShooting.enabled = true;
        if (playerHealth != null) playerHealth.ResetHealth();
        if (playerMalee != null) playerMalee.enabled = true;

        playerSpriteRenderer.color = Color.white;

        // ✅ ซิงค์ Inventory UI อีกครั้งหลัง Respawn
        if (inventoryController != null)
        {
            var inventoryData = inventoryController.InventoryData;
            var inventoryUI = inventoryController.InventoryUI;

            if (inventoryData != null && inventoryUI != null)
            {
                inventoryUI.ResetAllItems();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
                }
            }
        }
    }
}
