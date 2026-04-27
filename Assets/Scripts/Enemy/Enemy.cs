using Singletons;
using UnityEngine;
using Health;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private int _enemyDamage = 10;
    [SerializeField] private GameObject _healthOrb;
    [SerializeField] private Vector2Int _orbCountRange = new Vector2Int(1, 2);
    [SerializeField] private int _scoreIncrease = 5;
    [SerializeField] private float _orbSpawnRadius = 1f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private LayerMask _playerLayer;

    private Transform _targetTransform;

    public Vector3 Direction { get; private set; }
    public bool IsAttacking { get; private set; }

    // Set when the enemy is intentionally killed during gameplay.
    private bool _shouldSpawnOrb = false;

    private void Start()
    {
        FindNearestTarget();
    }

    private void FindNearestTarget()
    {
        // First try to find any "Agent" (for AI vs AI mode)
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
        
        if (agents.Length > 0)
        {
            float nearestDist = float.MaxValue;
            Transform nearest = null;

            foreach(var agent in agents)
            {
                // Basic check to see if they are alive (has HealthSystem and health > 0)
                var hs = agent.GetComponent<HealthSystem>();
                if (hs != null && hs.CurrentHealth > 0)
                {
                    float dist = Vector3.Distance(transform.position, agent.transform.position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = agent.transform;
                    }
                }
            }
            if (nearest != null)
            {
                _targetTransform = nearest;
                return;
            }
        }
        
        // Fallback to "Player" (for single player mode)
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var hs = player.GetComponent<HealthSystem>();
            if (hs != null && hs.CurrentHealth > 0)
            {
                _targetTransform = player.transform;
            }
        }
    }

    private void Update()
    {
        // Periodically re-evaluate target if there are multiple agents, or target is dead
        if (Time.frameCount % 30 == 0 || _targetTransform == null) 
        {
             FindNearestTarget();
        }

        MoveTowardsTarget();
        InAttackRange();

        // Visualize direction (green) for quick debugging
        Debug.DrawRay(transform.position, Direction * 1.0f, Color.green);
    }

    private void MoveTowardsTarget()
    {
        if (_targetTransform == null) return;

        // Always update facing direction so animations are correct
        Direction = (_targetTransform.position - transform.position).normalized;

        // But only move when not attacking
        if (IsAttacking) return;

        transform.position += Direction * _moveSpeed * Time.deltaTime;
    }

    private void InAttackRange()
    {
        // Use Physics2D.OverlapCircle with the configured LayerMask to detect targets.
        // We will expand this layer mask in the editor to include our Agents.
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _attackRange, _playerLayer.value);
        IsAttacking = hit != null;
    }

    public void Attack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _attackRange, _playerLayer.value);
        if (hit == null) return;

        // Robustly find the HealthSystem on the hit collider or nearby in the hierarchy
        var playerHealth = hit?.GetComponentInParent<HealthSystem>();                    

        if (playerHealth == null)
        {
            Debug.LogWarning($"Enemy.Attack: HealthSystem not found on '{hit.gameObject.name}'. Ensure player's HealthSystem is on the collider or parent.");
            return;
        }

        // Calculate direction from enemy to hit point and use the opposite direction + 90 degrees for the particle rotation.
        Vector3 hitPos = hit.transform.position;
        Vector3 hitDir = (hitPos - transform.position);

        Quaternion rot;
        if (hitDir.sqrMagnitude <= Mathf.Epsilon)
        {
            rot = Quaternion.identity;
        }
        else
        {
            Vector3 opposite = -hitDir.normalized;

            // Rotate the opposite vector by +90 degrees around Z (perpendicular)
            // For 2D, rotating a vector (x,y) by +90 degrees gives (-y, x).
            Vector3 rotated = new Vector3(opposite.y, -opposite.x, 0f);

            // Orient particle so its "up" points to the rotated vector.
            rot = Quaternion.LookRotation(Vector3.forward, rotated);
        }

        AudioManager.Instance?.PlaySFX("Player Hit");
        ParticleManager.Instance.PlayParticle("Player Hurt", hitPos, rot);
        playerHealth.TakeDmg(_enemyDamage);
    }

    public void Die()
    {
        _shouldSpawnOrb = true;
        // Score is no longer awarded here based on bullet attribution.
        // It is awarded when the Agent collects the dropped Health Orbs.
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Only spawn orbs if the enemy was intentionally killed during gameplay.
        // Also guard against editor/playmode quirks by ensuring the application is playing.
        if (!Application.isPlaying || !_shouldSpawnOrb)
            return;

        DropHealthOrb();
    }

    private void DropHealthOrb()
    {
        if (_healthOrb == null) 
            return;

        // Ensure the min is not greater than the max
        int min = Mathf.Min(_orbCountRange.x, _orbCountRange.y);
        int max = Mathf.Max(_orbCountRange.x, _orbCountRange.y);

        // Random.Range for ints is maxExclusive, so add +1 to include the max value.
        int count = Random.Range(min, max + 1);

        for (int i = 0; i < count; i++)
        {
            // random position around the enemy within a circle on the XY plane
            Vector2 offset2D = Random.insideUnitCircle * _orbSpawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset2D.x, offset2D.y, 0f);
            Instantiate(_healthOrb, spawnPos, Quaternion.identity);
        }
    }

    // Draw the attack range in the Scene view when the enemy is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
