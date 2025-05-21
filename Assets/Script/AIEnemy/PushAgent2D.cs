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

    [Header("Direction Lock Settings")]
    public float moveLockDuration = 0.5f;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private Animator anim;

    private float lastDistanceToPlayer;

    private Vector2 lookDirection = Vector2.right;

    private int currentMoveDirection = 0; // 0=idle, 1=left, 2=right
    private float moveLockTimer = 0f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector2.zero;
        if (playerRb != null) playerRb.velocity = Vector2.zero;
        lastDistanceToPlayer = Vector2.Distance(transform.position, player.position);
        currentMoveDirection = 0;
        moveLockTimer = 0f;
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

        // ✅ ปลดล็อกทันทีถ้าผู้เล่นอยู่คนละฝั่ง
        float dirToPlayer = player.position.x - transform.position.x;
        float signToPlayer = Mathf.Sign(dirToPlayer);
        float signMoving = currentMoveDirection == 1 ? -1f : (currentMoveDirection == 2 ? 1f : 0f);

        if (moveLockTimer > 0f && signToPlayer != 0f && signToPlayer != signMoving)
        {
            moveLockTimer = 0f;
        }

        // ✅ ล็อกทิศทางใหม่เมื่อหมดเวลา หรือเปลี่ยนใจ
        if (moveAction != 0 && moveAction != currentMoveDirection && moveLockTimer <= 0f)
        {
            currentMoveDirection = moveAction;
            moveLockTimer = moveLockDuration;
        }

        // ✅ นับถอยหลังล็อก
        if (moveLockTimer > 0f)
        {
            moveLockTimer -= Time.deltaTime;
        }

        // ✅ เดินตาม currentMoveDirection
        float moveX = 0f;
        if (currentMoveDirection == 1) moveX = -1f;
        else if (currentMoveDirection == 2) moveX = 1f;

        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        // ✅ อัปเดตทิศที่หัน
        if (moveX != 0)
            lookDirection = new Vector2(moveX, 0f);

        // ✅ กระโดดเฉพาะเมื่อเจอ obstacle
        if (jumpAction == 1 && IsGrounded() && IsObstacleAhead())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            AddReward(0.02f);
            Debug.Log($"[Jump] Agent Push jumped at position {transform.position}, velocity: {rb.velocity}");
        }

        // ❌ โดดโดยไม่มี obstacle → ลงโทษ
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

        // ✅ Flip sprite ตามตำแหน่งผู้เล่น
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // ✅ เล่นแอนิเมชันเดิน
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.05f;
        anim.SetBool("moving", isMoving);
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
