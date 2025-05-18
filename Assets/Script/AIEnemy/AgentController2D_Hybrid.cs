using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentController2D_Hybrid : Agent
{
    [Header("References")]
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Zone Settings")]
    public Vector2 zoneCenter = new Vector2(0, 0);
    public Vector2 zoneSize = new Vector2(6, 3);

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackRange = 1.0f;
    public float attackCooldown = 1.0f;

    private float lastAttackTime = 0f;
    private float zoneMinX;
    private float zoneMaxX;

    private Rigidbody2D rb;
    private float previousDistance;
    private int patrolDirection = 1;

    private enum AgentState { Patrol, Chasing, Attacking }
    private AgentState currentState = AgentState.Patrol;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        float halfWidth = zoneSize.x / 2f;
        zoneMinX = zoneCenter.x - halfWidth;
        zoneMaxX = zoneCenter.x + halfWidth;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector2(zoneCenter.x - 1f, zoneCenter.y);
        rb.velocity = Vector2.zero;
        currentState = AgentState.Patrol;
        patrolDirection = 1;
        previousDistance = Vector2.Distance(transform.position, player.position);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        lastAttackTime = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x / 10f);
        sensor.AddObservation(transform.position.y / 5f);
        sensor.AddObservation(player.position.x / 10f);
        sensor.AddObservation(player.position.y / 5f);
        sensor.AddObservation(Vector2.Distance(transform.position, player.position) / 10f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        UpdateState();

        switch (currentState)
        {
            case AgentState.Patrol:
                Patrol();
                break;
            case AgentState.Chasing:
                ChasePlayer();
                break;
            case AgentState.Attacking:
                AttackPlayer();
                break;
        }

        AddReward(-0.001f);
        FlipToFacePlayer();
    }

    private void Patrol()
    {
        float nextX = transform.position.x + (patrolDirection * moveSpeed * Time.fixedDeltaTime);
        if (nextX < zoneMinX || nextX > zoneMaxX)
        {
            patrolDirection *= -1;
        }
        Move(patrolDirection);
    }

    private void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Move(direction);

        float currentDistance = Vector2.Distance(transform.position, player.position);
        AddReward((previousDistance - currentDistance) * 0.01f);
        previousDistance = currentDistance;
    }

    private void AttackPlayer()
    {
        // หยุดการเคลื่อนไหว
        rb.velocity = Vector2.zero;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"[Agent Dagger] Attacked {player.name}, dealt {attackDamage} damage.");
                AddReward(1f);
                lastAttackTime = Time.time;
            }
        }
    }

    private void Move(float direction)
    {
        float newX = transform.position.x + (direction * moveSpeed * Time.fixedDeltaTime);
        newX = Mathf.Clamp(newX, zoneMinX, zoneMaxX);
        transform.position = new Vector2(newX, transform.position.y);
    }

    private void FlipToFacePlayer()
    {
        if (player == null) return;

        bool facingRight = player.position.x > transform.position.x;

        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ca = actionsOut.ContinuousActions;
        ca[0] = 0f;
    }

    private void UpdateState()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        float verticalDiff = Mathf.Abs(player.position.y - transform.position.y);

        if (!IsGrounded())
        {
            currentState = AgentState.Patrol;
            return;
        }

        if (distance < attackRange && verticalDiff < 1.0f)
        {
            currentState = AgentState.Attacking;
        }
        else if (distance < 10f && verticalDiff < 1.0f)
        {
            currentState = AgentState.Chasing;
        }
        else
        {
            currentState = AgentState.Patrol;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Vector3 size = new Vector3(zoneSize.x, zoneSize.y, 0.1f);
        Vector3 center = new Vector3(zoneCenter.x, zoneCenter.y, 0f);

        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(center, size);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
}
