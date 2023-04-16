using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Inventory Config")]
public class ItemData : ScriptableObject
{
    public int itemId;
    public string itemName;
    public Sprite itemImg;
    public bool canStack;
}
