using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent, RequireComponent(typeof(TMP_InputField))]
public class IntegerInputField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TMP_InputField m_inputField;
    public int m_minValue;
    public int m_maxValue;
    public int m_scrollScale;
    bool m_isMouseOver = false;

    void Start()
    {
        m_inputField = GetComponent<TMP_InputField>();
        m_inputField.onValueChanged.AddListener(delegate { OnValueChanged(); });
        OnValueChanged();
    }

    void Update()
    {
        if (!m_isMouseOver || m_scrollScale == 0 || Mouse.current.scroll.value.y == 0.0f) return;

        m_inputField.text = (GetValue() + (m_scrollScale * (int)Mathf.Sign(Mouse.current.scroll.value.y))).ToString();
        OnValueChanged();
    }

    public void OnPointerEnter(PointerEventData _pointerEventData) { m_isMouseOver = true; }
    public void OnPointerExit(PointerEventData _pointerEventData) { m_isMouseOver = false; }

    void OnValueChanged()
    {
        //If text box is empty, set it to the minimum value
        if (m_inputField.text == "" || m_inputField.text == "-" || m_inputField.text == null) m_inputField.text = m_minValue.ToString();

        //Make input field into numbers
        m_inputField.text = ((m_inputField.text[0] == '-') ? '-' : "") + new string(m_inputField.text.Where(char.IsDigit).ToArray());

        //Update text
        m_inputField.text = Mathf.Clamp(GetValue(), m_minValue, m_maxValue).ToString();
    }

    public int GetValue()
    {
        return int.Parse(m_inputField.text.ToString());
    }
}
