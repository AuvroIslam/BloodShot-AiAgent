using Singletons;
using UnityEngine;
using UnityEngine.Events;

namespace Health
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100;
        private UnityEvent<float> _onHealthChanged = new UnityEvent<float>();
        private UnityEvent _onDeath = new UnityEvent();
        private float _currentHealth;

        public UnityEvent<float> OnHealthChanged => _onHealthChanged;
        public UnityEvent OnDeath => _onDeath;

        // Public properties to be used for UI
        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsAlive => _currentHealth > 0;


        public GameObject LastAttacker { get; private set; }

        private void Awake()
        {
            ResetHealth();
        }


        public void TakeDmg(int damageAmount, GameObject source = null)
        {
            if (!IsAlive) return;

            if (source != null)
            {
                LastAttacker = source;
            }

            _currentHealth -= damageAmount;

            // Clamp health at 0
            if (_currentHealth < 0)
                _currentHealth = 0;

            // Notify listeners about health change
            _onHealthChanged.Invoke(_currentHealth);

            // Check if entity has died from this damage
            if (_currentHealth <= 0)
            {
                HandleDeath();
            }
        }
        public void Heal(float healAmount)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Min(_currentHealth + healAmount, _maxHealth);
            _onHealthChanged.Invoke(_currentHealth);
        }
        private void HandleDeath()
        {
            _onDeath.Invoke();
            if (gameObject.CompareTag("Enemy"))
            {
                AudioManager.Instance?.PlaySFX("Enemy Die");
                ParticleManager.Instance.PlayParticle("Enemy Death", transform.position, Quaternion.identity);
                gameObject.GetComponent<Enemy>()?.Die();
            }
            else if (gameObject.CompareTag("Agent"))
            {
                AudioManager.Instance?.PlaySFX("Player Die");
                ParticleManager.Instance.PlayParticle("Enemy Death", transform.position, Quaternion.identity);
                
                // Let AI GameManager know this agent died
                var aiGm = Object.FindFirstObjectByType<AIGameManager>();
                if (aiGm != null)
                {
                    aiGm.ReportAgentDeath(gameObject);
                }
                
                Destroy(gameObject, 0.1f);
            }
            else if (gameObject.CompareTag("Player"))
            {
                AudioManager.Instance?.PlaySFX("Player Die");
                ParticleManager.Instance.PlayParticle("Enemy Death", transform.position, Quaternion.identity);
                
                // Call GameManager to show game over
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ShowGameOver();
                }
                
                Destroy(gameObject, 0.1f);
            }
        }
        private void ResetHealth()
        {
            _currentHealth = _maxHealth;
            _onHealthChanged.Invoke(_currentHealth);
        }
    }
}
