using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [SerializeField] GameObject shop;
    [SerializeField] Transform shopItems;

    private void Awake()
    {
        Instance = this;
    }

    public void PurchaseItem(ItemInfo item)
    {
        GameManager.Instance.items.Clear();
        GameManager.Instance.items.Add(item);
        GameManager.Instance.money -= item.price;
    }

    public void UpdateShopItems()
    {
        foreach (Transform shopItem in shopItems)
        {
            shopItem.GetComponent<ShopItem>().UpdateShopItem();
        }
    }
}
