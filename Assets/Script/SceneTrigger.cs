using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger2D : MonoBehaviour
{
    public string sceneToLoad; // ชื่อ Scene ที่จะโหลด

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
