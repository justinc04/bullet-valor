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
    [SerializeField] Color baseColor;
    [SerializeField] Color selectedColor;

    private void Start()
    {
        itemNameText.text = itemInfo.itemName;
        itemPriceText.text = itemInfo.price.ToString();
        itemImage.sprite = itemInfo.graphic;

        UpdateShopItem();
    }

    public void UpdateShopItem()
    {
        if (GameManager.Instance.inventory.Contains(itemInfo))
        {
            if (GameManager.Instance.items.Contains(itemInfo))
            {
                itemButton.image.color = selectedColor;
            }
            else
            {
                itemButton.image.color = baseColor;
            }

            itemOwnedText.enabled = true;
            itemPriceText.enabled = false;
            coinImage.enabled = false;
        }
        else if (GameManager.Instance.money < itemInfo.price)
        {
            itemImage.color = itemNameText.color = itemPriceText.color = coinImage.color = Color.black;
            itemButton.interactable = false;
        }
        else
        {
            itemImage.color = itemNameText.color = itemPriceText.color = coinImage.color = Color.white;
            itemButton.interactable = true;
        }
    }

    public void ShowPreview()
    {
        ShopManager.Instance.ChangeItemPreview(itemInfo, itemImage.rectTransform.sizeDelta);
    }

    public void OnClick()
    {
        if (GameManager.Instance.inventory.Contains(itemInfo))
        {
            GameManager.Instance.SelectPlayerItem(itemInfo);
        }
        else
        {
            ShopManager.Instance.PurchaseItem(itemInfo);
        }

        ShopManager.Instance.UpdateShopItems();
    }
}
