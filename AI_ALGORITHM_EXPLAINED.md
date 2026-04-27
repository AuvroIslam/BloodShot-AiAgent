# BloodShot AI Explained (Simple Version)

This file explains what AI is currently used in the game, which parameters control behavior, and which values change dynamically during play.

## 1) Which AI algorithms are used right now?

In the current codebase, there are two agent decision algorithms:

1. GreedyAgent
2. MinimaxAgent (implemented as a simplified one-step heuristic lookahead, not a full deep minimax tree)

Both inherit from AIAlgorithm and are executed by AIGoblinController every frame.

## 2) High-level AI flow per frame

For each AI goblin:

1. AIGoblinController.Update runs.
2. It updates cooldown timers.
3. It calls aiLogic.DecideAction().
4. The algorithm decides movement direction and optional shoot target.
5. Controller applies movement, rotates fire point, and fires if allowed.

So the algorithm chooses intent, and the controller enforces game rules (fire rate, health cost, etc.).

## 3) Core game rule that strongly affects AI

Health is also ammo.

- Every shot costs health (bullet cost).
- If health is too low, the AI cannot shoot.
- Health orbs restore health.

This creates a risk-reward loop:

- shoot more -> score chance increases but self-health drops
- collect more orbs -> survive longer and shoot more later

## 4) Algorithm details

## 4.1 GreedyAgent (local decisions)

GreedyAgent chooses immediate best actions using current nearest enemy/orb.

Decision logic:

1. Find closest enemy and closest health orb.
2. If health is below low-health threshold, prioritize orb.
3. Otherwise use enemy distance:
   - too close -> flee
   - too far -> chase
   - in mid range -> hold/strafe
4. Shoot if enemy is within attack distance and health is above shoot threshold.
5. If no strong target, wander around starting position with noise.

Behavior style:

- fast and reactive
- no deep future planning

## 4.2 MinimaxAgent (simplified heuristic lookahead)

This is not a full multi-depth minimax tree. It is a real-time approximation:

1. Build a small set of candidate movement directions (8 directions + stand still).
2. For each candidate, simulate a short future position.
3. Score that simulated state with heuristic terms.
4. Pick the direction with highest score.
5. Independently decide shooting using distance + health condition.

State score (conceptual):

score = enemy-threat-term + orb-proximity-term + opponent-distance-term + health-term

Where the terms are weighted by inspector parameters.

Behavior style:

- more evaluative than Greedy
- still lightweight enough for per-frame use

## 5) Parameters used by AI

Below are the important parameters and what they do.

## 5.1 GreedyAgent parameters (tunable)

- lowHealthThreshold
  - If current health is below this, orb collection is prioritized.
- safeDistance
  - If enemy is closer than this, agent retreats.
- attackDistance
  - If enemy is farther than this, agent approaches.
  - Also used as shooting range gate.

## 5.2 MinimaxAgent parameters (tunable)

- scoreWeight
  - Present but currently not used in EvaluateState.
- healthWeight
  - Multiplies health and orb-benefit terms.
- opponentDistanceWeight
  - Penalizes/rewards distance to opponent depending on sign.
  - Current value is negative, so being farther lowers score.
- enemyThreatWeight
  - Multiplies near-enemy penalty term.
  - Current value is negative, so threat penalty reduces final score.

## 5.3 AIGoblinController parameters (tunable gameplay constraints)

- moveSpeed
  - Final movement speed.
- fireRate
  - Minimum time between shots.
- bulletCost
  - Health spent per shot.
- firePointDistance
  - Spawn offset of bullet from agent center.
- bulletPrefab / firePoint / aiLogic
  - Required references for shooting and decision logic.

These parameters do not choose strategy directly, but they heavily change strategy outcomes.

## 6) Dynamic parameters (change during runtime)

These are the most important dynamic values the AI reads or is affected by:

- currentHealth (agent health)
- fireCooldown (can shoot now or not)
- currentMoveDirection
- nearest enemy position
- nearest orb position
- enemy count and orb count
- distance to enemy/orb/opponent
- target alive/dead state
- time-based values (for wander noise and cooldown updates)

Dynamic environment systems that indirectly affect AI performance:

- EnemySpawner increases spawn pressure over time by reducing spawn interval.
- Enemy targeting picks nearest alive agent/player.
- HealthOrb attraction and pickup restore health and add score.

## 7) What metrics show AI effect on game outcome?

In this project, AI affects match result through:

- survival duration (staying alive)
- ability to keep health economy stable (shooting vs orb recovery)
- orb collection efficiency
- kill conversion (enemy pressure control)
- score gain via orb pickups after enemy deaths
- final winner when one agent remains alive

## 8) Quick comparison: Greedy vs Simplified Minimax

GreedyAgent:

- simpler rules
- very fast reaction
- can be short-sighted

MinimaxAgent (simplified):

- evaluates multiple candidate moves each frame
- considers combined heuristic terms
- can choose better positioning in complex situations

## 9) Important implementation notes

1. The project report mentions full minimax/alpha-beta/MCTS and A* concepts, but the current gameplay code currently runs:
   - GreedyAgent
   - Simplified one-step MinimaxAgent
   - Enemy nearest-target chase (no explicit A* grid/path code in current Enemy.cs)

2. Shooting has hard constraints in controller:
   - blocked by cooldown
   - blocked by low health (health must be greater than bullet cost)

3. Because of health-as-ammo, aggressive policies can lose even with good aim if orb routing is poor.

## 10) If you want to present this in class/report

Use this short line:

The game compares a rule-based Greedy policy and a lightweight heuristic Minimax-style policy under a health-as-ammo economy, where dynamic state variables such as health, enemy distance, orb distance, and cooldown directly drive per-frame action selection.
