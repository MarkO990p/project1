using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PushAgent2D : Agent
{
    [Header("References")]
    public Transform player;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float pushForce = 8f;
    public float pushDistanceThreshold = 1.2f;
    public float maxPushPower = 15f;

    [Header("Reward Settings")]
    public float pushRewardMultiplier = 0.2f;
    public float distancePenalty = 0.001f;
    public float timePenalty = 0.001f;

    [Header("Environment Checks")]
    public float wallCheckDistance = 0.5f;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private float lastDistanceToPlayer;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector2.zero;
        if (playerRb != null) playerRb.velocity = Vector2.zero;
        lastDistanceToPlayer = Vector2.Distance(transform.position, player.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);                                 // 2
        sensor.AddObservation(player.position);                                    // 2
        sensor.AddObservation((player.position - transform.position).normalized);  // 2
        sensor.AddObservation(rb.velocity);                                        // 2
        sensor.AddObservation(playerRb != null ? playerRb.velocity : Vector2.zero);// 2
        sensor.AddObservation(IsGrounded() ? 1f : 0f);                             // 1
        sensor.AddObservation(DistanceToWall());                                   // 1
        sensor.AddObservation(player.position.x - transform.position.x);           // 1 ← บอกว่าผู้เล่นอยู่ทางไหน

        // รวม = 13 observations
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // ✅ ให้รางวัลถ้าเข้าใกล้ player มากขึ้น
        if (distToPlayer < lastDistanceToPlayer)
        {
            AddReward(0.01f);
        }
        else
        {
            AddReward(-0.005f); // ถ้าห่างออก
        }

        // ✅ ติดกำแพง + ยังเดินทางเดิม = ปรับ
        if (IsWallAhead() && Mathf.Abs(moveX) > 0.1f)
        {
            AddReward(-0.02f);
        }

        // ✅ ยืนนิ่งไม่ขยับ = ปรับเล็กน้อย
        if (rb.velocity.magnitude < 0.01f)
        {
            AddReward(-0.002f);
        }

        lastDistanceToPlayer = distToPlayer;

        // ✅ ถ้าอยู่ใกล้ player → ผลักได้
        if (distToPlayer <= pushDistanceThreshold)
        {
            PushPlayer(distToPlayer);
        }

        AddReward(-timePenalty);
        AddReward(-CalculateDistancePenalty());
    }

    private void PushPlayer(float distance)
    {
        if (playerRb == null) return;

        Vector2 pushDir = (player.position - transform.position).normalized;
        float pushPower = Mathf.Clamp(pushForce / Mathf.Max(distance, 0.1f), 0, maxPushPower);
        playerRb.AddForce(pushDir * pushPower, ForceMode2D.Impulse);

        AddReward(pushPower * pushRewardMultiplier);
        AddReward(1f); // สำเร็จ!
        EndEpisode();
    }

    private float CalculateDistancePenalty()
    {
        return Vector2.Distance(transform.position, player.position) * distancePenalty;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWallAhead()
    {
        if (wallCheck == null) return false;
        return Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);
    }

    private float DistanceToWall()
    {
        if (wallCheck == null) return wallCheckDistance;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);
        return hit.collider != null ? hit.distance : wallCheckDistance;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, player.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pushDistanceThreshold);

        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * wallCheckDistance);
        }
    }
}
