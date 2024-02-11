using UnityEngine;
using System;
using UnityEditor;
using static UnityEditor.Progress;
using System.Reflection;

[Serializable] public class InventorySystem
{
    [Serializable] public struct Slot
    {
        public Slot(Item _item = null, byte _amount = 0)
        {
            m_item = _item;
            m_amount = _amount;
        }

        public Item m_item; //Stored Item
        public byte m_amount; //How much of that item is currently stored
        
        public bool IsValid()
        {
            return m_item != null && m_amount > 0;
        }
    }

    public Slot[] m_slots;
    public delegate void OnChangeDelegate(ref Slot _slot, int _slotIndex); public event OnChangeDelegate m_onChange;

    public InventorySystem(int _slotCount)
    {
        m_slots = new Slot[_slotCount];
    }

    public byte AddItem(Item _item, byte _amountToAdd)
    {
        //Check whether the given item is valid or whether the amount to add is valid
        if (_item == null || _amountToAdd <= 0) return _amountToAdd;

        //Iterate over each slot
        for (int i = 0; i < m_slots.Length; i++)
        {
            ref Slot slot = ref m_slots[i];

            //Skip slots that do not have a matching item
            if (slot.m_item != _item) continue;

            //Calculate the amount that can be added to this slot
            byte possibleIncrease = Math.Min(_amountToAdd, (byte)(_item.m_capacity - slot.m_amount));

            //Add amount to the slot
            slot.m_amount += possibleIncrease;
            _amountToAdd -= possibleIncrease;
            m_onChange?.Invoke(ref slot, i);

            //If there is no amount remaining, exit the function
            if (_amountToAdd == 0) return 0;
        }

        //If there is an amount remaining after iterating all slots, create new slots
        for (int i = 0; i < m_slots.Length; i++)
        {
            ref Slot slot = ref m_slots[i];
            if (slot.IsValid()) continue;

            //Add amount to the slot
            slot.m_item = _item; slot.m_amount = Math.Min(_amountToAdd, _item.m_capacity);
            _amountToAdd -= slot.m_amount;
            m_onChange?.Invoke(ref slot, i);

            if (_amountToAdd == 0) return 0;
        }

        //If there is an amount remaining after iterating all slots, return the remaining amount
        return _amountToAdd;
    }

    public byte SubtractItem(Item _item, byte _amountToSubtract)
    {
        //Check whether the given item is valid or whether the amount to subtract is valid
        if (_item == null || _amountToSubtract <= 0) return _amountToSubtract;

        //Iterate over each row
        for (int i = 0; i < m_slots.Length; i++)
        {
            ref Slot slot = ref m_slots[i];

            //Skip slots that do not have a matching item
            if (!slot.IsValid() || slot.m_item != _item) continue;

            //Calculate the amount that can be subtracted from this slot
            byte possibleDecrease = Math.Min(_amountToSubtract, slot.m_amount);

            //Subtract amount from the slot
            slot.m_amount -= possibleDecrease;
            _amountToSubtract -= possibleDecrease;
            m_onChange?.Invoke(ref slot, i);

            //If there is no amount remaining, exit the function
            if (_amountToSubtract == 0) return 0;
        }
        
        //If there is an amount remaining after iterating all slots, return the remaining amount
        return _amountToSubtract;
    }

    public bool SubtractItem(int _slotIndex, byte _amountToSubtract, bool _capAtMinimum = true)
    {
        //Check if the position are within the bounds of the array
        if (_slotIndex < 0 || _slotIndex >= m_slots.Length) return false;

        //Check whether the slot contains an item
        Slot slot = m_slots[_slotIndex];
        if (!slot.IsValid()) return false;

        //
        if (slot.m_amount - _amountToSubtract < 0 && _capAtMinimum == false) return false;

        //
        slot.m_amount -= _amountToSubtract;
        m_onChange?.Invoke(ref m_slots[_slotIndex], _slotIndex);

        return true;
    }

    public byte CheckAddItem(Item _item, byte _amountToAdd)
    {
        //Check whether the given item is valid or whether the amount to add is valid
        if (_item == null || _amountToAdd <= 0) return _amountToAdd;

        //Iterate over each slot
        foreach (Slot slot in m_slots)
        {
            //Skip slots that do not have a matching item
            if (!slot.IsValid() || slot.m_item != _item) continue;

            //Calculate the amount that can be added to this slot
            byte possibleIncrease = Math.Min(_amountToAdd, (byte)(_item.m_capacity - slot.m_amount));

            //Subtract the possible increase from the amount to check
            _amountToAdd -= possibleIncrease;

            //If there is no amount remaining, exit the function
            if (_amountToAdd == 0) return 0;
        }

        //If there is an amount remaining after iterating all slots, check if new slots can be created
        for (int i = 0; i < m_slots.Length; i++)
        {
            if (m_slots[i].IsValid()) continue;

            //Subtract the possible amount to add from the amount to check
            _amountToAdd -= Math.Min(_amountToAdd, _item.m_capacity);

            if (_amountToAdd == 0) return 0;
        }

        //If there is an amount remaining after iterating all slots, return the remaining amount
        return _amountToAdd;
    }

    public byte CheckSubtractItem(Item _item, byte _amountToSubtract)
    {
        //Check whether the given item is valid or whether the amount to subtract is valid
        if (_item == null || _amountToSubtract <= 0) return _amountToSubtract;

        //Iterate over each slot
        foreach (Slot slot in m_slots)
        {
            //Skip slots that do not have a matching item
            if (!slot.IsValid() || slot.m_item != _item) continue;

            //Calculate the amount that can be subtracted from this slot
            byte possibleDecrease = Math.Min(_amountToSubtract, slot.m_amount);

            //Subtract amount from the _amountToSubtract
            _amountToSubtract -= possibleDecrease;

            //If there is no amount remaining, exit the function
            if (_amountToSubtract == 0) return 0;
        }

        //If there is an amount remaining after iterating all slots, return the remaining amount
        return _amountToSubtract;
    }

    public bool SwapItem(int _slotAIndex, int _slotBIndex)
    {
        //Check if the positions are within the bounds of the array
        if (_slotAIndex < 0 || _slotAIndex >= m_slots.Length) return false;
        if (_slotBIndex < 0 || _slotBIndex >= m_slots.Length) return false;

        //Swap the slots
        Slot temp = m_slots[_slotAIndex];
        m_slots[_slotAIndex] = m_slots[_slotBIndex];
        m_slots[_slotBIndex] = temp;
        m_onChange?.Invoke(ref m_slots[_slotAIndex], _slotAIndex);
        m_onChange?.Invoke(ref m_slots[_slotBIndex], _slotBIndex);

        return true;
    }

    public bool FillSlot(int _slotTargetPos, int _slotSourcePos)
    {
        //Check if the positions are within the bounds of the array
        if (_slotTargetPos < 0 || _slotTargetPos >= m_slots.Length) return false;
        if (_slotSourcePos < 0 || _slotSourcePos >= m_slots.Length) return false;

        //Get slots
        ref Slot targetSlot = ref m_slots[_slotTargetPos];
        ref Slot sourceSlot = ref m_slots[_slotSourcePos];
        if (!targetSlot.IsValid() || !sourceSlot.IsValid()) return false;
        if (targetSlot.m_item != sourceSlot.m_item) return false;

        //Calculate the amount to transfer from the source to the target
        byte amountToTransfer = Math.Min((byte)(targetSlot.m_item.m_capacity - targetSlot.m_amount), sourceSlot.m_amount);
        
        //Transfer amount
        targetSlot.m_amount += amountToTransfer;
        sourceSlot.m_amount -= amountToTransfer;
        m_onChange?.Invoke(ref targetSlot, _slotSourcePos);
        m_onChange?.Invoke(ref sourceSlot, _slotSourcePos);

        return true;
    }

//#if UNITY_EDITOR
//    public Item m_itemToAdjust;
//    public byte m_amountToAdjust;

//    [CustomPropertyDrawer(typeof(InventorySystem))]
//    public class InventoryDrawer : PropertyDrawer
//    {
//        bool isFoldOutOpen = false;
//        float propertyHeight = EditorGUIUtility.singleLineHeight;

//        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
//        {
//            propertyHeight = EditorGUIUtility.singleLineHeight;
//            InventorySystem inventorySystem = (InventorySystem)fieldInfo.GetValue(_property.serializedObject.targetObject);
//            Rect fieldPosition = new Rect(_position.x, _position.y, _position.width, EditorGUIUtility.singleLineHeight);

//            //Check fold out open
//            isFoldOutOpen = EditorGUI.Foldout(fieldPosition, isFoldOutOpen, _label);
//            if (!isFoldOutOpen) return;

//            // Display m_itemToAdjust and m_amountToAdjust fields
//            fieldPosition.y += EditorGUIUtility.singleLineHeight;
//            inventorySystem.m_itemToAdjust = (Item)EditorGUI.ObjectField(fieldPosition, "Item to Adjust", inventorySystem.m_itemToAdjust, typeof(Item), true);

//            fieldPosition.y += EditorGUIUtility.singleLineHeight;
//            inventorySystem.m_amountToAdjust = (byte)EditorGUI.IntField(fieldPosition, "Amount to Adjust", inventorySystem.m_amountToAdjust);

//            //if (Application.isPlaying)
//            {
//                //Add/Subtract Item
//                fieldPosition.y += EditorGUIUtility.singleLineHeight;
//                if (GUI.Button(fieldPosition, "Add Item")) inventorySystem.AddItem(inventorySystem.m_itemToAdjust, inventorySystem.m_amountToAdjust);

//                fieldPosition.y += EditorGUIUtility.singleLineHeight;
//                if (GUI.Button(fieldPosition, "Remove Item")) inventorySystem.SubtractItem(inventorySystem.m_itemToAdjust, inventorySystem.m_amountToAdjust);

//                //View all slots
//                fieldPosition.y += EditorGUIUtility.singleLineHeight;
//                for (int i = 0; i < inventorySystem.m_slots.Length; i++)
//                {
//                    ref Slot slot = ref inventorySystem.m_slots[i];
//                    string label = $"\t[{i}]: ";

//                    //Show slot data
//                    if (slot.IsValid()) label += $"Item: {slot.m_item.name}, Amount: {slot.m_amount}";
//                    else label += "null";

//                    //Print slot data
//                    EditorGUI.LabelField(fieldPosition, label);
//                    fieldPosition.y += EditorGUIUtility.singleLineHeight;
//                }

//                //Add space
//                fieldPosition.y += EditorGUIUtility.singleLineHeight;
//            }

//            //Set property height
//            propertyHeight += fieldPosition.y + (1.0f * EditorGUIUtility.singleLineHeight);
//        }

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            return propertyHeight;
//        }
//    }

//#endif
}