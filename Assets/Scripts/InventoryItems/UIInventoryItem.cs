using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler,
        IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text quantityTxt;
        [SerializeField] private Image borderImage;

        public event Action<UIInventoryItem> OnItemClicked,
            OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag,
            OnRightMouseBtnClick;

        private bool empty = true;

        public void Awake()
        {
            ResetData();
            Deselect();
        }
        public void ResetData()
        {
            itemImage.gameObject.SetActive(false);
            empty = true;
        }
        public void Deselect()
        {
            borderImage.enabled = false;
        }
        public void SetData(Sprite sprite, int quantity)
        {
            Debug.Log($"Setting data for UIInventoryItem: Sprite = {sprite}, Quantity = {quantity}");
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("Item Image is not assigned in UIInventoryItem.");
            }

            if (quantityTxt != null)
            {
                quantityTxt.text = quantity + "";
            }
            else
            {
                Debug.LogError("Quantity Text is not assigned in UIInventoryItem.");
            }

            empty = false;
        }

        public void Select()
        {
            if (borderImage != null)
            {
                borderImage.enabled = true;
                Debug.Log("Border image enabled for UIInventoryItem.");
            }
            else
            {
                Debug.LogError("Border Image is not assigned in UIInventoryItem.");
            }
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else
            {
                OnItemClicked?.Invoke(this);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty)
                return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}