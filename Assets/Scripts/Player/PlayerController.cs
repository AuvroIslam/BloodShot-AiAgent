using UnityEngine;
using UnityEngine.EventSystems;
using Singletons;
using Health;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _firePointDistance = 0.6f; // distance from player toward mouse
    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private int _bulletCost = 5;
    [SerializeField] private Transform _walkPoint;

    private float _fireCooldown = 0f;
    private bool _isFiring = false;
    private float _moveSoundTimer = 0f;
    [SerializeField] private float _moveSoundInterval = 0.4f; // Play footstep every 0.4 seconds

    private HealthSystem _playerHealth;

    private void OnEnable()
    {
        InputHandler.Instance.OnFire += Firing;
    }
    private void OnDisable()
    {
        InputHandler.Instance.OnFire -= Firing;
    }

    private Vector3 _lastPosition;

    private void Start()
    {
        _playerHealth = GetComponent<HealthSystem>();
        if(_playerHealth==null)
        {
            Debug.LogError("PlayerController: No HealthSystem component found on the player.");
        }
        _lastPosition = transform.position;
    }
    private void Update()
    {
        MovePlayer();
        UpdateFirePoint(); // keep fire point following the mouse
        Fire();
        if (_fireCooldown > 0f)
            _fireCooldown -= Time.deltaTime;
        if (_moveSoundTimer > 0f)
            _moveSoundTimer -= Time.deltaTime;
        print(_playerHealth.CurrentHealth);
    }

    private void MovePlayer()
    {
        Vector2 moveDir = InputHandler.Instance.MoveDirection;
        bool isMoving = moveDir.sqrMagnitude > 0.01f;

        // Play movement sound repeatedly while moving
        if (isMoving && _moveSoundTimer <= 0f && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("Player Move");
            _moveSoundTimer = _moveSoundInterval;
        }

        transform.position += _moveSpeed * Time.deltaTime * new Vector3(moveDir.x, moveDir.y, 0);
    }

    // Position the fire point at a fixed distance toward the mouse and make its Y axis point at the mouse.
    private void UpdateFirePoint()
    {
        if (_firePoint == null) return;
        if (InputHandler.Instance == null) return;

        var cam = Camera.main;
        if (cam == null) return;

        Vector3 mouseScreen = InputHandler.Instance.MousePosition;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = transform.position.z;

        Vector3 dir = mouseWorld - transform.position;
        if (dir.sqrMagnitude <= Mathf.Epsilon)
            return;

        Vector3 dirNormalized = dir.normalized;

        // Position the fire point at the specified distance toward the mouse
        // Ensure the firePoint's Y axis points toward the mouse (2D)
        _firePoint.SetPositionAndRotation(transform.position + dirNormalized * _firePointDistance, Quaternion.LookRotation(Vector3.forward, dirNormalized));
    }

   private void Fire()
   {
        // Don't fire if clicking on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
            
        if (!_isFiring || _fireCooldown > 0f|| _playerHealth.CurrentHealth <= _bulletCost )
            return;
        
        AudioManager.Instance?.PlaySFX("Player Shoot");
        Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
        ParticleManager.Instance.PlayParticle("Muzzle Flash", _firePoint.position, _firePoint.rotation);
        _playerHealth.TakeDmg(_bulletCost);
        _fireCooldown = _fireRate;
   }

   private void Firing(bool isFiring)
   {
       _isFiring = isFiring;
   }
}
