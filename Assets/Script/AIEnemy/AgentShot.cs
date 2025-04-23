using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentShot : Agent
{
    public Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float shootCooldown = 1f;
    public float bulletSpeed = 15f;
    public LayerMask obstacleLayer;

    private float cooldownTimer = Mathf.Infinity;
    private float timeAlive = 0f;

    public override void OnEpisodeBegin()
    {
        // Reset ตำแหน่ง Agent และเป้า
        transform.localPosition = new Vector2(Random.Range(-5f, 5f), 0f);
        player.localPosition = new Vector2(Random.Range(-5f, 5f), 0f);
        cooldownTimer = Mathf.Infinity;
        timeAlive = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);
        bool clearShot = !Physics2D.Raycast(transform.position, dir, dist, obstacleLayer);

        sensor.AddObservation(dir.x);
        sensor.AddObservation(dist);
        sensor.AddObservation(cooldownTimer >= shootCooldown ? 1f : 0f);
        sensor.AddObservation(clearShot ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        cooldownTimer += Time.deltaTime;
        timeAlive += Time.deltaTime;

        float shoot = actions.ContinuousActions[0];

        if (shoot > 0.5f && cooldownTimer >= shootCooldown && TargetVisible())
        {
            Shoot();
            cooldownTimer = 0f;
        }

        AddReward(-0.001f * timeAlive); // ลงโทษหากช้า
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = (player.position - firePoint.position).normalized * bulletSpeed;

        var bulletScript = bullet.GetComponent<AgentBullet>();
        bulletScript.shooter = this;
    }

    private bool TargetVisible()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);
        return !Physics2D.Raycast(transform.position, dir, dist, obstacleLayer);
    }
}