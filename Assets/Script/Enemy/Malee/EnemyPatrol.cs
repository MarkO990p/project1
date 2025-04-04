using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;

    private void Awake()
    {
        // ตรวจสอบว่า enemy ได้ถูกตั้งค่าใน Inspector
        if (enemy == null)
        {
            Debug.LogWarning("Enemy Transform is not assigned!");
            return;
        }

        initScale = enemy.localScale;
    }

    private void OnDisable()
    {
        if (anim != null)
        {
            anim.SetBool("moving", false);
        }
    }

    private void Update()
    {
        // ตรวจสอบว่า enemy, leftEdge และ rightEdge ถูกกำหนดใน Inspector ก่อนทำงาน
        if (enemy == null || leftEdge == null || rightEdge == null)
        {
            // Debug.LogWarning("Patrol points or enemy are not assigned!");
            return;
        }

        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    private void DirectionChange()
    {
        if (anim != null)
        {
            anim.SetBool("moving", false);
        }
        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
            movingLeft = !movingLeft;
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;

        if (anim != null)
        {
            anim.SetBool("moving", true);
        }

        // Make enemy face direction
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
            initScale.y, initScale.z);

        // Move in that direction
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }
}
