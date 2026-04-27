using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private AgentStats _agentStats; // New specific stat
    [SerializeField] private bool _findScoreManagerAutomatically = true;

    private void Start()
    {
        // Find text component if not assigned
        if (_scoreText == null)
        {
            _scoreText = GetComponent<TextMeshProUGUI>();
        }

        // Find ScoreManager automatically only if we don't have an AgentStats assigned
        if (_findScoreManagerAutomatically && _scoreManager == null && _agentStats == null)
        {
            _scoreManager = FindFirstObjectByType<ScoreManager>();
        }

        if (_scoreManager == null && _agentStats == null)
        {
            Debug.LogError("ScoreUI: Neither ScoreManager nor AgentStats found!");
        }
    }

    private void Update()
    {
        if (_agentStats != null && _scoreText != null)
        {
            // Agent-specific score
            _scoreText.text = _agentStats.Score.ToString();
        }
        else if (_scoreManager != null && _scoreText != null)
        {
            // Global score (single player)
            _scoreText.text = _scoreManager.GetScore().ToString();
        }
    }
}
