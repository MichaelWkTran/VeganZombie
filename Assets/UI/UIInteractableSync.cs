using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasGroup))]
public class UIInteractableSync : MonoBehaviour
{
    public CanvasGroup m_canvasGroup;

    void OnEnable()
    {
        m_canvasGroup.interactable = gameObject.activeSelf;
    }

    void OnDisable()
    {
        m_canvasGroup.interactable = gameObject.activeSelf;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIInteractableSync))]
public class UIInteractableSyncEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UIInteractableSync uiInteractableSync = target as UIInteractableSync;

        //
        if (Application.isPlaying) return;

        //
        if (uiInteractableSync.m_canvasGroup == null || uiInteractableSync.m_canvasGroup.gameObject != uiInteractableSync.gameObject)
            uiInteractableSync.m_canvasGroup = uiInteractableSync.GetComponent<CanvasGroup>();

        //
        if (uiInteractableSync.m_canvasGroup) uiInteractableSync.m_canvasGroup.interactable = uiInteractableSync.gameObject.activeSelf;
    }
}

#endif