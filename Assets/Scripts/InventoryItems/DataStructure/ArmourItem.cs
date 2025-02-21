using UnityEngine;
using Inventory.Model;

[CreateAssetMenu(fileName = "New Armour Item", menuName = "Inventory/Armour Item")]
public class ArmourItem : ItemSO
{
    public ArmourSlotType SlotType;
}
