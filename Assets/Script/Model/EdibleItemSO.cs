using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EdibleItemSO : ItemSo, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifierData = new List<ModifierData>();
        public string ActionName => "Consume";
        public AudioClip actionSFX { get; private set; }
        public bool PerformAction(GameObject character)
        {
            foreach (ModifierData data in modifierData)
            {
                data.statModifier.AffectCharacter(character, data.value);
            }
            return true;
        }

    }

    public interface IDestroyableItem
    {

    }

    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get;  }
        bool PerformAction(GameObject character);
    }

    [System.Serializable]
    public class ModifierData
    {
        public CharaterStatModifierSO statModifier;
        public float value;
    }

}