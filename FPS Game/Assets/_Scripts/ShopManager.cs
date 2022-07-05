using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public GameObject shop;
    [SerializeField] Transform headerButtons;
    [SerializeField] Transform itemGroups;
    [SerializeField] Transform shopItems;
    [SerializeField] Button readyButton;

    [SerializeField] GameObject itemPreview;
    [SerializeField] TMP_Text itemPreviewNameText;
    [SerializeField] Image itemPreviewImage;
    [SerializeField] float imageSizeRatio;
    [SerializeField] TMP_Text itemPreviewDescriptionText;

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

    public void ChangeItemPreview(ItemInfo item, Vector2 imageSize)
    {
        itemPreview.SetActive(true);
        itemPreviewNameText.text = item.itemName;
        itemPreviewImage.sprite = item.graphic;
        itemPreviewImage.rectTransform.sizeDelta = imageSize * imageSizeRatio;
        itemPreviewDescriptionText.text = item.description;
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
        itemPreview.SetActive(false);
        UpdateShopItems();
    }
}
