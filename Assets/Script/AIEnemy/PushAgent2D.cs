using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

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
    public float jumpForce = 7f;

    [Header("Push Settings")]
    public float pushForce = 8f;
    public float pushDistanceThreshold = 1.2f;
    public float maxPushPower = 15f;

    [Header("Reward Settings")]
    public float pushRewardMultiplier = 0.2f;
    public float distancePenalty = 0.001f;
    public float timePenalty = 0.001f;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private float lastDistanceToPlayer;

    private Vector2 lookDirection = Vector2.right; // ✅ ทิศทางที่ Agent หันหน้า

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
        sensor.AddObservation(transform.position);
        sensor.AddObservation(player.position);
        sensor.AddObservation((player.position - transform.position).normalized);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(playerRb != null ? playerRb.velocity : Vector2.zero);
        sensor.AddObservation(IsGrounded() ? 1f : 0f);
        sensor.AddObservation(DistanceToWall());
        sensor.AddObservation(player.position.x - transform.position.x);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0]; // 0=idle, 1=left, 2=right
        int jumpAction = actions.DiscreteActions[1]; // 0=ไม่กระโดด, 1=กระโดด

        float moveX = 0f;
        if (moveAction == 1) moveX = -1f;
        else if (moveAction == 2) moveX = 1f;

        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        //Debug.Log($"[Agent Push Action] MoveAction: {moveAction}, JumpAction: {jumpAction}, MoveX: {moveX}, Velocity: {rb.velocity}");

        // ✅ อัปเดตทิศทางที่หัน
        if (moveX != 0)
            lookDirection = new Vector2(moveX, 0f);

        // ✅ กระโดดเฉพาะเมื่อเจอ Obstacle
        if (jumpAction == 1 && IsGrounded() && IsObstacleAhead())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            AddReward(0.02f);

            Debug.Log($"[Jump] Agent Push jumped at position {transform.position}, velocity: {rb.velocity}");
        }

        // ❌ ลงโทษถ้ากระโดดโดยไม่เจอ Obstacle
        if (jumpAction == 1 && !IsObstacleAhead())
        {
            AddReward(-0.01f);
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < lastDistanceToPlayer)
            AddReward(0.01f);
        else
            AddReward(-0.005f);

        if (IsWallAhead() && Mathf.Abs(moveX) > 0.1f)
            AddReward(-0.02f);

        if (rb.velocity.magnitude < 0.01f)
            AddReward(-0.002f);

        lastDistanceToPlayer = distToPlayer;

        if (distToPlayer <= pushDistanceThreshold)
            PushPlayer(distToPlayer);

        AddReward(-timePenalty);
        AddReward(-CalculateDistancePenalty());

        // ✅ Flip หันหน้าตามผู้เล่น
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void PushPlayer(float distance)
    {
        if (playerRb == null) return;

        Vector2 pushDir = (player.position - transform.position).normalized;
        float pushPower = Mathf.Clamp(pushForce / Mathf.Max(distance, 0.1f), 0, maxPushPower);
        playerRb.AddForce(pushDir * pushPower, ForceMode2D.Impulse);

        AddReward(pushPower * pushRewardMultiplier);
        AddReward(1f);
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
        return Physics2D.Raycast(wallCheck.position, lookDirection, 0.5f, wallLayer);
    }

    private float DistanceToWall()
    {
        if (wallCheck == null) return 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, lookDirection, 0.5f, wallLayer);
        return hit.collider != null ? hit.distance : 0.5f;
    }

    private bool IsObstacleAhead()
    {
        if (wallCheck == null) return false;
        Debug.DrawRay(wallCheck.position, lookDirection * 0.6f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, lookDirection, 0.6f, wallLayer);
        return hit.collider != null && hit.collider.CompareTag("Obstacle");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.LeftArrow)) discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.RightArrow)) discreteActions[0] = 2;
        else discreteActions[0] = 0;

        discreteActions[1] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
