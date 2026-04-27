using UnityEngine;
using UnityEngine.UI;
using Health;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Image _fillImage;
    [SerializeField] private HealthSystem _playerHealth;
    [SerializeField] private bool _findPlayerAutomatically = true;

    [Header("Animation Settings")]
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private Color _healColor = Color.green;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private float _flashDuration = 0.3f;
    [SerializeField] private float _shakeMagnitude = 5f;
    [SerializeField] private float _scalePulse = 1.15f;

    private float _previousHealth;
    private Vector3 _originalPosition;
    private Vector3 _originalScale;
    private float _flashTimer = 0f;
    private bool _isFlashing = false;
    private Color _currentFlashColor;

    private void Start()
    {
        // Find the slider if not assigned
        if (_healthSlider == null)
        {
            _healthSlider = GetComponent<Slider>();
        }

        // Find fill image if not assigned
        if (_fillImage == null && _healthSlider != null)
        {
            _fillImage = _healthSlider.fillRect?.GetComponent<Image>();
        }

        // Find player automatically if needed
        if (_findPlayerAutomatically && _playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerHealth = player.GetComponent<HealthSystem>();
            }
        }

        // Subscribe to health changes
        if (_playerHealth != null)
        {
            _healthSlider.maxValue = _playerHealth.MaxHealth;
            _playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            _previousHealth = _playerHealth.CurrentHealth;
            UpdateHealthBar(_playerHealth.CurrentHealth);
        }
        else
        {
            Debug.LogError("HealthBarUI: No HealthSystem found!");
        }

        // Store original transform
        _originalPosition = transform.localPosition;
        _originalScale = transform.localScale;

        // Set normal color
        if (_fillImage != null)
        {
            _fillImage.color = _normalColor;
        }
    }

    private void Update()
    {
        // Handle flash animation
        if (_isFlashing)
        {
            _flashTimer -= Time.deltaTime;
            
            if (_flashTimer <= 0f)
            {
                // Flash ended, return to normal
                _isFlashing = false;
                if (_fillImage != null)
                {
                    _fillImage.color = _normalColor;
                }
                transform.localPosition = _originalPosition;
                transform.localScale = _originalScale;
            }
            else
            {
                // Flash progress (0 to 1, where 1 is start and 0 is end)
                float progress = _flashTimer / _flashDuration;
                
                // Color lerp
                if (_fillImage != null)
                {
                    _fillImage.color = Color.Lerp(_normalColor, _currentFlashColor, progress);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }

    private void UpdateHealthBar(float currentHealth)
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = currentHealth;
        }

        // Determine if damage or heal
        if (currentHealth < _previousHealth)
        {
            // Damage taken
            PlayDamageAnimation();
        }
        else if (currentHealth > _previousHealth)
        {
            // Health gained
            PlayHealAnimation();
        }

        _previousHealth = currentHealth;
    }

    private void PlayDamageAnimation()
    {
        // Red flash and shake
        _currentFlashColor = _damageColor;
        _flashTimer = _flashDuration;
        _isFlashing = true;

        // Shake effect
        transform.localPosition = _originalPosition + (Vector3)Random.insideUnitCircle * _shakeMagnitude;
    }

    private void PlayHealAnimation()
    {
        // Green flash and scale pulse
        _currentFlashColor = _healColor;
        _flashTimer = _flashDuration;
        _isFlashing = true;

        // Scale pulse effect
        transform.localScale = _originalScale * _scalePulse;
    }
}
