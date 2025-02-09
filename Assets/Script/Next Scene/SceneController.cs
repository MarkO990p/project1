using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject player;  // อ้างอิงถึง GameObject ของผู้เล่น (ต้องกำหนดใน Inspector)
    public Vector3 startPosition; // ตำแหน่งเริ่มต้นของผู้เล่นที่สามารถตั้งค่าใน Inspector
    private string currentSceneName;  // ตัวแปรเก็บชื่อของ Scene ปัจจุบัน

    void Start()
    {
        // ตรวจสอบว่ามีการตั้งค่าตัวแปร player หรือไม่ ถ้าไม่ ให้ลองค้นหา GameObject ที่มี Tag "Player"
        if (player == null)
        {
            Debug.LogWarning("Player object is not assigned in the Inspector. Attempting to find the player using Tag 'Player'.");
            player = GameObject.FindGameObjectWithTag("Player");

            // ถ้ายังไม่เจอ Player ให้แสดงข้อผิดพลาด
            if (player == null)
            {
                Debug.LogError("Player object could not be found in the scene. Please assign it in the Inspector or ensure a GameObject with the 'Player' tag exists.");
                return;
            }
        }

        // เก็บชื่อ Scene ปัจจุบัน
        currentSceneName = SceneManager.GetActiveScene().name;

        // ตรวจสอบว่ามีข้อมูลตำแหน่งของผู้เล่นใน PlayerPrefs หรือไม่
        if (HasPlayerPrefsData() && PlayerPrefs.GetString("LastScene") == currentSceneName)
        {
            LoadPlayerTransform();  // โหลดตำแหน่งและการหมุนของผู้เล่นจาก PlayerPrefs
        }
        else
        {
            // ตั้งค่าตำแหน่งเริ่มต้นให้กับผู้เล่น
            player.transform.position = startPosition;  // ใช้ startPosition ที่ตั้งค่าไว้ใน Inspector
            player.transform.rotation = Quaternion.identity;  // การหมุนเริ่มต้น
        }
    }

    public void GoToScene2()
    {
        SavePlayerTransform();  // บันทึกตำแหน่งของผู้เล่น
        StartCoroutine(LoadSceneAfterSaving("LABzone2"));  // โหลดฉาก LABzone2
    }

    public void GoToScene1()
    {
        SavePlayerTransform();  // บันทึกตำแหน่งของผู้เล่น
        StartCoroutine(LoadSceneAfterSaving("SampleScene"));  // โหลดฉาก SampleScene
    }

    public void GoToScene3()
    {
        SavePlayerTransform();  // บันทึกตำแหน่งของผู้เล่น
        StartCoroutine(LoadSceneAfterSaving("LABzone3"));  // โหลดฉาก LABzone3
    }

    private void SavePlayerTransform()
    {
        if (player != null)
        {
            PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
            PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);

            PlayerPrefs.SetFloat("PlayerRotX", player.transform.rotation.x);
            PlayerPrefs.SetFloat("PlayerRotY", player.transform.rotation.y);
            PlayerPrefs.SetFloat("PlayerRotZ", player.transform.rotation.z);
            PlayerPrefs.SetFloat("PlayerRotW", player.transform.rotation.w);

            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();

            Debug.Log("บันทึกตำแหน่งของผู้เล่นเรียบร้อย: " + player.transform.position);
        }
        else
        {
            Debug.LogWarning("Player object is not assigned.");
        }
    }

    private void LoadPlayerTransform()
    {
        Vector3 savedPosition = new Vector3(
            PlayerPrefs.GetFloat("PlayerX"),
            PlayerPrefs.GetFloat("PlayerY"),
            PlayerPrefs.GetFloat("PlayerZ")
        );

        Quaternion savedRotation = new Quaternion(
            PlayerPrefs.GetFloat("PlayerRotX"),
            PlayerPrefs.GetFloat("PlayerRotY"),
            PlayerPrefs.GetFloat("PlayerRotZ"),
            PlayerPrefs.GetFloat("PlayerRotW")
        );

        player.transform.position = savedPosition;
        player.transform.rotation = savedRotation;

        Debug.Log("โหลดตำแหน่งของผู้เล่นจาก PlayerPrefs: " + savedPosition);
    }

    private bool HasPlayerPrefsData()
    {
        return PlayerPrefs.HasKey("PlayerX") && PlayerPrefs.HasKey("PlayerY") && PlayerPrefs.HasKey("PlayerZ") &&
               PlayerPrefs.HasKey("PlayerRotX") && PlayerPrefs.HasKey("PlayerRotY") && PlayerPrefs.HasKey("PlayerRotZ") && PlayerPrefs.HasKey("PlayerRotW");
    }

    private IEnumerator LoadSceneAfterSaving(string sceneName)
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(sceneName);
    }

    public void ResetPlayerPosition()
    {
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("PlayerZ");
        PlayerPrefs.DeleteKey("PlayerRotX");
        PlayerPrefs.DeleteKey("PlayerRotY");
        PlayerPrefs.DeleteKey("PlayerRotZ");
        PlayerPrefs.DeleteKey("PlayerRotW");
        PlayerPrefs.Save();

        Debug.Log("รีเซ็ตตำแหน่งของผู้เล่นใน PlayerPrefs เรียบร้อย");
    }
}
