using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Scriptable Objects/InventoryItemSO")]
public abstract class InventoryItemSO : ScriptableObject
{
    public Sprite UISprite;
    public string ItemName;
    public Rarity Rarity;
    public int Price;

    [TextArea(3, 6)]
    public string SpecialDescription;
}
