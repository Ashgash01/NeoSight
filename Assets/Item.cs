using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;

    public virtual void PickUp()
    {
        Sprite ItemIcon = GetComponent<Image>().sprite;
        if (ItemPickupUICotroller.Instance != null )
        {
            ItemPickupUICotroller.Instance.ShowItemPickup(Name, ItemIcon);
        }
    }
}
