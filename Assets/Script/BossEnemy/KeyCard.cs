using UnityEngine;

public class KeyCard : MonoBehaviour
{
    public bool hasKeyCard = false;

    public void ObtainKeyCard()
    {
        hasKeyCard = true;
        Debug.Log("ได้รับ KeyCard แล้ว!");
    }

    public bool UseKeyCard()
    {
        if (hasKeyCard)
        {
            hasKeyCard = false;
            Debug.Log("ใช้ KeyCard แล้ว");
            return true;
        }
        Debug.Log("ไม่มี KeyCard ให้ใช้");
        return false;
    }
}
