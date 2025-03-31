using UnityEngine;

public class Door2D : MonoBehaviour
{
    public float openDistance = 3f;
    public float moveSpeed = 5f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;

    private void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openDistance;
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    private void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);
    }
}
