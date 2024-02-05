using UnityEngine;
using System;
using UnityEditor;
using static InventorySystem;

[Serializable] public class InventorySystem
{
    public class Slot
    {
        public Slot(Item _item = null, byte _amount = 0)
        {
            m_item = _item;
            m_amount = _amount;
        }

        public Item m_item; //Stored Item
        public byte m_amount; //How much of that item is currently stored
    }

    static int m_slotColumns = 10; static int m_slotRows = 4;
    public Slot[,] m_slots { get; private set; } = new Slot[m_slotColumns, m_slotRows]; //The first row is accessible in the hotbar
    public delegate void OnChangeDelegate(Slot _slot); public event OnChangeDelegate m_onChange;

    public byte AddItem(Item _item, byte _amountToAdd)
    {
        //Iterate over each slot
        foreach (Slot slot in m_slots)
        {
            //Skip slots that do not have a matching item
            if (slot == null || slot.m_item != _item) continue;

            //Calculate the amount that can be added to this slot
            byte possibleIncrease = Math.Min(_amountToAdd, (byte)(_item.m_capacity - slot.m_amount));

            //Add amount to the slot
            slot.m_amount += possibleIncrease;
            _amountToAdd -= possibleIncrease;
            m_onChange?.Invoke(slot);

            //If there is no amount remaining, exit the function
            if (_amountToAdd == 0) return 0;
        }

        //If there is an amount remaining after iterating all slots, create new slots
        for (int col = 0; col < m_slotColumns; col++)
            for (int row = 0; row < m_slotRows; row++)
            {
                if (m_slots[col, row] != null) continue;

                //Add amount to the slot
                Slot slot = new Slot(_item, Math.Min(_amountToAdd, _item.m_capacity));
                m_slots[col, row] = slot;
                _amountToAdd -= slot.m_amount;
                m_onChange?.Invoke(slot);

                if (_amountToAdd == 0) return 0;
            }
        

        //If there is an amount remaining after iterating all slots, return the remaining amount
        return _amountToAdd;
    }

    public byte SubtractItem(Item _item, byte _amountToSubtract)
    {
        //Iterate over each row
        for (int col = 0; col < m_slotColumns; col++)
            for (int row = 0; row < m_slotRows; row++)
            {
                Slot slot = m_slots[col, row];

                //Skip slots that do not have a matching item
                if (slot == null || slot.m_item != _item) continue;

                //Calculate the amount that can be subtracted from this slot
                byte possibleDecrease = Math.Min(_amountToSubtract, slot.m_amount);

                //Subtract amount from the slot
                slot.m_amount -= possibleDecrease;
                _amountToSubtract -= possibleDecrease;
                m_onChange?.Invoke(slot);

                //If the slot is empty, set it to null
                if (slot.m_amount == 0) m_slots[col, row] = null;

                //If there is no amount remaining, exit the function
                if (_amountToSubtract == 0) return 0;
            }
        
        //If there is an amount remaining after iterating all slots, return the remaining amount
        return _amountToSubtract;
    }

    public bool SubtractItem(Vector2Int _slotPos, byte _amountToSubtract, bool _capAtMinimum = true)
    {
        //Check if the position are within the bounds of the array
        if (_slotPos.x < 0 || _slotPos.x >= m_slotColumns || _slotPos.y < 0 || _slotPos.y >= m_slotRows) return false;

        //Check whether the slot contains an item
        Slot slot = m_slots[_slotPos.x, _slotPos.y];
        if (slot == null) return false;

        //
        if (slot.m_amount - _amountToSubtract < 0 && _capAtMinimum == false) return false;

        //
        slot.m_amount -= _amountToSubtract;
        if (slot.m_amount <= 0) m_slots[_slotPos.x, _slotPos.y] = null;
        m_onChange?.Invoke(m_slots[_slotPos.x, _slotPos.y]);

        return true;
    }

    public byte CheckAddItem(Item _item, byte _amountToAdd)
    {
        //Iterate over each slot
        foreach (Slot slot in m_slots)
        {
            //Skip slots that do not have a matching item
            if (slot == null || slot.m_item != _item) continue;

            //Calculate the amount that can be added to this slot
            byte possibleIncrease = Math.Min(_amountToAdd, (byte)(_item.m_capacity - slot.m_amount));

            //Subtract the possible increase from the amount to check
            _amountToAdd -= possibleIncrease;

            //If there is no amount remaining, exit the function
            if (_amountToAdd == 0) return 0;
        }

        //If there is an amount remaining after iterating all slots, check if new slots can be created
        for (int col = 0; col < m_slotColumns; col++)
            for (int row = 0; row < m_slotRows; row++)
            {
                if (m_slots[col, row] != null) continue;

                //Subtract the possible amount to add from the amount to check
                _amountToAdd -= Math.Min(_amountToAdd, _item.m_capacity);

                if (_amountToAdd == 0) return 0;
            }

        //If there is an amount remaining after iterating all slots, return the remaining amount
        return _amountToAdd;
    }

    public byte CheckSubtractItem(Item _item, byte _amountToSubtract)
    {
        //Iterate over each slot
        foreach (Slot slot in m_slots)
        {
            //Skip slots that do not have a matching item
            if (slot == null || slot.m_item != _item) continue;

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

    public bool SwapItem(Vector2Int _slotAPos, Vector2Int _slotBPos)
    {
        //Check if the positions are within the bounds of the array
        if (_slotAPos.x < 0 || _slotAPos.x >= m_slotColumns || _slotAPos.y < 0 || _slotAPos.y >= m_slotRows ||
            _slotBPos.x < 0 || _slotBPos.x >= m_slotColumns || _slotBPos.y < 0 || _slotBPos.y >= m_slotRows) return false;

        //Swap the slots
        Slot temp = m_slots[_slotAPos.x, _slotAPos.y];
        m_slots[_slotAPos.x, _slotAPos.y] = m_slots[_slotBPos.x, _slotBPos.y];
        m_slots[_slotBPos.x, _slotBPos.y] = temp;
        m_onChange?.Invoke(m_slots[_slotAPos.x, _slotAPos.y]);
        m_onChange?.Invoke(m_slots[_slotBPos.x, _slotBPos.y]);

        return true;
    }

    public bool FillSlot(Vector2Int _slotTargetPos, Vector2Int _slotSourcePos)
    {
        //Check if the positions are within the bounds of the array
        if (_slotTargetPos.x < 0 || _slotTargetPos.x >= m_slotColumns || _slotTargetPos.y < 0 || _slotTargetPos.y >= m_slotRows ||
            _slotSourcePos.x < 0 || _slotSourcePos.x >= m_slotColumns || _slotSourcePos.y < 0 || _slotSourcePos.y >= m_slotRows) return false;

        //Get slots
        Slot targetSlot = m_slots[_slotTargetPos.x, _slotTargetPos.y];
        Slot sourceSlot = m_slots[_slotSourcePos.x, _slotSourcePos.y];
        if (targetSlot == null && sourceSlot == null) return false;
        if (targetSlot.m_item != sourceSlot.m_item) return false;

        //Calculate the amount to transfer from the source to the target
        byte amountToTransfer = Math.Min((byte)(targetSlot.m_item.m_capacity - targetSlot.m_amount), sourceSlot.m_amount);
        
        //Transfer amount
        targetSlot.m_amount += amountToTransfer;
        sourceSlot.m_amount -= amountToTransfer;
        m_onChange?.Invoke(targetSlot);
        m_onChange?.Invoke(sourceSlot);

        return true;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InventorySystem))]
    public class InventoryDrawer : PropertyDrawer
    {
        bool isFoldOutOpen = false;
        float propertyHeight = EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            propertyHeight = EditorGUIUtility.singleLineHeight;
            InventorySystem inventorySystem = (InventorySystem)fieldInfo.GetValue(_property.serializedObject.targetObject);
            Rect fieldPosition = new Rect(_position.x, _position.y, _position.width, EditorGUIUtility.singleLineHeight);

            //Add/Subtract Item
            if (!Application.isPlaying) return;

            //Check fold out open
            isFoldOutOpen = EditorGUI.Foldout(fieldPosition, isFoldOutOpen, _label);
            if (!isFoldOutOpen) return;

            //View all slots
            fieldPosition.y += EditorGUIUtility.singleLineHeight;
            for (int row = 0; row < m_slotRows; row++) for (int col = 0; col < m_slotColumns; col++)
                {
                    Slot slot = inventorySystem.m_slots[col, row];
                    string label = $"\t[{col}, {row}]: ";

                    //Show slot data
                    if (slot != null) label += $"Item: {slot.m_item.name}, Amount: {slot.m_amount}";

                    //Print slot data
                    EditorGUI.LabelField(fieldPosition, label);
                    fieldPosition.y += EditorGUIUtility.singleLineHeight;
                }

            //Add space
            fieldPosition.y += EditorGUIUtility.singleLineHeight;

            //Set property height
            propertyHeight += fieldPosition.y + (1.0f * EditorGUIUtility.singleLineHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propertyHeight;
        }
    }

#endif
}