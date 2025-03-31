using System.Collections.Generic;
using UnityEngine;

public class Armorbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth; // อ้างอิงไปที่ Health.cs
    [SerializeField] private List<GameObject> armorBars; // รายการของ GameObject แต่ละ Bar

    private float armorPerBar = 10f; // แต่ละ Bar แทนค่าเกราะ 10 หน่วย

    private void Start()
    {
        UpdateArmorBars(); // อัปเดตค่าเริ่มต้นเมื่อเริ่มเกม
    }

    private void Update()
    {
        UpdateArmorBars();
    }

    private void UpdateArmorBars()
    {
        if (playerHealth == null) return;

        float currentArmor = playerHealth.GetCurrentArmor();

        for (int i = 0; i < armorBars.Count; i++)
        {
            if (armorBars[i] != null) // ป้องกันข้อผิดพลาดในกรณีที่มี Bar หายไป
            {
                armorBars[i].SetActive(currentArmor >= (i + 1) * armorPerBar);
            }
        }
    }
}
