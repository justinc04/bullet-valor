using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] ItemInfo itemInfo;
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemPriceText;
    [SerializeField] TMP_Text itemOwnedText;
    [SerializeField] Image itemImage;
    [SerializeField] Image coinImage;
    [SerializeField] Button itemButton;

    private void Start()
    {
        itemNameText.text = itemInfo.itemName;
        itemPriceText.text = itemInfo.price.ToString();
        itemImage.sprite = itemInfo.graphic;

        UpdateShopItem();
    }

    public void UpdateShopItem()
    {
        if (GameManager.Instance.items.Contains(itemInfo) || itemInfo.price == 0)
        {
            itemOwnedText.enabled = true;
            itemPriceText.enabled = false;
            coinImage.enabled = false;
            itemButton.interactable = false;
        }
        else if (GameManager.Instance.money < itemInfo.price)
        {
            itemImage.color = itemNameText.color = itemPriceText.color = coinImage.color = Color.black;
            itemButton.interactable = false;
        }
    }

    public void OnClick()
    {
        ShopManager.Instance.PurchaseItem(itemInfo);
        ShopManager.Instance.UpdateShopItems();
    }
}
