using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PushAgent2D : Agent
{
    [Header("References")]
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float pushForce = 8f;
    public float pushRadius = 1.2f;
    public float maxPushPower = 15f;

    [Header("Reward Settings")]
    public float pushRewardMultiplier = 0.2f;
    public float distancePenalty = 0.001f;
    public float timePenalty = 0.001f;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private bool isGrounded;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb == null)
        {
            Debug.LogError("Player missing Rigidbody2D component!");
        }
    }

    public override void OnEpisodeBegin()
    {
        // กำหนดตำแหน่งเริ่มต้นแบบสุ่ม
        transform.localPosition = new Vector2(Random.Range(-4f, 4f), 0f);
        player.localPosition = new Vector2(Random.Range(-4f, 4f), 0f);

        // รีเซ็ตความเร็ว
        rb.velocity = Vector2.zero;
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ข้อมูลตำแหน่งและทิศทาง
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(player.localPosition);
        sensor.AddObservation((player.position - transform.position).normalized);

        // ข้อมูลความเร็ว
        sensor.AddObservation(rb.velocity);
        if (playerRb != null)
        {
            sensor.AddObservation(playerRb.velocity);
        }
        else
        {
            sensor.AddObservation(Vector2.zero);
        }

        // สถานะพื้น
        sensor.AddObservation(IsGrounded() ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // การเคลื่อนที่
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        // ตรวจสอบการผลัก
        if (IsPlayerInPushRange())
        {
            PushPlayer();
        }

        // ให้รางวัล/ค่าปรับ
        AddReward(-CalculateDistancePenalty());
        AddReward(-timePenalty);
    }

    private float CalculateDistancePenalty()
    {
        return Vector2.Distance(transform.position, player.position) * distancePenalty;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsPlayerInPushRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, pushRadius, playerLayer);
        return hit != null && hit.transform == player;
    }

    private void PushPlayer()
    {
        if (playerRb == null) return;

        Vector2 pushDir = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        // คำนวณแรงผลักตามระยะทาง (ยิ่งใกล้ยิ่งแรง)
        float pushPower = Mathf.Clamp(pushForce / distance, 0, maxPushPower);

        playerRb.AddForce(pushDir * pushPower, ForceMode2D.Impulse);

        // ให้รางวัลตามความแรงของการผลัก
        AddReward(pushPower * pushRewardMultiplier);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    // สำหรับ Debug วาดระยะผลักใน Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pushRadius);
    }
}