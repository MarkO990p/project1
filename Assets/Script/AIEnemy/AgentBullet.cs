using UnityEngine;

public class AgentBullet : MonoBehaviour
{
    public AgentShot shooter;
    public float lifetime = 3f;
    public int damage = 10;

    [SerializeField] private LayerMask groundLayer; // <-- เพิ่มตรงนี้

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            shooter.AddReward(1f);
            shooter.EndEpisode();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            shooter.AddReward(-1f);
            shooter.EndEpisode();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (((1 << other.gameObject.layer) & groundLayer) != 0) // <-- ตรวจว่าโดน Layer "Ground"
        {
            Destroy(gameObject); // โดนพื้นก็หายไป
        }
    }
}
