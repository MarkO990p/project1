using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStatArmorModifierSO : CharaterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        Health health = character.GetComponent<Health>();  // เปลี่ยนชื่อเป็น health แทน armor
        if (health != null)
            health.AddArmor((int)val);  // ใช้ AddArmor() ที่สร้างในคลาส Health
    }
}
