using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;
    private bool _isPaused = false;

    private void Start()
    {
        // Make sure pause menu is hidden at start
        if (_pauseMenuPanel != null)
        {
            _pauseMenuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Toggle pause with ESC key (works with new Input System)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        _isPaused = true;
        if (_pauseMenuPanel != null)
        {
            _pauseMenuPanel.SetActive(true);
        }
        Time.timeScale = 0f; // Freeze the game
    }

    public void Resume()
    {
        _isPaused = false;
        if (_pauseMenuPanel != null)
        {
            _pauseMenuPanel.SetActive(false);
        }
        Time.timeScale = 1f; // Resume the game
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading
        SceneManager.LoadScene(0); // Load scene at index 0 (Main Menu)
    }
}
