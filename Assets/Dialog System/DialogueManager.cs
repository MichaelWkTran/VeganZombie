using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Febucci.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager m_dialogueManager { get; private set; }
    public TextAsset m_inkJSON;
    Story m_story;

    [Serializable] public struct DialogEvent
    {
        public string m_dialog; //What dialog has been displayed
        public List<string> m_tags; //What tags does the dialog have
        public List<Choice> m_choices; //What are the available options
        public uint m_selectedChoice; //What option have the player selected
    };
    [SerializeField] List<DialogEvent> m_history = new List<DialogEvent>(); //The recorded dialouge read
    [SerializeField] uint m_historyIndex = 0U; //------------------------------------------
    bool m_isReadingRecordedDialog = false; //Whether the player is reading recorded dialog

    [Header("UI")]
    [SerializeField] Button m_dialogPannel; //The dialog pannel
    RectTransform m_dialogTransform; //The rect transform of the dialog pannel
    [SerializeField] TMP_Text m_dialogText; //The text component where the dialog is displayed
    [SerializeField] TypewriterByCharacter m_typeWriter;
    [SerializeField] LayoutGroup m_choicesGroup; //The group containing the buttons 
    [SerializeField] Button m_choiceButtonPrefab; //The button prefab used by the player to select a dialog choice
    [SerializeField] GameObject m_namePannel;
    [SerializeField] float m_dialogLerpTime;
    [SerializeField] float m_charactersPerSecond;
    [SerializeField] CanvasGroup m_canvasGroup;

    void Awake()
    {
        m_dialogTransform = m_dialogPannel.GetComponent<RectTransform>();
        m_dialogTransform.anchoredPosition3D = Vector3.down * Screen.height;
        m_canvasGroup.interactable = false;
    }

    void Start()
    {
        //Ensure this class is a singeton
        if (m_dialogueManager == null) m_dialogueManager = this;
        else if (m_dialogueManager != this) { Destroy(gameObject); return; }
    }

    public void SetInkJSON(TextAsset _inkJSON)
    {
        m_inkJSON = _inkJSON;
    }

    public void StartDialouge(TextAsset _inkJSON)
    {
        SetInkJSON(_inkJSON);
        StartDialouge();
    }

    public void StartDialouge()
    {
        gameObject.SetActive(true);

        //If no text file is given, disable the manager
        if (m_inkJSON == null) { EndDialouge(); return; }

        //Start Open Animation
        Open();

        //Create New Story
        m_story = new Story(m_inkJSON.text);

        //Get errors or warnings from the story
        m_story.onError += (msg, type) =>
        {
            if (type == Ink.ErrorType.Warning) Debug.LogWarning(msg);
            else Debug.LogError(msg);
        };

        //Reset manager variables
        m_history.Clear();
        m_historyIndex = 0U;

        //Start Dialouge
        Continue();
    }

    public void EndDialouge()
    {
        Close();
    }

    public void Open()
    {
        //Move the dialog on screen when it is opened
        LeanTween.cancel(m_dialogTransform);
        LeanTween.move(m_dialogTransform, Vector3.zero, m_dialogLerpTime).
            setEase(LeanTweenType.easeOutExpo).setIgnoreTimeScale(true);
        m_canvasGroup.interactable = true;
        PauseManager.m_current.m_interactionsPaused.Add(this);
    }

    public void Close()
    {
        //Move the dialog off screen when it is closed
        LeanTween.cancel(m_dialogTransform);
        LeanTween.move(m_dialogTransform, Vector3.down * Screen.height, m_dialogLerpTime).
            setEase(LeanTweenType.easeOutExpo).setIgnoreTimeScale(true);
        m_canvasGroup.interactable = false;
        PauseManager.m_current.m_interactionsPaused.Remove(this);
    }

    bool CanContinueHaveChoices()
    {
        return m_story.canContinue || m_story.currentChoices.Count > 0;
    }

    public void Continue(int _choice = 0)
    {
        //Skip to full dialog when the typing animation is still running
        //if (m_currentTypeDialog != null) { UpdateUI(false); return; }
        if (m_typeWriter.isShowingText) { m_typeWriter.SkipTypewriter(); return; }

        //Close the dialog UI when the dialog has finished
        if (!CanContinueHaveChoices() && m_historyIndex <= 0) { EndDialouge(); return; }

        //Update the dialog that is being shown
        if (m_historyIndex > 0)
        {
            //If a previously viewed dialog is shown then move to a more recent dialog
            m_historyIndex--;
        }
        else
        {
            if (m_story.currentChoices.Count > 0) m_story.ChooseChoiceIndex(_choice);

            //Add newly read dialog to the dialog history
            DialogEvent newDialogEvent;
            newDialogEvent.m_dialog = m_story.Continue();
            newDialogEvent.m_tags = m_story.currentTags;
            newDialogEvent.m_choices = m_story.currentChoices;
            newDialogEvent.m_selectedChoice = (uint)_choice;
            m_history.Add(newDialogEvent);
        }

        //Update the UI to match the viewed dialog
        UpdateUI(!m_isReadingRecordedDialog);

        //Reset reading recorded dialog variable
        if (m_historyIndex <= 0) m_isReadingRecordedDialog = false;
    }

    public void Back()
    {
        if (LeanTween.isTweening(m_dialogTransform)) return;

        //Prevent historyIndex from going outside the dialog history range
        if (m_historyIndex >= m_history.Count - 1) return;
        m_historyIndex++;
        m_isReadingRecordedDialog = true;
        UpdateUI(false);
    }

    void UpdateUI(bool _typeDialog = true)
    {
        DialogEvent dialogEvent = m_history[^(1 + (int)m_historyIndex)];
        
        //Set the text in the name pannel
        try
        {
            m_namePannel.SetActive(true);
            m_namePannel.GetComponentInChildren<TMP_Text>().text = dialogEvent.m_tags.Find(name => name.Contains("name:")).Substring(5);
        }
        catch
        {
            m_namePannel.SetActive(false);
        }

        //Set the dialog text
        m_dialogPannel.interactable = true;
        if (_typeDialog) m_typeWriter.ShowText(dialogEvent.m_dialog);
        else m_dialogText.text = dialogEvent.m_dialog;
        
        //Clear choice buttons
        foreach (Transform child in m_choicesGroup.transform) Destroy(child.gameObject);

        //Create choice buttons
        if (m_historyIndex == 0U && dialogEvent.m_choices.Count > 0) m_dialogPannel.interactable = false;

        foreach (Choice choice in dialogEvent.m_choices)
        {
            //Create buttons and set their text
            GameObject choiceButton = Instantiate(m_choiceButtonPrefab.gameObject, m_choicesGroup.transform);
            choiceButton.GetComponentInChildren<TMP_Text>().text = choice.text;

            //Disable choise button if the player is seeing dialouge from the history
            if (m_historyIndex > 0U) choiceButton.GetComponent<Button>().interactable = false;
            choiceButton.GetComponent<Button>().onClick.AddListener(delegate { Continue(choice.index); });
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DialogueManager dialogueManager = (DialogueManager)target;
        if (GUILayout.Button("Open Dialogue")) dialogueManager.StartDialouge();
    }
}

#endif