using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "ScriptableObjects/Item Data")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int itemID;
    public ItemType itemType;
    public int itemValue;


    // Additional properties and methods can be added as needed


    public enum ItemType
    {
        Consumable,
        Equipment,
        Quest
        // Add more types as needed
    }
}
