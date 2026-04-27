using UnityEngine;
using UnityEngine.Events;

public class AgentStats : MonoBehaviour
{
    private int _score = 0;
    
    public int Score => _score;
    public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();

    public void AddScore(int amount)
    {
        if (amount > 0)
        {
            _score += amount;
            OnScoreChanged.Invoke(_score);
        }
    }

    public void ResetScore()
    {
        _score = 0;
        OnScoreChanged.Invoke(_score);
    }
}
