using UnityEngine;
using Health;
using Singletons;

public class AIGoblinController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _firePointDistance = 0.6f;
    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private int _bulletCost = 5;

    [Tooltip("The specific AI logic component attached to this agent (e.g., GreedyAgent or MinimaxAgent)")]
    [SerializeField] private AIAlgorithm _aiLogic;

    private float _fireCooldown = 0f;
    private float _moveSoundTimer = 0f;
    private float _moveSoundInterval = 0.4f;

    private HealthSystem _healthSystem;
    private Rigidbody2D _rb;

    public Vector2 CurrentMoveDirection { get; private set; }

    private void Start()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _rb = GetComponent<Rigidbody2D>();
        
        if (_rb != null)
        {
             // Force unfreeze position just in case, keep rotation frozen for 2D
             _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
             _rb.bodyType = RigidbodyType2D.Dynamic; // Ensure it is dynamic
        }
        if (_healthSystem == null)
        {
            Debug.LogError("AIGoblinController: No HealthSystem found.");
        }

        if (_aiLogic == null)
        {
            _aiLogic = GetComponent<AIAlgorithm>();
            if (_aiLogic == null)
            {
                 Debug.LogWarning("AIGoblinController: No AIAlgorithm attached. The agent will not do anything.");
            }
        }

        if (_aiLogic != null)
        {
            _aiLogic.Initialize(this, _healthSystem);
        }
    }

    private void Update()
    {
        if (_healthSystem.CurrentHealth <= 0) return;

        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
        }

        if (_moveSoundTimer > 0f)
        {
            _moveSoundTimer -= Time.deltaTime;
        }

        // Ask the attached algorithm to make decisions
        if (_aiLogic != null)
        {
            _aiLogic.DecideAction();
        }
        else
        {
             // If NO logic is attached, the agent should still wander to show it works
             Move(new Vector2(Mathf.Sin(Time.time), Mathf.Cos(Time.time)));
        }

        // Apply visual updates (sound, flipping, firepoint)
        ApplyVisualEffects();
        UpdateFirePoint();

        // Debug draw to show current movement intent in Scene View
        Debug.DrawRay(transform.position, (Vector3)CurrentMoveDirection * 2f, Color.green);
    }

    private void FixedUpdate()
    {
        if (_healthSystem != null && _healthSystem.CurrentHealth <= 0)
        {
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
            return;
        }

        ApplyMovement();
    }

    /// <summary>
    /// Sets the current movement direction. Called by the AIAlgorithm.
    /// </summary>
    public void Move(Vector2 direction)
    {
        CurrentMoveDirection = direction.normalized;
    }

    public void ShootAt(Vector3 targetPosition)
    {
        _lastTargetPosition = targetPosition;
        _hasShootTarget = true;
        UpdateFirePoint();
        TryFire();
    }

    private void ApplyMovement()
    {
        Vector2 moveStep = CurrentMoveDirection * _moveSpeed;

        if (_rb != null)
        {
            // MovePosition is more reliable for script-driven physics objects
            _rb.MovePosition(_rb.position + moveStep * Time.fixedDeltaTime);
            
            // Log once every 60 frames to avoid spamming, but verify movement is happening
            if (Time.frameCount % 60 == 0 && CurrentMoveDirection.sqrMagnitude > 0.01f)
            {
                Debug.Log($"[AI Move] {gameObject.name} moving: {CurrentMoveDirection} at velocity {moveStep}");
            }
        }
        else
        {
            // Fallback for objects without Rigidbody
            transform.position += (Vector3)moveStep * Time.deltaTime;
        }
    }

    private void ApplyVisualEffects()
    {
        bool isMoving = CurrentMoveDirection.sqrMagnitude > 0.01f;

        if (isMoving && _moveSoundTimer <= 0f && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("Player Move");
            _moveSoundTimer = _moveSoundInterval;
        }

        // Basic sprite flipping logic based on movement direction
        if (isMoving)
        {
            float sign = Mathf.Sign(CurrentMoveDirection.x);
            Vector3 currentScale = transform.localScale;
            if ((sign < 0 && currentScale.x > 0) || (sign > 0 && currentScale.x < 0))
            {
                currentScale.x *= -1;
                transform.localScale = currentScale;
            }
        }
    }

    // Reference variables to store target position for UpdateFirePoint
    private Vector3 _lastTargetPosition;
    private bool _hasShootTarget;

    private void UpdateFirePoint()
    {
        if (!_hasShootTarget || _firePoint == null) return;
        
        Vector3 targetWorldPosition = _lastTargetPosition;
        targetWorldPosition.z = transform.position.z;

        Vector3 dir = targetWorldPosition - transform.position;
        if (dir.sqrMagnitude <= Mathf.Epsilon)
            return;

        Vector3 dirNormalized = dir.normalized;
        
        _firePoint.SetPositionAndRotation(
            transform.position + dirNormalized * _firePointDistance, 
            Quaternion.LookRotation(Vector3.forward, dirNormalized)
        );
    }

    private void TryFire()
    {
        if (_healthSystem == null || _fireCooldown > 0f || _healthSystem.CurrentHealth <= _bulletCost)
            return;

        if (_bulletPrefab != null && _firePoint != null)
        {
            AudioManager.Instance?.PlaySFX("Player Shoot");
            
            GameObject bulletObj = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            var bullet = bulletObj.GetComponent<Weapon.Bullet>();
            if (bullet != null) bullet.Shooter = gameObject;

            if (ParticleManager.Instance != null)
            {
                ParticleManager.Instance.PlayParticle("Muzzle Flash", _firePoint.position, _firePoint.rotation);
            }
            
            _healthSystem.TakeDmg(_bulletCost);
            _fireCooldown = _fireRate;
        }
        else
        {
            Debug.LogWarning($"AIGoblinController on {gameObject.name}: Cannot shoot! BulletPrefab or FirePoint is missing in Inspector.");
        }
    }
}
