using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public GameObject shop;

    private void Awake()
    {
        Instance = this;
    }

    public void PurchaseItem(ItemInfo item)
    {
        GameManager.Instance.items.Clear();
        GameManager.Instance.items.Add(item);

        GameManager.Instance.playerManager.CreateController();
        shop.SetActive(false);
    }
}
