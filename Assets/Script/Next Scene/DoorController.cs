//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class DoorController : MonoBehaviour
//{
//    public SceneReference sceneToLoad; // ใช้ SceneReference แทน string
//    public Vector3 customReturnPosition;
//    public GameObject player;
//    public KeyCode interactKey = KeyCode.E;
//    private bool isPlayerNearby = false;

//    void Start()
//    {
//        player.transform.position = customReturnPosition;
//        Debug.Log("ตั้งตำแหน่งเริ่มต้นของผู้เล่นเป็น customReturnPosition: " + customReturnPosition);
//    }

//    void Update()
//    {
//        if (isPlayerNearby && Input.GetKeyDown(interactKey))
//        {
//            SceneManager.LoadScene(sceneToLoad.SceneName); // ใช้ SceneName ที่ได้จาก SceneReference
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            isPlayerNearby = true;
//        }
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            isPlayerNearby = false;
//        }
//    }
//}
