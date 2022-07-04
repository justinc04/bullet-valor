using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public GameObject shop;
    [SerializeField] Transform headerButtons;
    [SerializeField] Transform itemGroups;
    [SerializeField] Transform shopItems;
    [SerializeField] Button readyButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        headerButtons.GetChild(0).GetComponent<ShopHeaderButton>().Select();
    }

    public void PurchaseItem(ItemInfo item)
    {
        GameManager.Instance.AddToInventory(item);
        GameManager.Instance.SelectPlayerItem(item);
        GameManager.Instance.ChangeMoney(-item.price);
    }

    public void UpdateShopItems()
    {
        foreach (Transform shopItem in shopItems)
        {
            shopItem.GetComponent<ShopItem>().UpdateShopItem();
        }
    }

    public void UpdateHeaderButtons()
    {
        foreach (Transform button in headerButtons)
        {
            button.GetComponent<ShopHeaderButton>().Deselect();
        }
    }

    public void ChangeItemDisplay(Transform items)
    {
        foreach (Transform itemGroup in itemGroups)
        {
            itemGroup.gameObject.SetActive(false);
        }

        items.gameObject.SetActive(true);
        shopItems = items;
        UpdateShopItems();
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
