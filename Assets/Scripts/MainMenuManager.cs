using UnityEngine;
using Singletons;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _settingsButton;
    [SerializeField] private GameObject _quitButton;
    [SerializeField] private GameObject _aiVsAiButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject _settingsPanel;

    private void Start()
    {
        // Play main menu background music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM("Menu BGM");
        }

        // Ensure settings panel is hidden at start
        if (_settingsPanel != null)
        {
            _settingsPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        // This will be called by the Start Game button
        Singletons.SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
        if (sceneLoader != null)
        {
            sceneLoader.SceneLoad("SampleScene");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }

    public void StartAIVsAIGame()
    {
        // This will be called by the AI vs AI button
        Singletons.SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
        if (sceneLoader != null)
        {
            sceneLoader.SceneLoad("AiVsAiScene");
        }
        else
        {
            // Direct load without loading scene if SceneLoader missing
            UnityEngine.SceneManagement.SceneManager.LoadScene("AiVsAiScene");
        }
    }

    public void OpenSettings()
    {
        // Hide main menu buttons
        if (_startButton != null) _startButton.SetActive(false);
        if (_settingsButton != null) _settingsButton.SetActive(false);
        if (_quitButton != null) _quitButton.SetActive(false);
        if (_aiVsAiButton != null) _aiVsAiButton.SetActive(false);

        // Show settings panel
        if (_settingsPanel != null) _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        // Hide settings panel
        if (_settingsPanel != null) _settingsPanel.SetActive(false);

        // Show main menu buttons
        if (_startButton != null) _startButton.SetActive(true);
        if (_settingsButton != null) _settingsButton.SetActive(true);
        if (_quitButton != null) _quitButton.SetActive(true);
        if (_aiVsAiButton != null) _aiVsAiButton.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
