using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneButton : MonoBehaviour
{
    [Header("ชื่อของ Scene ที่จะโหลด")]
    public SceneReference sceneToLoad;


    public void LoadSpecifiedScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad.SceneName))
        {
            SceneManager.LoadScene(sceneToLoad.SceneName);
        }
        else
        {
            Debug.LogWarning("กรุณาเลือก Scene ที่ต้องการโหลดใน Inspector");
        }
    }

}
