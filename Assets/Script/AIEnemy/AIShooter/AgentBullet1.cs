using UnityEngine;

public class AgentBullet1 : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public int damage = 10;
    public EnemyShooterAgent ownerAgent;
    public bool facingCorrect = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // ❌ ลบ Update ออก เพราะมัน override ความเร็วที่ตั้งไว้
    /*
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20f);

                if (playerHealth.GetCurrentHealth() <= 0f && ownerAgent != null)
                {
                    ownerAgent.SetReward(1.0f);
                    ownerAgent.EndEpisode();
                }
                else if (ownerAgent != null)
                {
                    ownerAgent.AddReward(0.1f);
                }
            }

            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy"))
        {
            if (ownerAgent != null)
            {
                ownerAgent.AddReward(-0.3f);
            }

            Destroy(gameObject);
        }
    }
}
