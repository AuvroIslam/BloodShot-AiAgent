using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Singletons;

public class ButtonAnimated : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float _hoverScale = 1.2f;
    [SerializeField] private float _pressScale = 0.9f;
    [SerializeField] private float _animationSpeed = 10f;

    private Vector3 _normalScale;
    private Vector3 _targetScale;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _normalScale = _rectTransform.localScale;
        _targetScale = _normalScale;
    }

    private void Update()
    {
        // Smooth scale animation (uses unscaledDeltaTime so it works when paused)
        _rectTransform.localScale = Vector3.Lerp(_rectTransform.localScale, _targetScale, Time.unscaledDeltaTime * _animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up on hover
        _targetScale = _normalScale * _hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Return to normal scale
        _targetScale = _normalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Scale down on press
        _targetScale = _normalScale * _pressScale;

        // Play button click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("Button Click");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Return to hover scale if still hovering, otherwise normal
        if (eventData.hovered.Contains(gameObject))
        {
            _targetScale = _normalScale * _hoverScale;
        }
        else
        {
            _targetScale = _normalScale;
        }
    }

    private void OnDisable()
    {
        // Reset scale when disabled
        if (_rectTransform != null)
        {
            _rectTransform.localScale = _normalScale;
            _targetScale = _normalScale;
        }
    }
}
