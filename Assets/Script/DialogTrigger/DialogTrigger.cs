using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string[] dialogLines; // ข้อความที่จะโชว์
    public KeyCode interactKey = KeyCode.E;

    private bool isPlayerInRange;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            DialogManager.Instance.StartDialog(dialogLines);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}
