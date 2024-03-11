using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[DisallowMultipleComponent, RequireComponent(typeof(TMP_InputField))]
public class IntegerInputField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TMP_InputField m_inputField; public TMP_InputField m_InputField { get { if (m_inputField == null) m_inputField = GetComponent<TMP_InputField>(); return m_inputField; } }
    public int m_minValue;
    public int m_maxValue;
    public int m_scrollScale;
    bool m_isMouseOver = false;
    
    public int m_Value
    {
        get { return int.Parse(m_InputField.text.ToString()); }
        set { m_InputField.text = value.ToString(); }
    }

    void Start()
    {
        m_InputField.onValueChanged.AddListener(delegate { OnValueChanged(); });
        OnValueChanged();
    }

    void Update()
    {
        if (!m_isMouseOver || m_scrollScale == 0 || Mouse.current.scroll.value.y == 0.0f) return;

        m_InputField.text = (m_Value + (m_scrollScale * (int)Mathf.Sign(Mouse.current.scroll.value.y))).ToString();
        OnValueChanged();
    }

    public void OnPointerEnter(PointerEventData _pointerEventData) { m_isMouseOver = true; }
    public void OnPointerExit(PointerEventData _pointerEventData) { m_isMouseOver = false; }

    void OnValueChanged()
    {
        //If text box is empty, set it to the minimum value
        if (m_InputField.text == "" || m_InputField.text == "-" || m_InputField.text == null) m_InputField.text = m_minValue.ToString();

        //Make input field into numbers
        m_InputField.text = ((m_InputField.text[0] == '-') ? '-' : "") + new string(m_InputField.text.Where(char.IsDigit).ToArray());

        //Update text
        m_InputField.text = Mathf.Clamp(m_Value, m_minValue, m_maxValue).ToString();
    }
}
