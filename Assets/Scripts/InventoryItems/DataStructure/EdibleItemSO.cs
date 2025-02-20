using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EdibleItemSO : ItemSO, IDestroyableItem
    {
        [SerializeField] private List<ModifierData> modifiersData = new List<ModifierData>();
        public string ActionName => "Consume";

        public AudioClip actionSFX {get; private set;}

        public void PerformAction(GameObject gameObject)
        {
                HealthBar.Instance.UpdateHealth(+10);
        }
    }

    public interface IDestroyableItem
    {

    }

    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
        bool PerformAction(GameObject character);
    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifier;
        public float value;
    }

}
