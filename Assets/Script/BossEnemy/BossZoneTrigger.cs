using UnityEngine;

public class BossZoneTrigger : MonoBehaviour
{
    public GameObject boss;
    public DoorCon leftDoor;  // อ้างอิงไปยัง DoorCon ของประตูซ้าย
    public DoorCon rightDoor; // อ้างอิงไปยัง DoorCon ของประตูขวา

    private Boss bossScript;

    private void Start()
    {
        bossScript = boss.GetComponent<Boss>();
        bossScript.enabled = false;
        Debug.Log("BossZoneTrigger initialized and boss script disabled.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the boss area.");
            CloseDoors();

            if (bossScript != null)
            {
                bossScript.enabled = true;
                Debug.Log("Boss starts attacking.");
            }
        }
    }

    public void OnBossDefeated()
    {
        Debug.Log("Boss defeated! Opening doors.");
        StartCoroutine(leftDoor.OpenDoor());
        StartCoroutine(rightDoor.OpenDoor());
    }

    private void CloseDoors()
    {
        StartCoroutine(leftDoor.CloseDoor());
        StartCoroutine(rightDoor.CloseDoor());
        Debug.Log("Both doors are closing.");
    }
}
