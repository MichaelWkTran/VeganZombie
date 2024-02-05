using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour 
{
    [SerializeField] SceneTransition m_sceneTransitionPrefab;
    public LoadSceneMode m_loadSceneMode;
    public bool m_loadedAsync;

    public void LoadScene(string _sceneName) 
    {
        if (m_sceneTransitionPrefab == null)
        {
            if (!m_loadedAsync) SceneManager.LoadScene(_sceneName, m_loadSceneMode);
            else SceneManager.LoadSceneAsync(_sceneName, m_loadSceneMode);
        }
        else
        {
            SceneTransition sceneTransition = Instantiate(m_sceneTransitionPrefab);
            sceneTransition.m_newSceneName = _sceneName;
            sceneTransition.m_loadSceneMode = m_loadSceneMode;
            sceneTransition.m_loadedAsync = m_loadedAsync;
        }
    }
}