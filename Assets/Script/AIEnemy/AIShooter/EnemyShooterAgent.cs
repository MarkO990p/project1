using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class EnemyShooterAgent : Agent
{
    [Header("References")]
    public Transform player;
    public Transform firePointLeft;
    public Transform firePointRight;
    public GameObject bulletPrefab;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    [Header("Cooldown Settings")]
    public float baseCooldown = 1.5f;
    public float minCooldown = 0.2f;
    private float dynamicCooldown;

    [Header("Zone (X-Axis)")]
    public float zoneXMin = 0f;
    public float zoneXMax = 0f;

    [Header("Bullet")]
    public float bulletSpeed = 10f;
    public float groundYMin = -10f;

    private float lastShootTime;
    private bool lastShootPressed = false;

    // 🔥 Combo system
    private int hitCombo = 0;
    private int comboToFastFire = 3;

    public override void OnEpisodeBegin()
    {
        if (rb != null)
            rb.velocity = Vector2.zero;

        lastShootTime = Time.time;
        dynamicCooldown = baseCooldown;
        hitCombo = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (player == null)
        {
            SetReward(-1f);
            EndEpisode();
            return;
        }

        Vector2 toPlayer = player.position - transform.position;

        sensor.AddObservation(transform.position);
        sensor.AddObservation(player.position);
        sensor.AddObservation(toPlayer.normalized);
        sensor.AddObservation(toPlayer.magnitude);
        sensor.AddObservation(player.position.x >= zoneXMin && player.position.x <= zoneXMax ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float shoot = actions.ContinuousActions[0];

        if (transform.position.y < groundYMin)
        {
            SetReward(-1f);
            EndEpisode();
            return;
        }

        if (player != null)
        {
            float dir = player.position.x - transform.position.x;
            spriteRenderer.flipX = dir < 0;
        }

        bool playerInZone = player.position.x >= zoneXMin && player.position.x <= zoneXMax;
        float dirToPlayer = player.position.x - transform.position.x;
        bool facingCorrect = (dirToPlayer > 0 && !spriteRenderer.flipX) || (dirToPlayer < 0 && spriteRenderer.flipX);

        bool shootPressed = shoot > 0.5f;
        if (shootPressed && !lastShootPressed && Time.time - lastShootTime >= dynamicCooldown && playerInZone)
        {
            TryShoot(facingCorrect);
            lastShootTime = Time.time;
        }

        lastShootPressed = shootPressed;
    }

    void TryShoot(bool facingCorrect)
    {
        bool isFacingLeft = spriteRenderer.flipX;
        Transform selectedFirePoint = isFacingLeft ? firePointLeft : firePointRight;

        GameObject bullet = Instantiate(bulletPrefab, selectedFirePoint.position, Quaternion.identity);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            float direction = isFacingLeft ? -1f : 1f;
            bulletRb.velocity = new Vector2(direction * bulletSpeed, 0f);
        }

        AgentBullet1 bulletScript = bullet.GetComponent<AgentBullet1>();
        if (bulletScript != null)
        {
            bulletScript.ownerAgent = this;
            bulletScript.facingCorrect = facingCorrect;
        }

        Debug.DrawLine(selectedFirePoint.position, selectedFirePoint.position + Vector3.right * (isFacingLeft ? -1f : 1f) * 2f, Color.red, 1f);
    }

    public void OnSuccessfulHit()
    {
        hitCombo++;
        AddReward(0.2f);

        if (hitCombo >= comboToFastFire)
        {
            dynamicCooldown = Mathf.Max(minCooldown, dynamicCooldown - 0.1f);
        }
    }

    public void OnMissedShot()
    {
        hitCombo = 0;
        dynamicCooldown = baseCooldown;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetKey(KeyCode.F) ? 1f : 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(zoneXMin, transform.position.y - 1f, 0), new Vector3(zoneXMin, transform.position.y + 2f, 0));
        Gizmos.DrawLine(new Vector3(zoneXMax, transform.position.y - 1f, 0), new Vector3(zoneXMax, transform.position.y + 2f, 0));
    }
}
