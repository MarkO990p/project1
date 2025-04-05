using UnityEngine;
using System.Collections;

public class DoorController2D : MonoBehaviour
{
    public Transform closedPositionTarget;
    public float moveSpeed = 3f;

    private Vector3 openPosition;
    private Vector3 closedPosition;
    private Coroutine moveRoutine;

    private void Start()
    {
        openPosition = transform.position;
        if (closedPositionTarget != null)
        {
            closedPosition = closedPositionTarget.position;
            closedPosition.z = transform.position.z; // ล็อคแกน Z ให้นิ่ง
        }
        else
        {
            Debug.LogError($"{name} missing closedPositionTarget");
        }
    }

    public void OpenDoor()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveDoor(openPosition));
    }

    public void CloseDoor()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveDoor(closedPosition));
    }

    private IEnumerator MoveDoor(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}
