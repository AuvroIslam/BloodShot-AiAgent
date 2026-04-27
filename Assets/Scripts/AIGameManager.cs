using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Singletons;

public class AIGameManager : MonoBehaviour
{
    private static AIGameManager _instance;
    public static AIGameManager Instance => _instance;

    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _winnerText;

    private int _aliveAgents = 2; // Assuming 1v1

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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM("Game BGM");
        }

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }
        
        // Count active agents on start
        _aliveAgents = GameObject.FindGameObjectsWithTag("Agent").Length;
    }

    public void ReportAgentDeath(GameObject deadAgent)
    {
        _aliveAgents--;

        if (_aliveAgents <= 1)
        {
            // The match is over. Find who is still alive.
            GameObject winner = null;
            var agents = GameObject.FindGameObjectsWithTag("Agent");
            foreach(var agent in agents)
            {
                 // In Unity, Destroy doesn't immediately remove the object until end of frame, 
                 // and it might just be deactivated anyway. Let's just grab the first one not matching deadAgent.
                 if (agent != deadAgent)
                 {
                     winner = agent;
                     break;
                 }
            }

            DeclareWinner(winner != null ? winner.name : "Draw");
        }
    }

    private void DeclareWinner(string winnerName)
    {
        if (_winnerText != null)
        {
            _winnerText.text = $"{winnerName} Wins!";
        }

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // Pause
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
         Time.timeScale = 1f; 
         // Assuming SceneLoader exists, but usually its better to just push index 0
         SceneManager.LoadScene(0);
    }
}
