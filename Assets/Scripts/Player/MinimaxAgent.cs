using UnityEngine;

/// <summary>
/// A simplified, real-time approximation of a Minimax search.
/// True Minimax in a continuous 2D space requires heavy discretization (grids) and deep tree copies.
/// To keep real-time performance, this agent evaluates a set of discrete "candidate actions" 
/// 1-step ahead and picks the one minimizing the opponent's max advantage.
/// </summary>
public class MinimaxAgent : AIAlgorithm
{
    [Header("Heuristic Weights")]
    [SerializeField] private float scoreWeight = 1.0f;
    [SerializeField] private float healthWeight = 2.0f;
    [SerializeField] private float opponentDistanceWeight = -0.5f; // Prefers staying away from opponent
    [SerializeField] private float enemyThreatWeight = -1.5f; // Penalizes being near enemies
    
    // We cache finding the opponent once per game (or periodically)
    private Transform _opponent;

    public override void DecideAction()
    {
        if (_opponent == null)
        {
            FindOpponent();
        }

        GameObject[] enemies = new GameObject[0];
        GameObject[] orbs = new GameObject[0];

        try { enemies = GameObject.FindGameObjectsWithTag("Enemy"); } catch { }
        try { orbs = GameObject.FindGameObjectsWithTag("HealthOrb"); } catch { }

        // Generate discrete movement options (Current pos, Up, Down, Left, Right)
        Vector2[] moveDirections = {
            Vector2.zero,
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            new Vector2(1,1).normalized,
            new Vector2(-1,1).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(-1,-1).normalized
        };

        Vector2 bestMove = Vector2.zero;
        float bestScore = float.MinValue;
        Transform targetToShoot = null;

        // If no enemies and no orbs, just move towards starting area
        if (enemies.Length == 0 && orbs.Length == 0)
        {
            bestMove = (StartingPosition - (Vector2)transform.position).normalized;
        }
        else
        {
            // PHYSICAL PROOF: Draw a blue line to the nearest orb if one exists
            Transform closestOrb = GetClosest((Vector2)transform.position, orbs);
            if (closestOrb != null)
            {
                Debug.DrawLine(transform.position, closestOrb.position, Color.blue);
            }

            // Simulate each move
            foreach (Vector2 dir in moveDirections)
            {
                // Position if we take this move (simulating 1 second ahead at arbitrary speed)
                Vector2 simulatedPos = (Vector2)transform.position + (dir * 2f); 
                
                float val = EvaluateState(simulatedPos, enemies, orbs);

                if (val > bestScore)
                {
                    bestScore = val;
                    bestMove = dir;
                }
            }
        }

        // Combat decision: Evaluate taking a shot
        // We only shoot if it yields a positive expected value compared to saving the health
        Transform closestEnemy = GetClosest(transform.position, enemies);
        if (closestEnemy != null)
        {
            float distToEnemy = Vector2.Distance(transform.position, closestEnemy.position);
            // Simple heuristic to shoot
            if (distToEnemy < 8f && Health.CurrentHealth > 20)
            {
                targetToShoot = closestEnemy;
            }
        }

        Controller.Move(bestMove);

        if (targetToShoot != null)
        {
            Controller.ShootAt(targetToShoot.position);
        }
    }

    private float EvaluateState(Vector2 position, GameObject[] enemies, GameObject[] orbs)
    {
        float score = 0;

        // 1. Health/Safety (Distance from enemies)
        float threatPenalty = 0;
        foreach(var enemy in enemies)
        {
             float dist = Vector2.Distance(position, enemy.transform.position);
             if (dist < 3f) {
                 threatPenalty += (3f - dist) * 10f; // High penalty for touching
             } else if (dist < 6f) {
                 threatPenalty += (6f - dist) * 2f; 
             }
        }
        score += threatPenalty * enemyThreatWeight;

        // 2. Resource Gathering (Distance to orbs)
        Transform closestOrb = GetClosest(position, orbs);
        if (closestOrb != null)
        {
             float dist = Vector2.Distance(position, closestOrb.position);
             score += (10f / (dist + 0.1f)) * healthWeight; // Closer = much higher score
        }

        // 3. Opponent Minimization (Keep distance, or close in if dominating)
        if (_opponent != null)
        {
             float distToOpponent = Vector2.Distance(position, _opponent.position);
             score += distToOpponent * opponentDistanceWeight; 
        }

        // 4. Base survival
        score += Health.CurrentHealth * healthWeight;

        return score;
    }

    private Transform GetClosest(Vector2 pos, GameObject[] targets)
    {
        Transform closest = null;
        float minDistance = float.MaxValue;
        
        foreach (var t in targets)
        {
            if (t == null) continue;
            float dist = Vector2.SqrMagnitude(pos - (Vector2)t.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = t.transform;
            }
        }
        return closest;
    }

    private void FindOpponent()
    {
        var agents = GameObject.FindGameObjectsWithTag("Agent");
        foreach(var agent in agents)
        {
             if (agent != this.gameObject)
             {
                 _opponent = agent.transform;
                 break;
             }
        }
    }
}
