using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class EnemyAgent : Agent
{
    public Transform player; // ตำแหน่งของผู้เล่น
    public Rigidbody2D rb;
    public float speed = 2f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    private bool isGrounded;

    public override void OnEpisodeBegin()
    {
        // รีเซ็ตตำแหน่งของศัตรูเมื่อเริ่มรอบใหม่
        transform.position = new Vector3(Random.Range(-3f, 3f), transform.position.y, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // เก็บข้อมูลตำแหน่งศัตรูและผู้เล่น
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(player.position.x);
        sensor.AddObservation(Vector2.Distance(transform.position, player.position));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];

        // ศัตรูเดินไปทางซ้าย (-1) หรือขวา (+1)
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        // ตรวจสอบว่าศัตรูอยู่บนพื้นหรือไม่
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        // ให้ AI กระโดดเมื่ออยู่บนพื้น
        if (isGrounded && Random.value > 0.9f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // เพิ่ม Reward ถ้าศัตรูเข้าใกล้ผู้เล่น
        float distance = Mathf.Abs(transform.position.x - player.position.x);
        if (distance < 2f) AddReward(0.1f);
        else AddReward(-0.1f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal"); // ให้ศัตรูใช้ปุ่มซ้าย-ขวาแบบ manual
    }
}