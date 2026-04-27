using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Singletons;
using System.Collections;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button _skipButton;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private TextMeshProUGUI _tipText;
    [SerializeField] private float _minimumLoadTime = 2f;
    [SerializeField] private float _skipButtonDelay = 1f;
    [SerializeField] private float _skipButtonScalePulse = 1.3f;
    [SerializeField] private float _skipButtonAnimDuration = 0.3f;
    
    [Header("Tips")]
    [SerializeField] private string[] _tips = new string[]
    {
        "It's better to not shoot aimlessly.\nEvery shot costs your life.So make it count.",
        "Collect souls as fast as you can.\nSouls fuel your survival and strengthen your spirit.",
        "Each shot has an equivalent exchange with life.\nFire wisely, for every bullet draws from your life force.",
        "It doesn't mean you want to kill — but you must.\nThis world demands balance, not malice.",
        "Watch your health bar like your soul depends on it — it does.\nOne hit can be the price of a mistake.",
        "Fire in bursts, not floods.\nExcess drains you faster than your enemies.",
        "Reclaim life from every fallen hero.\nRespect their sacrifice . It's your lifeline.",
        "If you don't kill, you'll be overwhelmed by enemies.\nStillness invites the horde . Fight or be crowded out."
    };
    
    private bool _canSkip = false;
    private string _targetSceneName = "SampleScene"; // Fallback if SceneLoader isn't found
    private float _loadingProgress = 0f;

    private void Start()
    {
        // Setup skip button
        if (_skipButton != null)
        {
            _skipButton.onClick.AddListener(SkipLoading);
            _skipButton.interactable = false;
            _skipButton.gameObject.SetActive(false); // Hide initially
        }

        // Setup loading bar
        if (_loadingBar != null)
        {
            _loadingBar.minValue = 0f;
            _loadingBar.maxValue = 1f;
            _loadingBar.value = 0f;
        }

        // Show random tip
        if (_tipText != null && _tips.Length > 0)
        {
            int randomIndex = Random.Range(0, _tips.Length);
            _tipText.text = _tips[randomIndex];
        }

        // Start loading process
        StartCoroutine(LoadingProcess());
        StartCoroutine(ActivateSkipButton());
    }

    private IEnumerator ActivateSkipButton()
    {
        // Wait for delay
        yield return new WaitForSeconds(_skipButtonDelay);

        if (_skipButton != null)
        {
            // Show button
            _skipButton.gameObject.SetActive(true);
            
            // Store original scale
            Vector3 originalScale = _skipButton.transform.localScale;
            
            // Scale up animation
            float elapsed = 0f;
            while (elapsed < _skipButtonAnimDuration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (_skipButtonAnimDuration / 2f);
                _skipButton.transform.localScale = Vector3.Lerp(originalScale, originalScale * _skipButtonScalePulse, t);
                yield return null;
            }
            
            // Scale back down animation
            elapsed = 0f;
            while (elapsed < _skipButtonAnimDuration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (_skipButtonAnimDuration / 2f);
                _skipButton.transform.localScale = Vector3.Lerp(originalScale * _skipButtonScalePulse, originalScale, t);
                yield return null;
            }
            
            // Ensure exact original scale
            _skipButton.transform.localScale = originalScale;
            
            // Make button clickable immediately after animation
            _skipButton.interactable = true;
        }
    }

    private void Update()
    {
        // Update loading bar smoothly
        if (_loadingBar != null)
        {
            _loadingBar.value = Mathf.Lerp(_loadingBar.value, _loadingProgress, Time.deltaTime * 2f);
        }
    }

    private IEnumerator LoadingProcess()
    {
        float elapsedTime = 0f;

        // Simulate loading with progress bar
        while (elapsedTime < _minimumLoadTime)
        {
            elapsedTime += Time.deltaTime;
            _loadingProgress = Mathf.Clamp01(elapsedTime / _minimumLoadTime);
            yield return null;
        }

        // Loading complete
        _loadingProgress = 1f;

        // Auto-load after a bit more time if player doesn't skip
        yield return new WaitForSeconds(3f);
        LoadGameScene();
    }

    public void SkipLoading()
    {
        if (_skipButton != null && _skipButton.interactable)
        {
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
        if (sceneLoader != null && !string.IsNullOrEmpty(sceneLoader.TargetScene))
        {
            _targetSceneName = sceneLoader.TargetScene;
        }
        
        SceneManager.LoadScene(_targetSceneName);
    }

    private void OnDestroy()
    {
        if (_skipButton != null)
        {
            _skipButton.onClick.RemoveListener(SkipLoading);
        }
    }
}
