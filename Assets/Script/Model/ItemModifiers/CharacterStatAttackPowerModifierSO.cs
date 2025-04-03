using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStatAttackPowerModifierSO : CharaterStatModifierSO
{
    [SerializeField] private float duration = 60f; // ระยะเวลาในการเพิ่มพลังโจมตี (ในวินาที)
    [SerializeField] private int attackBoost = 100; // เพิ่มพลังโจมตี 100

    public override void AffectCharacter(GameObject character, float val)
    {
        MaleAttack maleAttack = character.GetComponent<MaleAttack>(); // ดึงคอมโพเนนต์ MaleAttack
        if (maleAttack != null)
        {
            maleAttack.AddAttackPower(attackBoost); // เพิ่มพลังโจมตี
            // เรียกใช้ Coroutine เพื่อให้การเพิ่มพลังโจมตีหมดอายุหลังจากระยะเวลาที่กำหนด
            maleAttack.StartCoroutine(RemoveAttackPowerAfterTime(maleAttack));  // เรียก Coroutine จาก MaleAttack
        }
    }

    // Coroutine ที่จะลบพลังโจมตีออกหลังจากระยะเวลาที่กำหนด
    private IEnumerator RemoveAttackPowerAfterTime(MaleAttack maleAttack)
    {
        yield return new WaitForSeconds(duration);  // รอจนกว่าจะหมดเวลา
        maleAttack.RemoveAttackPower(attackBoost);  // คืนค่าพลังโจมตีกลับ
    }
}
