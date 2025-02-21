using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.UI;
using Inventory;
using Inventory.Model;


namespace Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField] private UIInventoryItem itemPrefab;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private RectTransform armourSlotsParent;
        [SerializeField] private UIInventoryDescription itemDescription;
        [SerializeField] private MouseFollower mouseFollower;
        [SerializeField] private ItemActionPanel actionPanel;
        [SerializeField] private GameObject armourSlotPrefab;
        [SerializeField] private InventorySO inventoryData;
        private List<UIInventoryItem> armourSlots = new List<UIInventoryItem>();
        List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
        private int currentlyDraggedItemIndex = -1;
        public event Action<int, int> OnSwapItems;
        public event Action<int> OnDescriptionRequested,
                OnItemActionRequested,
                OnStartDragging;

        private void Awake()
        {
            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }

        public void SetInventoryData(InventorySO inventoryData)
        {
            this.inventoryData = inventoryData;
        }

        public void InitalizeArmourSlots(int slotCount)
        {
            Debug.Log($"Initializing {slotCount} armor slots.");

            foreach (Transform child in armourSlotsParent)
            {
                Destroy(child.gameObject);
            }
            armourSlots.Clear();

            for (int i = 0; i < slotCount; i++)
            {
                GameObject slot = Instantiate(armourSlotPrefab, armourSlotsParent);
                UIInventoryItem slotUI = slot.GetComponent<UIInventoryItem>();
                if (slotUI != null)
                {
                    armourSlots.Add(slotUI);
                    slotUI.ResetData(); // Initialize the slot as empty

                    // Add drag-and-drop support for armor slots
                    slotUI.OnItemBeginDrag += HandleBeginDrag;
                    slotUI.OnItemEndDrag += HandleEndDrag;
                    slotUI.OnItemDroppedOn += HandleSwap;

                    Debug.Log($"Armor slot {i} initialized.");
                }
                else
                {
                    Debug.LogError($"Failed to get UIInventoryItem component for armor slot {i}.");
                }
            }
        }

        public void UpdateArmourSlot(int index, Sprite itemImage, int quantity)
        {
            if (index >= 0 && index < armourSlots.Count)
            {
                armourSlots[index].SetData(itemImage, quantity);
            }
            else
            {
                Debug.LogError($"Invalid armor slot index: {index}");
            }
        }

        public void InitializeInventoryUI(int inventorysize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                UIInventoryItem uiItem =
                    Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                uiItem.transform.SetParent(contentPanel);
                listOfUIItems.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        internal void ResetAllItems()
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();

            if (itemIndex < listOfUIItems.Count)
            {
                // Select the main inventory item
                listOfUIItems[itemIndex].Select();
            }
            else
            {
                // Select the armor slot
                int armorIndex = itemIndex - listOfUIItems.Count;
                if (armorIndex >= 0 && armorIndex < armourSlots.Count)
                {
                    armourSlots[armorIndex].Select();
                    Debug.Log("Selected armor slot: " + armorIndex);
                }
                else
                {
                    Debug.LogWarning("Not selecting armor slot due to invalid index: " + armorIndex);
                }
            }
        }

        public void UpdateData(int itemIndex,
            Sprite itemImage, int itemQuantity)
        {
            if (listOfUIItems.Count > itemIndex)
            {
                listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index != -1)
            {
                // Item is from the main inventory
                OnItemActionRequested?.Invoke(index);
            }
            else
            {
                // Item is from the armor slots
                int armorIndex = armourSlots.IndexOf(inventoryItemUI);
                if (armorIndex != -1)
                {
                    // Trigger the action for the armor slot
                    OnItemActionRequested?.Invoke(armorIndex + inventoryData.Size); // Use inventoryData.Size
                }
            }
        }

        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDraggedItem();
        }

        private void HandleSwap(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index != -1)
            {
                // Item is from the main inventory
                OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            }
            else
            {
                // Item is from the armor slots
                int armorIndex = armourSlots.IndexOf(inventoryItemUI);
                if (armorIndex != -1)
                {
                    // Handle swapping with armor slots
                    OnSwapItems?.Invoke(currentlyDraggedItemIndex, armorIndex + inventoryData.Size);
                }
            }
            HandleItemSelection(inventoryItemUI);
        }

        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index != -1)
            {
                // Item is from the main inventory
                currentlyDraggedItemIndex = index;
                OnStartDragging?.Invoke(index);
            }
            else
            {
                // Item is from the armor slots
                int armorIndex = armourSlots.IndexOf(inventoryItemUI);
                if (armorIndex != -1)
                {
                    currentlyDraggedItemIndex = armorIndex + inventoryData.Size; // Offset by main inventory size
                    OnStartDragging?.Invoke(currentlyDraggedItemIndex);
                }
                else
                {
                    Debug.LogError("Invalid drag source: Item not found in main inventory or armor slots.");
                }
            }
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }

        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index != -1)
            {
                // Item is from the main inventory
                OnDescriptionRequested?.Invoke(index);
            }
            else
            {
                // Item is from the armor slots
                int armorIndex = armourSlots.IndexOf(inventoryItemUI);
                if (armorIndex != -1)
                {
                    // Trigger the description for the armor slot
                    OnDescriptionRequested?.Invoke(armorIndex + inventoryData.Size);
                }
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectAllItems();
        }

        public void AddAction(string actionName, Action performAction)
        {
            actionPanel.AddButon(actionName, performAction);
        }

        public void ShowItemAction(int itemIndex)
        {
            actionPanel.Toggle(true);
            actionPanel.transform.position = listOfUIItems[itemIndex].transform.position;
        }

        private void DeselectAllItems()
        {
            foreach (UIInventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
            foreach (UIInventoryItem slot in armourSlots)
            {
                slot.Deselect();
            }
            actionPanel.Toggle(false);
        }

        public void Hide()
        {
            actionPanel.Toggle(false);
            gameObject.SetActive(false);
            ResetDraggedItem();
        }
    }
}