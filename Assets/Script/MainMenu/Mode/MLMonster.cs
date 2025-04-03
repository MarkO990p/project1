using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class MLMonster : Agent
{
    [SerializeField] private int monsterType = 1;
    [SerializeField] private BehaviorParameters behaviorParameters;
    [SerializeField] private Health healthComponent;

    private float baseSpeed;
    private float baseDamage;

    public override void Initialize()
    {
        base.Initialize();
        baseSpeed = behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly ? 5f : 3f;
        baseDamage = 10f;
        UpdateDifficultySettings();
    }

    public void UpdateDifficultySettings()
    {
        // อัพเดทโมเดล ML-Agent
        behaviorParameters.Model = MLDifficultyManager.Instance.GetModelForMonsterType(monsterType);

        // อัพเดทค่าพลังชีวิต
        if (healthComponent != null)
        {
            healthComponent.SetMaxHealth(MLDifficultyManager.Instance.GetHealthForMonsterType(monsterType));
            healthComponent.ResetHealth();
        }
    }

    public override void OnEpisodeBegin()
    {
        UpdateDifficultySettings();
        // รีเซ็ตตำแหน่งและสถานะอื่นๆ
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // เก็บข้อมูลสถานะของมอนสเตอร์
        sensor.AddObservation(healthComponent.currentHealth / healthComponent.MaxHealth);
        // เพิ่ม observations อื่นๆ ตามต้องการ
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // ประมวลผลการกระทำจากโมเดล
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        bool shouldAttack = actions.DiscreteActions[0] == 1;

        // เคลื่อนที่
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;
        transform.Translate(movement * (baseSpeed * Time.deltaTime));

        // โจมตีถ้าต้องการ
        if (shouldAttack)
        {
            // เรียกฟังก์ชันโจมตี
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // สำหรับการควบคุมด้วยมือ (ถ้าต้องการทดสอบ)
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}