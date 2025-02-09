using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneButton : MonoBehaviour
{
    [Header("ชื่อของ Scene ที่จะโหลด")]
    public string sceneName;

    public void LoadSpecifiedScene()
    {
        // ตรวจสอบว่าชื่อ Scene ไม่เป็นค่าว่าง
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("กรุณากำหนดชื่อ Scene ที่ต้องการจะโหลดใน Inspector");
        }
    }
}
