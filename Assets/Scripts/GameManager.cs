using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Singletons;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] private GameObject _gameOverPanel;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        // Play game background music when game scene starts
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM("Game BGM");
        }

        // Hide game over panel at start
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        StartCoroutine(ShowGameOverDelayed());
    }

    private IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSecondsRealtime(1f);
        
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(0); // Load main menu (scene 0)
    }
}
