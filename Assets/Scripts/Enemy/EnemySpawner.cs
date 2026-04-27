using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(25f, 15f);

    [Header("Spawn Rate Progression")]
    [Tooltip("Every X seconds the spawnInterval will be reduced by spawnIntervalDecrease (down to minSpawnInterval).")]
    [SerializeField] private float spawnIncreaseInterval = 30f;
    [Tooltip("Amount to decrease spawnInterval each increase step.")]
    [SerializeField] private float spawnIntervalDecrease = 0.2f;
    [Tooltip("Lower bound for spawnInterval.")]
    [SerializeField] private float minSpawnInterval = 0.25f;

    private float timer;
    private float increaseTimer;

    private void Update()
    {
        float dt = Time.deltaTime;
        timer += dt;
        increaseTimer += dt;

        // Spawn logic
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }

        // Increase spawn rate (i.e. reduce spawnInterval) after set amount of time
        if (increaseTimer >= spawnIncreaseInterval)
        {
            // Reduce spawn interval but don't go below minimum
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrease);
            increaseTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        Vector2 spawnPos = GetRandomPointInArena();
        Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], spawnPos, Quaternion.identity);
    }

    private Vector2 GetRandomPointInArena()
    {
        // Pick a completely random point within the defined Box bounds, relative to the spawner's actual position
        float randomX = UnityEngine.Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomY = UnityEngine.Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);

        return (Vector2)transform.position + new Vector2(randomX, randomY);
    }

    // Visualize the Arena size in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0f));
    }
}
