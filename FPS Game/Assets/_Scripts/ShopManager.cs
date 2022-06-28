using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public GameObject shop;
    [SerializeField] Transform shopItems;
    [SerializeField] Button readyButton;

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
        readyButton.interactable = false;
        GameManager.Instance.ReadyToStart();
    }

    public void OpenShop()
    {
        shop.SetActive(true);
        readyButton.interactable = true;
        UpdateShopItems();
    }
}
