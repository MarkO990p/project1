using UnityEngine;

public enum PortalType
{
    Next,
    Previous,
    Custom
}

public class Portal : MonoBehaviour
{
    public PortalType portalType = PortalType.Custom;
    public string destinationId; // ใช้เมื่อ portalType = Custom

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting || !other.CompareTag("Player"))
            return;

        string targetId = GetDestinationId();

        Transform destination = PortalManager.instance.GetDestination(targetId);
        if (destination != null)
        {
            isTeleporting = true;
            other.transform.position = destination.position;
        }
        else
        {
            Debug.LogWarning("Destination not found: " + targetId);
        }
    }

    private string GetDestinationId()
    {
        switch (portalType)
        {
            case PortalType.Next:
                return GetNextId();
            case PortalType.Previous:
                return GetPreviousId();
            case PortalType.Custom:
            default:
                return destinationId;
        }
    }

    private string GetNextId()
    {
        // ตัวอย่าง: ถ้า destinationId = Room_01 → จะหา Room_02
        if (destinationId.StartsWith("Room_"))
        {
            int num;
            if (int.TryParse(destinationId.Substring(5), out num))
                return $"Room_{(num + 1):00}";
        }
        return destinationId;
    }

    private string GetPreviousId()
    {
        if (destinationId.StartsWith("Room_"))
        {
            int num;
            if (int.TryParse(destinationId.Substring(5), out num))
                return $"Room_{(num - 1):00}";
        }
        return destinationId;
    }
}
