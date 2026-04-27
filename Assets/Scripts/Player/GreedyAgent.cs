using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Health;

public class GreedyAgent : AIAlgorithm
{
    [Header("Greedy Evaluation Weights")]
    [SerializeField] private float _lowHealthThreshold = 40f;
    [SerializeField] private float _safeDistance = 5f;
    [SerializeField] private float _attackDistance = 8f;

    public override void DecideAction()
    {
        // 1. Gather environmental data
        GameObject[] enemies = new GameObject[0];
        GameObject[] orbs = new GameObject[0];

        try { enemies = GameObject.FindGameObjectsWithTag("Enemy"); } catch { }
        try { orbs = GameObject.FindGameObjectsWithTag("HealthOrb"); } catch { }

        Transform closestEnemy = GetClosest(enemies);
        Transform closestOrb = GetClosest(orbs);

        Vector2 movementTarget = Vector2.zero;
        Vector3? shootTarget = null;
        bool shouldShoot = false;

        // 2. Logic: Prioritize Health if low, OR if an orb is extremely close
        bool needsHealth = Health.CurrentHealth < _lowHealthThreshold;
        
        if (closestOrb != null)
        {
            // PHYSICAL PROOF: Draw a blue line to the orb in Scene View
            Debug.DrawLine(transform.position, closestOrb.position, Color.blue);

            float distToOrb = Vector2.Distance(transform.position, closestOrb.position);
            if (needsHealth || distToOrb < 2f) 
            {
                movementTarget = (closestOrb.position - transform.position).normalized;
            }
        }

        // 3. Logic: Combat positioning and shooting
        if (closestEnemy != null)
        {
            float distToEnemy = Vector2.Distance(transform.position, closestEnemy.position);
            
            // If we aren't strongly pulled by health, handle enemy spacing
            if (movementTarget == Vector2.zero)
            {
                if (distToEnemy < _safeDistance)
                {
                    // Flee (move away from enemy)
                    movementTarget = (transform.position - closestEnemy.position).normalized;
                }
                else if (distToEnemy > _attackDistance)
                {
                    // Pursue (move towards enemy)
                    movementTarget = (closestEnemy.position - transform.position).normalized;
                }
                else
                {
                    // Kite (circle around or stay put)
                    // For simplicity, just small random strafes or hold ground
                    movementTarget = Vector2.zero; 
                }
            }

            // Decide whether to shoot
            // Greedy favors shooting if we have enough health and an enemy is in range
            if (distToEnemy <= _attackDistance && Health.CurrentHealth > 15f)
            {
                shouldShoot = true;
                shootTarget = closestEnemy.position;
            }
        }

        // Fallback: Wander if nothing else to do
        if (movementTarget == Vector2.zero)
        {
             // Wander around the starting position to stay in local area
             Vector2 dirToStart = (StartingPosition - (Vector2)transform.position).normalized;
             
             // Add some noise to make it look less robotic
             Vector2 noise = new Vector2(Mathf.Sin(Time.time * 2f), Mathf.Cos(Time.time * 2f)).normalized;
             
             movementTarget = (dirToStart * 0.5f + noise * 0.5f).normalized;
        }

        // 4. Send commands to controller
        Controller.Move(movementTarget);

        if (shouldShoot && shootTarget.HasValue)
        {
            Controller.ShootAt(shootTarget.Value);
        }
    }

    private Transform GetClosest(GameObject[] targets)
    {
        Transform closest = null;
        float minDistance = float.MaxValue;
        
        foreach (var t in targets)
        {
            if (t == null) continue;
            
            // Check if it's an agent or enemy, make sure it's alive
            HealthSystem hs = null;
            try { hs = t.GetComponent<HealthSystem>(); } catch {}
            
            if (hs != null && !hs.IsAlive) continue;

            float dist = Vector2.SqrMagnitude((Vector2)transform.position - (Vector2)t.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = t.transform;
            }
        }
        return closest;
    }
}
