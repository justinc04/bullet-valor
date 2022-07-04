using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : ScriptableObject
{
    public string itemName;
    public enum ItemType { Equipable, Armor, Ability }
    public ItemType itemType;
    public int price;
    public Sprite graphic;
}
