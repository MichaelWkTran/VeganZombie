using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Item", menuName = "Inventory and Items/Create New Item")]
public class Item : ScriptableObject
{
    public enum Category
    {
        Status,
        Weapon,
        Armor,
        Key
    }

    [TextArea] public string m_description; //The description of the move
    public Sprite m_icon; //The icon representing the item
    public Category[] m_category; //What category does the item belong to
    public byte m_capacity = byte.MaxValue; //How many of that item can be held
}
