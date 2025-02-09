using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public Transform shootingPoint;  // จุดที่กระสุนจะถูกยิงออก
    public GameObject bulletPrefab;  // Prefab ของกระสุน
    public float bulletSpeed = 20f;  // ความเร็วของกระสุน
    public AudioClip shootSound;     // ไฟล์เสียงการยิง

    private Animator animator;       // อ้างอิง Animator
    private AudioSource audioSource; // อ้างอิง AudioSource
    private bool isShooting = false; // ตัวแปรควบคุมสถานะการยิง

    void Start()
    {
        animator = GetComponent<Animator>();      // อ้างอิง Animator
        audioSource = GetComponent<AudioSource>(); // อ้างอิง AudioSource

        // กำหนดค่าเบื้องต้นของ AudioSource
        audioSource.mute = false;
        audioSource.volume = 1.0f;
    }

    void Update()
    {
        // ตรวจสอบการกดปุ่ม J เพื่อยิง
        if (Keyboard.current.jKey.wasPressedThisFrame && !isShooting)
        {
            StartCoroutine(Shoot()); // เรียกใช้ Coroutine เพื่อยิง
        }
    }

    // Coroutine สำหรับการยิง
    IEnumerator Shoot()
    {
        isShooting = true; // ตั้งค่าสถานะการยิง
        animator.SetBool("isShooting", true); // ตั้งค่า Bool isShooting ใน Animator ให้เป็น true

        // เล่นเสียงการยิง
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // รอจนกว่าอนิเมชั่นการยิงจะเสร็จ (ปรับเวลาตามระยะเวลาของอนิเมชั่น)
        yield return new WaitForSeconds(0.5f); // เปลี่ยนตัวเลขนี้ตามระยะเวลาของอนิเมชั่น

        // สร้างกระสุนเมื่ออนิเมชั่นจบ
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = shootingPoint.right * bulletSpeed;

        // ปิดสถานะการยิง
        animator.SetBool("isShooting", false); // ตั้งค่า Bool isShooting ใน Animator ให้เป็น false
        isShooting = false; // รีเซ็ตสถานะการยิง
    }
}
