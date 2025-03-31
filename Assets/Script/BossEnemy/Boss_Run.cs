using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float attackRange = 3f;

    Transform player;
    Rigidbody2D rb;
    Boss boss;  // เพิ่มการอ้างอิงไปยัง Boss script

    // เริ่มต้นเมื่อเข้าสู่สถานะ
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
    }

    // เรียกใช้งานทุก frame ในระหว่างที่บอสอยู่ในสถานะนี้
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!boss.IsActive) return;  // ตรวจสอบว่า บอสถูกเปิดใช้งานหรือไม่

        LookAtPlayer(animator);  // ให้บอสหันไปทางผู้เล่น

        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);  // ให้บอสวิ่งไปที่ตำแหน่งผู้เล่น

        // เมื่อระยะห่างระหว่างบอสกับผู้เล่นน้อยกว่าระยะโจมตี
        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("Attack");  // เรียกใช้การโจมตี
        }
    }

    // เมื่อออกจากสถานะนี้
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");  // รีเซ็ต Trigger การโจมตี
    }

    // ฟังก์ชันให้บอสหันหน้าไปหาผู้เล่น
    private void LookAtPlayer(Animator animator)
    {
        Vector3 scale = animator.gameObject.transform.localScale;

        // พลิกทิศทางบอสไปทางซ้ายหรือขวาตามตำแหน่งของผู้เล่น
        if (animator.gameObject.transform.position.x > player.position.x && scale.x > 0)  // ถ้าผู้เล่นอยู่ทางซ้าย
        {
            scale.x *= -1;  // พลิกไปทางซ้าย
            Debug.Log("Flipping to Left");
        }
        else if (animator.gameObject.transform.position.x < player.position.x && scale.x < 0)  // ถ้าผู้เล่นอยู่ทางขวา
        {
            scale.x *= -1;  // พลิกไปทางขวา
            Debug.Log("Flipping to Right");
        }

        animator.gameObject.transform.localScale = scale;  // อัปเดต localScale เพื่อให้บอสพลิกทิศทาง
    }
}
