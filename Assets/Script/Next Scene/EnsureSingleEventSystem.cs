using UnityEngine;
using UnityEngine.EventSystems;

public class EnsureSingleEventSystem : MonoBehaviour
{
    void Awake()
    {
        EventSystem[] systems = FindObjectsOfType<EventSystem>();
        if (systems.Length > 1)
        {
            Debug.LogWarning("พบ EventSystem มากกว่า 1 ตัว กำลังลบตัวนี้ออก", this);
            Destroy(this.gameObject);
        }
    }
}
