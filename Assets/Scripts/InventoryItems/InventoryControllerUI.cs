using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private UIInventoryPage inventoryUI;
        [SerializeField] private InventorySO inventoryData;
        [SerializeField] private AudioClip dropClip;
        [SerializeField] private AudioSource audioSource;
        public List<InventoryItem> initialItems = new List<InventoryItem>();


        [SerializeField]
        private List<ArmourSlotType> armourSlotTypes = new List<ArmourSlotType>
{
    ArmourSlotType.Helmet,
    ArmourSlotType.Chestplate,
    ArmourSlotType.Leggings,
    ArmourSlotType.Boots,
    ArmourSlotType.EquippedItem
};

        private void Start()
        {
            PrepareUI();
            PrepareInventoryData();
        }

        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;

            // Ensure armourItems is initialized with the same size as armourSlotTypes
            for (int i = 0; i < armourSlotTypes.Count; i++)
            {
                inventoryData.SetArmourItem(i, InventoryItem.GetEmptyItem());
            }

            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                    item.Value.quantity);
            }
        }

        private void PrepareUI()
        {
            Debug.Log("Preparing UI.");

            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.SetInventoryData(inventoryData);
            inventoryUI.InitalizeArmourSlots(armourSlotTypes.Count); // Use armourSlotTypes.Count

            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;

            Debug.Log("UI preparation complete.");
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            if (itemIndex < inventoryData.Size)
            {
                // Handle main inventory item
                InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
                if (inventoryItem.IsEmpty)
                    return;

                IItemAction itemAction = inventoryItem.item as IItemAction;
                if (itemAction != null)
                {
                    inventoryUI.ShowItemAction(itemIndex);
                    inventoryUI.AddAction("Use", () => PerformAction(itemIndex));
                }

                IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
                if (destroyableItem != null)
                {
                    inventoryUI.ShowItemAction(itemIndex);
                    inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
                }

                if (destroyableItem != null)
                {
                    inventoryUI.ShowItemAction(itemIndex);
                    inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
                }



                // Check if the item is armor and can be equipped
                if (inventoryItem.item is ArmourItem armourItem)
                {
                    inventoryUI.ShowItemAction(itemIndex);
                    inventoryUI.AddAction("Equip", () => EquipItem(itemIndex, armourItem));
                }
            }
            else
            {
                // Handle armor slot item
                int armorIndex = itemIndex - inventoryData.Size;
                InventoryItem armorItem = inventoryData.GetArmourItem(armorIndex);
                if (armorItem.IsEmpty)
                    return;

                // Add actions for armor slots (e.g., unequip)
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction("Unequip", () => UnequipItem(armorIndex));
            }
        }

        private void UnequipItem(int armorIndex)
        {
            InventoryItem armorItem = inventoryData.GetArmourItem(armorIndex);
            if (!armorItem.IsEmpty)
            {
                inventoryData.AddItem(armorItem); // Add the item back to the main inventory
                inventoryData.SetArmourItem(armorIndex, InventoryItem.GetEmptyItem()); // Clear the armor slot
                UpdateArmorUI();
            }
        }

        private void EquipItem(int itemIndex, ArmourItem armourItem)
        {
            int slotIndex = (int)armourItem.SlotType;
            Debug.Log($"Equipping item to armor slot: slotIndex = {slotIndex}");

            InventoryItem currentArmor = inventoryData.GetArmourItem(slotIndex);

            if (!currentArmor.IsEmpty)
            {
                // Swap items: Unequip the current armor and add it back to the main inventory
                Debug.Log($"Unequipping current armor: {currentArmor.item.name}");
                inventoryData.AddItem(currentArmor);
            }

            // Equip the new item
            InventoryItem itemToEquip = inventoryData.GetItemAt(itemIndex);
            Debug.Log($"Equipping new item: {itemToEquip.item.name}");
            inventoryData.SetArmourItem(slotIndex, itemToEquip);
            inventoryData.RemoveItem(itemIndex, 1);

            // Update the UI
            UpdateArmorUI();
        }

        private void UpdateArmorUI()
        {
            Debug.Log("Updating armor UI.");

            for (int i = 0; i < armourSlotTypes.Count; i++)
            {
                InventoryItem armorItem = inventoryData.GetArmourItem(i);
                if (!armorItem.IsEmpty)
                {
                    Debug.Log($"Updating armor slot {i} with item: {armorItem.item.name}");
                    inventoryUI.UpdateArmourSlot(i, armorItem.item.ItemImage, armorItem.quantity);
                }
                else
                {
                    Debug.Log($"Clearing armor slot {i}.");
                    inventoryUI.UpdateArmourSlot(i, null, 0);
                }
            }
        }

        private void DropItem(int itemIndex, int quantity)
        {
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }

        public void PerformAction(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                    inventoryUI.ResetSelection();
            }
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            Debug.Log($"Handling swap: itemIndex_1 = {itemIndex_1}, itemIndex_2 = {itemIndex_2}");

            if (itemIndex_1 < 0 || itemIndex_2 < 0)
            {
                Debug.LogError("Invalid indices: Indices cannot be negative.");
                return;
            }

            if (itemIndex_1 < inventoryData.Size && itemIndex_2 < inventoryData.Size)
            {
                // Swap within the main inventory
                Debug.Log("Swapping within main inventory.");
                inventoryData.SwapItems(itemIndex_1, itemIndex_2);
            }
            else if (itemIndex_1 < inventoryData.Size && itemIndex_2 >= inventoryData.Size)
            {
                // Swap from main inventory to armor slot
                int armorIndex = itemIndex_2 - inventoryData.Size;
                Debug.Log($"Attempting to swap to armor slot: armorIndex = {armorIndex}");

                if (armorIndex >= 0 && armorIndex < armourSlotTypes.Count)
                {
                    InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex_1);
                    if (inventoryItem.item is ArmourItem armourItem && (int)armourItem.SlotType == armorIndex)
                    {
                        Debug.Log($"Equipping item to armor slot: {armourItem.SlotType}");
                        EquipItem(itemIndex_1, armourItem);
                    }
                    else
                    {
                        Debug.LogError($"Item cannot be equipped to armor slot: {armorIndex}");
                    }
                }
                else
                {
                    Debug.LogError($"Invalid armor index: {armorIndex}");
                }
            }
            else if (itemIndex_1 >= inventoryData.Size && itemIndex_2 < inventoryData.Size)
            {
                // Swap from armor slot to main inventory
                int armorIndex = itemIndex_1 - inventoryData.Size;
                Debug.Log($"Attempting to swap from armor slot: armorIndex = {armorIndex}");

                if (armorIndex >= 0 && armorIndex < armourSlotTypes.Count)
                {
                    InventoryItem armorItem = inventoryData.GetArmourItem(armorIndex);
                    if (!armorItem.IsEmpty)
                    {
                        Debug.Log($"Unequipping item from armor slot: {armorIndex}");
                        inventoryData.AddItem(armorItem);
                        inventoryData.SetArmourItem(armorIndex, InventoryItem.GetEmptyItem());
                        UpdateArmorUI();
                    }
                    else
                    {
                        Debug.LogError($"Armor slot is empty: {armorIndex}");
                    }
                }
                else
                {
                    Debug.LogError($"Invalid armor index: {armorIndex}");
                }
            }
            else
            {
                Debug.LogError($"Invalid indices: itemIndex_1 = {itemIndex_1}, itemIndex_2 = {itemIndex_2}");
            }
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            if (itemIndex < inventoryData.Size)
            {
                // Handle main inventory item
                InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
                if (inventoryItem.IsEmpty)
                {
                    inventoryUI.ResetSelection();
                    return;
                }
                ItemSO item = inventoryItem.item;
                string description = PrepareDescription(inventoryItem);
                inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, description);
            }
            else
            {
                // Handle armor slot item
                int armorIndex = itemIndex - inventoryData.Size;
                InventoryItem armorItem = inventoryData.GetArmourItem(armorIndex);
                if (armorItem.IsEmpty)
                {
                    inventoryUI.ResetSelection();
                    return;
                }
                ItemSO item = armorItem.item;
                string description = PrepareDescription(armorItem);
                inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, description);
            }
        }

        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();
            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " +
                    $": {inventoryItem.itemState[i].value} / " +
                    $"{inventoryItem.item.DefaultParametersList[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void Update()
        {
           if(!PauseMenu.isPaused)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (inventoryUI.isActiveAndEnabled == false)
                    {
                        inventoryUI.Show();
                        foreach (var item in inventoryData.GetCurrentInventoryState())
                        {
                            inventoryUI.UpdateData(item.Key,
                                item.Value.item.ItemImage,
                                item.Value.quantity);
                        }
                    }
                    else
                    {
                        inventoryUI.Hide();
                    }

                }
            }
        }
    }
}