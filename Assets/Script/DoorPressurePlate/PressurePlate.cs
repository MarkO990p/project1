using UnityEngine;

public class PressurePlate2D : MonoBehaviour
{
    public float pressDistance = 0.1f;
    public Color originalColor = Color.white;
    public Color pressedColor = Color.red;

    public Door2D door; // อ้างถึง Door2D โดยตรง

    private Vector3 plateInitialPos;
    private SpriteRenderer plateRenderer;
    private bool isPressed = false;

    private void Start()
    {
        plateInitialPos = transform.position;
        plateRenderer = GetComponent<SpriteRenderer>();
        if (plateRenderer != null)
            plateRenderer.color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            isPressed = true;

            transform.position = plateInitialPos - new Vector3(0, pressDistance, 0);
            if (plateRenderer != null)
                plateRenderer.color = pressedColor;

            if (door != null)
                door.ToggleDoor(); // เรียกใช้งาน Toggle จากฝั่ง Door
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPressed = false;
            transform.position = plateInitialPos;
            if (plateRenderer != null)
                plateRenderer.color = originalColor;
        }
    }
}
