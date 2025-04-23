using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentController2D : Agent
{
    [Header("References")]
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private bool isJumping;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector2(Random.Range(-5f, 5f), 0f);
        player.localPosition = new Vector2(Random.Range(-5f, 5f), 0f);
        rb.velocity = Vector2.zero;
        isJumping = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 directionToPlayer = (player.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(directionToPlayer.x); // 1
        sensor.AddObservation(IsGrounded() ? 1f : 0f); // 1
        //sensor.AddObservation(IsObstacleAhead() ? 1f : 0f); // 1
        //sensor.AddObservation(IsPlayerInAttackRange() ? 1f : 0f); // 1
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float jump = Mathf.Clamp(actions.ContinuousActions[1], 0f, 1f);
        float attack = Mathf.Clamp(actions.ContinuousActions[2], 0f, 1f);

        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        if (jump > 0.5f && IsGrounded() && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }

        if (IsGrounded())
        {
            isJumping = false;
        }

        if (attack > 0.5f && IsPlayerInAttackRange())
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // คำนวณทิศทางไปยังผู้เล่น
        Vector2 directionToPlayer = (player.localPosition - transform.localPosition).normalized;

        // ให้รางวัลถ้าเข้าใกล้ผู้เล่น
        float distanceBefore = Vector2.Distance(transform.localPosition, player.localPosition);
        float reward = (distanceBefore - Vector2.Distance(transform.localPosition, player.localPosition)) * 0.01f;
        AddReward(reward);

        // ลงโทษถ้าเดินผิดทาง
        if (Mathf.Sign(moveX) != Mathf.Sign(directionToPlayer.x))
        {
            AddReward(-0.01f); // ลงโทษถ้าเดินหนี
        }

        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Horizontal");
        ca[1] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        ca[2] = Input.GetKey(KeyCode.Z) ? 1f : 0f;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private bool IsObstacleAhead()
    {
        Vector2 dir = Vector2.right * Mathf.Sign(player.position.x - transform.position.x);
        return Physics2D.Raycast(groundCheck.position, dir, 1f, obstacleLayer);
    }

    private bool IsPlayerInAttackRange()
    {
        return Vector2.Distance(transform.position, player.position) < 1.5f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AddReward(-1f); // ถูกโจมตี
            EndEpisode();
        }
    }
}