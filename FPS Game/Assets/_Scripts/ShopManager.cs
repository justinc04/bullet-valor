using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public GameObject shop;
    [SerializeField] Transform shopItems;

    private void Awake()
    {
        Instance = this;
    }

    public void PurchaseItem(ItemInfo item)
    {
        GameManager.Instance.AddPlayerItem(item);
        GameManager.Instance.ChangeMoney(-item.price);
    }

    public void UpdateShopItems()
    {
        foreach (Transform shopItem in shopItems)
        {
            shopItem.GetComponent<ShopItem>().UpdateShopItem();
        }
    }

    public void OnClickReady()
    {
        shop.SetActive(false);
        GameManager.Instance.playerManager.CreateController();
    }
}
