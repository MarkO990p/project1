using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Armorbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalArmorBar;
    [SerializeField] private Image currentArmorBar;

    private void Start()
    {
        totalArmorBar.fillAmount = playerHealth.currentArmor / 50;
    }

    private void Update()
    {
        currentArmorBar.fillAmount = playerHealth.currentArmor / 50;
    } 
}
