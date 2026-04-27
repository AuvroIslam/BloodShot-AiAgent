using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance => _instance;

    private int _score;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // avoid duplicates
            return;
        }

        _instance = this;
    }
    private void Update()
    {
        print(_score);
    }
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        _score += amount;
    }

    public int GetScore() => _score;

    public void ResetScore() => _score = 0;
}
