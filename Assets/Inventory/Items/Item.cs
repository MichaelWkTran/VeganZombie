using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Item", menuName = "Inventory/Create New Item")]
public class Item : ScriptableObject
{
    [TextArea] public string m_description; //The description of the move
    public Sprite m_icon; //The icon representing the item
    public byte m_capacity = byte.MaxValue; //How many of that item can be held
}