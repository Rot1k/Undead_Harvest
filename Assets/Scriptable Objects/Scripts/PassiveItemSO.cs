using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemSO", menuName = "Scriptable Objects/PassiveItemSO")]
public partial class PassiveItemSO : InventoryItemSO
{

    [Serializable]
    public class ModifierData
    {
        public StatType statType;
        public ModifierType modifierType;
        public float value;
    }

    public List<ModifierData> Modifiers = new();

}
