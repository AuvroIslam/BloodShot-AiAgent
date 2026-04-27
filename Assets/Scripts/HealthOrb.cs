using Singletons;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    [SerializeField] private int _healthRestoreAmount = 8;
    [SerializeField] private float _detectionRadius = 3f;
    [SerializeField] private float _attractSpeed = 5f;
    [SerializeField] private LayerMask _detectionLayer; // use layers for detection

    private Transform _target;
    private bool _isAttracted = false;
    
    // Static cooldown to prevent sound spam when collecting multiple orbs
    private static float _lastPickupSoundTime = 0f;
    private const float SOUND_INTERVAL = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerAttraction();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Agent"))
        {
            // Only play sound if enough time has passed since last pickup
            if (Time.time - _lastPickupSoundTime >= SOUND_INTERVAL)
            {
                Singletons.AudioManager.Instance?.PlaySFX("Health Pickup");
                _lastPickupSoundTime = Time.time;
            }
            
            // Get health system from self or parents
            var healthSystem = collision.GetComponentInParent<Health.HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.Heal(_healthRestoreAmount);
                ParticleManager.Instance.PlayParticle("Orb Pickup", transform.position, Quaternion.identity);

                // Add points based on this specific rule: collection = points
                var agentStats = collision.GetComponentInParent<AgentStats>();
                if (agentStats != null)
                {
                    agentStats.AddScore(5); // Adjust hardcoded 5 if you want a public variable
                }
                else if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(5);
                }

                Destroy(gameObject);
            }
        }
    }
    private void PlayerAttraction()
    {
        if (!_isAttracted)
        {
            // Use the LayerMask to detect player (or other layers) within radius.
            Collider2D hit = Physics2D.OverlapCircle(transform.position, _detectionRadius, _detectionLayer.value);
            if (hit != null)
            {
                _target = hit.transform;
                _isAttracted = true;
            }
        }
        else
        {
            // If we had a target but it's null (destroyed or scene change), stop attraction
            if (_target == null)
            {
                _isAttracted = false;
                return;
            }

            // Move toward the player's current position
            transform.position = Vector3.MoveTowards(transform.position, _target.position, _attractSpeed * Time.deltaTime);
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
