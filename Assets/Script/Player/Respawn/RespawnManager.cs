using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private GameObject player;

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
        if (respawnPoint == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (go != null)
            {
                respawnPoint = go.transform;
                Debug.Log("✅ RespawnPoint assigned from scene: " + scene.name);
            }
            else
            {
                Debug.LogWarning("⚠️ No RespawnPoint found in loaded scene: " + scene.name);
            }
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

        if (respawnPoint == null)
        {
            Debug.LogError("❌ respawnPoint is not assigned! Cancelling respawn.");
            yield break;
        }

        player.transform.position = respawnPoint.position;
        playerAnimator.SetTrigger("idle");
        playerRb.bodyType = RigidbodyType2D.Dynamic;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (playerShooting != null)
        {
            playerShooting.enabled = true;
        }

        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        if (playerMalee != null)
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
