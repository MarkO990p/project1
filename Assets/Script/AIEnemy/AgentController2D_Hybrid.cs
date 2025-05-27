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
    public LayerMask playerLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Zone Settings")]
    public Vector2 zoneCenter = new Vector2(0, 0);
    public Vector2 zoneSize = new Vector2(6, 3);

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackRange = 1.0f;
    public float attackCooldown = 1.0f;

    [Header("Detection Settings")]
    public float visionDistance = 3f;

    private float attackCooldownTimer = 0f;
    private float zoneMinX;
    private float zoneMaxX;

    private Rigidbody2D rb;
    private Animator animator;
    private float previousDistance;
    private int patrolDirection = 1;
    private Vector2 lastPosition;
    private float idleTime = 0f;
    private bool previouslySawPlayer = false;
    private int successfulHits = 0;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        float halfWidth = zoneSize.x / 2f;
        zoneMinX = zoneCenter.x - halfWidth;
        zoneMaxX = zoneCenter.x + halfWidth;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector2(zoneCenter.x - 1f, zoneCenter.y);
        rb.velocity = Vector2.zero;
        patrolDirection = 1;
        previousDistance = Vector2.Distance(transform.position, player.position);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        attackCooldownTimer = 0f;
        idleTime = 0f;
        lastPosition = transform.position;
        previouslySawPlayer = false;
        successfulHits = 0;

        if (animator != null)
        {
            animator.SetBool("moving", false);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 agentPos = transform.position;
        Vector2 playerPos = player.position;
        Vector2 diff = playerPos - agentPos;

        sensor.AddObservation(agentPos.x / 10f);
        sensor.AddObservation(agentPos.y / 5f);
        sensor.AddObservation(playerPos.x / 10f);
        sensor.AddObservation(playerPos.y / 5f);
        sensor.AddObservation(diff.magnitude / 10f);
        sensor.AddObservation(diff.x);
        sensor.AddObservation(diff.y);
        sensor.AddObservation(IsPlayerInZone() ? 1f : 0f);
        sensor.AddObservation(CanSeePlayer() ? 1f : 0f);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (!IsPlayerInZone())
        {
            actionMask.SetActionEnabled(0, 1, false); // Chase
            actionMask.SetActionEnabled(0, 2, false); // Attack
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!IsGrounded())
        {
            AddReward(-0.005f);
            return;
        }

        attackCooldownTimer -= Time.fixedDeltaTime;

        int action = actions.DiscreteActions[0];
        bool playerInZone = IsPlayerInZone();
        bool canSee = CanSeePlayer();

        float currentDistance = Vector2.Distance(transform.position, player.position);
        float distanceDelta = previousDistance - currentDistance;

        // ให้รางวัลตามระยะที่เข้าใกล้ player
        AddReward(Mathf.Clamp(distanceDelta * 0.05f, -0.02f, 0.05f));

        // เพิ่มรางวัลตามการอยู่ใกล้ player
        float proximityReward = Mathf.Clamp(1f - (currentDistance / visionDistance), 0f, 1f) * 0.02f;
        AddReward(proximityReward);

        // รางวัลเมื่อเห็น player ครั้งแรก
        if (!previouslySawPlayer && canSee)
        {
            AddReward(0.05f);
        }
        previouslySawPlayer = canSee;

        switch (action)
        {
            case 0: // Patrol
                Patrol();
                if (!playerInZone)
                {
                    AddReward(0.02f); // ดีที่ patrol ขณะ player อยู่นอกโซน
                    if (currentDistance > 5f) AddReward(0.01f);
                }
                else
                {
                    AddReward(-0.02f); // ผิดที่ patrol ขณะ player อยู่ในโซน
                }
                break;

            case 1: // Chase
                if (playerInZone)
                {
                    ChasePlayer();
                    AddReward(canSee ? 0.03f : -0.01f);
                }
                else
                {
                    Patrol();
                    AddReward(-0.05f);
                }
                break;

            case 2: // Attack
                if (playerInZone)
                {
                    AttackPlayer();
                }
                else
                {
                    AddReward(-0.1f); // ผิดที่โจมตีนอกโซน
                }
                break;
        }

        // ลงโทษถ้าอยู่นิ่งนานเกินไป
        if (Vector2.Distance(transform.position, lastPosition) < 0.1f)
        {
            idleTime += Time.fixedDeltaTime;
            if (idleTime > 1.5f)
                AddReward(-0.01f);
        }
        else
        {
            idleTime = 0f;
        }
        lastPosition = transform.position;

        AddReward(-0.001f); // time penalty
        previousDistance = currentDistance;
    }

    private void Patrol()
    {
        float posX = transform.position.x;
        float nextX = posX + patrolDirection * moveSpeed * Time.fixedDeltaTime;

        if (nextX < zoneMinX || nextX > zoneMaxX)
        {
            patrolDirection *= -1;
        }

        Move(patrolDirection);
        FlipToPatrolDirection();
    }

    private void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Move(direction);
        FlipToFacePlayer();
    }

    private void AttackPlayer()
    {
        rb.velocity = Vector2.zero;

        float distance = Vector2.Distance(transform.position, player.position);
        bool facingWrongWay = (transform.position.x - player.position.x) * transform.localScale.x > 0;

        if (distance <= attackRange && attackCooldownTimer <= 0f && CanSeePlayer())
        {
            if (facingWrongWay)
            {
                AddReward(-0.1f); // โจมตีหันผิดด้าน
                return;
            }

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                if (animator != null)
                {
                    animator.SetTrigger("meleeAttack");
                    animator.SetBool("moving", false);
                }

                playerHealth.TakeDamage(attackDamage);
                successfulHits++;
                AddReward(0.8f + (0.2f * successfulHits));
                attackCooldownTimer = attackCooldown;
            }
        }
        else
        {
            AddReward(-0.02f); // โจมตีพลาด
        }
    }

    private void Move(float direction)
    {
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        if (animator != null)
        {
            animator.SetBool("moving", Mathf.Abs(direction) > 0.01f);
        }
    }

    private void FlipToFacePlayer()
    {
        if (player == null) return;

        bool facingRight = player.position.x > transform.position.x;
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void FlipToPatrolDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x = patrolDirection > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private bool IsPlayerInZone()
    {
        Vector2 playerPos = player.position;
        Vector2 min = zoneCenter - (zoneSize / 2f);
        Vector2 max = zoneCenter + (zoneSize / 2f);

        return playerPos.x >= min.x && playerPos.x <= max.x &&
               playerPos.y >= min.y && playerPos.y <= max.y;
    }

    private bool CanSeePlayer()
    {
        Vector2 origin = transform.position;
        Vector2 direction = (player.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionDistance, playerLayer);
        return hit.collider != null && hit.collider.transform == player;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var da = actionsOut.DiscreteActions;
        da[0] = 0; // Default: Patrol
    }

    private void OnDrawGizmos()
    {
        Vector3 size = new Vector3(zoneSize.x, zoneSize.y, 0.1f);
        Vector3 center = new Vector3(zoneCenter.x, zoneCenter.y, 0f);

        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);

        if (player != null)
        {
            Gizmos.color = Color.red;
            Vector2 dir = (player.position - transform.position).normalized;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(dir * visionDistance));
        }
    }
}
