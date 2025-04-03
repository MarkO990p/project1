using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStatHealthtemporaryModifierSO : CharaterStatModifierSO
{
    [SerializeField] private float duration = 90f; // ระยะเวลา 1.50 นาที (90 วินาที)
    [SerializeField] private int healAmount = 10; // เพิ่มเลือดทีละ 10

    public override void AffectCharacter(GameObject character, float val)
    {
        Health health = character.GetComponent<Health>();
        if (health != null)
        {
            // เริ่ม Coroutine เพื่อเพิ่มเลือดทีละ 10 ทุกๆ 10 วินาที
            health.StartCoroutine(HealOverTime(health));  // ใช้ StartCoroutine ผ่านคอมโพเนนต์ Health
        }
    }

    // Coroutine ที่จะเพิ่มเลือดทีละ 10 ทุกๆ 10 วินาทีในระยะเวลา 1.50 นาที
    private IEnumerator HealOverTime(Health health)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            health.AddHealth(healAmount);  // เพิ่มเลือดทีละ 10
            elapsedTime += 10f;  // เวลาผ่านไปทุกๆ 10 วินาที
            yield return new WaitForSeconds(10f); // รอ 10 วินาที
        }
    }
}
