using UnityEngine;

public class SmartDoor : MonoBehaviour
{
    public Vector3 closedPosition;
    private Vector3 openPosition;
    public float moveSpeed = 5f;

    private bool isClosing = false;
    private bool isOpening = false;

    private void Start()
    {
        openPosition = transform.position;
    }

    private void Update()
    {
        if (isClosing)
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, closedPosition) < 0.01f)
            {
                transform.position = closedPosition;
                isClosing = false;
                Debug.Log(gameObject.name + " closed.");
            }
        }

        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, openPosition) < 0.01f)
            {
                transform.position = openPosition;
                isOpening = false;
                Debug.Log(gameObject.name + " opened.");
            }
        }
    }

    public void OpenDoor()
    {
        isOpening = true;
        isClosing = false;
    }

    public void CloseDoor()
    {
        isClosing = true;
        isOpening = false;
    }
}
