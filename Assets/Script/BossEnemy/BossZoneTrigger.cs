using UnityEngine;

public class BossZoneTrigger : MonoBehaviour
{
    public GameObject boss;
    public DoorController2D leftDoor;   // แก้เป็น DoorController2D
    public DoorController2D rightDoor;

    private Boss bossScript;

    private void Start()
    {
        bossScript = boss.GetComponent<Boss>();
        bossScript.IsActive = false;
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
                bossScript.ActivateBoss();
                Debug.Log("Boss starts attacking.");
            }
        }
    }

    public void OnBossDefeated()
    {
        Debug.Log("Boss defeated! Opening doors.");
        leftDoor.OpenDoor();
        rightDoor.OpenDoor();
    }

    private void CloseDoors()
    {
        leftDoor.CloseDoor();
        rightDoor.CloseDoor();
        Debug.Log("Both doors are closing.");
    }
}
