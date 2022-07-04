using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopHeaderButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Color baseColor;
    [SerializeField] Color selectedColor;

    public void OnClick(Transform items)
    {
        ShopManager.Instance.ChangeItemDisplay(items);
        ShopManager.Instance.UpdateHeaderButtons();
        Select();
    }

    public void Select()
    {
        button.image.color = selectedColor;
    }

    public void Deselect()
    {
        button.image.color = baseColor;
    }
}
