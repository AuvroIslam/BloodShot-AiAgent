# BloodShot: AI Agent Competition

## Project Report

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [Game Overview](#2-game-overview)
3. [Core Game Mechanics](#3-core-game-mechanics)
4. [AI Architecture](#4-ai-architecture)
5. [Agent 1: Mini-Max Algorithm](#5-agent-1-mini-max-algorithm)
6. [Agent 2: Alternative Algorithm](#6-agent-2-alternative-algorithm)
7. [Enemy AI: A* Pathfinding](#7-enemy-ai-a-pathfinding)
8. [Evaluation Metrics](#8-evaluation-metrics)
9. [Conclusion](#9-conclusion)

---

## 1. Introduction

This project presents the design and implementation of an AI-driven competitive game built using the Unity Game Engine. The game, titled **BloodShot**, features two AI-controlled goblin agents competing against each other while fighting waves of human soldiers. The project explores and compares different artificial intelligence algorithms for game-playing agents, with a focus on decision-making under resource constraints.

### 1.1 Project Objectives

- Develop a top-down 2D action game with a unique health-as-ammunition mechanic
- Implement two competing AI agents using different algorithmic approaches
- Create intelligent enemy behavior using pathfinding algorithms
- Analyze and compare the performance of different AI strategies
- Demonstrate practical applications of classical AI algorithms in game development

### 1.2 Technology Stack

- Game Engine: Unity
- Programming Language: C#
- Target Platform: Windows / WebGL
- AI Algorithms: Mini-Max, Alpha-Beta Pruning, Greedy, MCTS, A*

---

## 2. Game Overview

BloodShot is a top-down 2D shooter game inspired by the survival-action genre, similar to games like Vampire Survivors. The game features a distinctive twist where ammunition is directly tied to the player's life force, creating a strategic trade-off between offense and survival.

### 2.1 Game Concept

In BloodShot, two AI-controlled goblin characters compete against each other on the same battlefield. Both goblins must:

- Fight against waves of human soldiers that spawn continuously
- Manage their health resources carefully since shooting costs health
- Collect souls dropped by defeated enemies to restore health
- Accumulate points by eliminating enemies

The goblin with the highest score at the end of the game session wins the competition.

---

## 3. Core Game Mechanics

### 3.1 Health-as-Ammunition System

The central mechanic of BloodShot is the **Blood Cost** system. Every bullet fired by a goblin agent costs a portion of its health points, creating a fundamental trade-off between offense and survival. This unique constraint forces AI agents to balance aggression with self-preservation, making strategic decisions about when to shoot and when to conserve health.

### 3.2 Enemy System

Human soldiers spawn continuously and pursue the goblin agents. They serve as both threats and opportunities, as defeating them provides score points and health orbs for recovery.

### 3.3 Health Restoration

When enemies are defeated, they drop health orbs (souls) that goblins can collect to restore their health. These orbs automatically move toward nearby goblins, creating strategic decisions about positioning and resource collection.

### 3.4 Victory Condition

The competition ends when one goblin dies. The surviving goblin, or the goblin with the highest score, wins the match.

---

## 4. AI Architecture

The AI architecture consists of three distinct intelligent components working simultaneously:

- **Agent 1:** Goblin using Mini-Max algorithm
- **Agent 2:** Goblin using an alternative algorithm
- **Human Soldiers:** NPCs using A* pathfinding

Each AI agent observes the current game state (positions, health values, scores, orb locations) and makes decisions about movement and shooting actions in real-time.

---

## 5. Agent 1: Mini-Max Algorithm

### 5.1 Algorithm Overview

The Mini-Max algorithm is a decision-making algorithm used in two-player competitive games. It operates by building a game tree and evaluating future states to determine the optimal move, assuming both players play optimally. The algorithm explores possible future scenarios by alternating between maximizing the agent's advantage and minimizing the opponent's advantage.

In BloodShot, the Mini-Max agent:
- **Maximizes:** Its own score and survival chances
- **Minimizes:** The opponent goblin's advantages

### 5.2 Search Depth

Due to the real-time nature of the game, the search depth is limited to ensure quick decision-making. The agent typically looks ahead 2-3 moves, balancing strategic planning with computational efficiency.

### 5.3 State Evaluation

The algorithm evaluates each potential game state using a heuristic function that considers:

- **Health Differential:** Comparison of own health vs opponent health
- **Score Differential:** Comparison of current scores
- **Orb Proximity:** Distance to collectible health orbs
- **Enemy Threat Level:** Number and position of nearby enemies
- **Shooting Opportunities:** Available targets and line of sight

### 5.4 Decision Making

The Mini-Max agent considers multiple factors when choosing actions:

**Movement Decisions:**
- Positioning relative to health orbs
- Distance from dangerous enemies
- Strategic positioning against opponent
- Map boundary awareness

**Shooting Decisions:**
- Current health sufficiency for shooting
- Target accuracy probability
- Expected value of taking the shot
- Nearby health orbs for recovery

### 5.5 Optimizations

To improve real-time performance:
- **Alpha-Beta Pruning:** Eliminates unnecessary branches in the search tree
- **Move Ordering:** Evaluates most promising moves first
- **Time-Limited Search:** Returns best solution within time constraint

---

## 6. Agent 2: Alternative Algorithm

Agent 2 serves as a competitive benchmark against the Mini-Max agent. Three algorithmic approaches are considered:

### 6.1 Option A: Greedy Algorithm

The Greedy algorithm makes locally optimal choices at each decision point without considering future consequences. It prioritizes immediate gains such as nearby health orbs when health is low, or shooting opportunities when health is sufficient.

**Characteristics:**
- Extremely fast computation with no search overhead
- Good baseline performance for simple scenarios
- Cannot plan ahead or anticipate strategic situations
- May miss opportunities requiring multi-step planning

### 6.2 Option B: Alpha-Beta Pruning

Alpha-Beta pruning is an optimization of the Mini-Max algorithm that significantly reduces the number of nodes evaluated while maintaining optimal decision quality. It eliminates branches of the search tree that cannot possibly influence the final decision, allowing deeper search within the same time budget.

**Characteristics:**
- Maintains optimality of Mini-Max decisions
- Allows deeper lookahead within time constraints
- More efficient than standard Mini-Max
- Particularly effective with good move ordering

### 6.3 Option C: Monte Carlo Tree Search

MCTS is a heuristic search algorithm that builds a search tree using random sampling of the decision space. It balances exploration of new possibilities with exploitation of known good strategies through four phases: selection, expansion, simulation, and backpropagation.

**Characteristics:**
- Handles large action spaces effectively
- Returns best solution found when time expires
- Does not require explicit evaluation function
- Balances exploration and exploitation naturally
- Good for handling uncertainty in game outcomes

### 6.4 Algorithm Comparison

| Criterion | Greedy | Alpha-Beta | MCTS |
|-----------|--------|------------|------|
| Computation Speed | Very Fast | Moderate | Moderate |
| Planning Depth | None | Fixed Depth | Adaptive |
| Optimality | Local Only | Global | Probabilistic |
| Handling Uncertainty | Poor | Moderate | Good |
| Implementation | Simple | Medium | Complex |

---

## 7. Enemy AI: A* Pathfinding

### 7.1 Algorithm Overview

The human soldier enemies use the A* (A-Star) pathfinding algorithm to navigate the game environment and pursue goblin targets. A* is an informed search algorithm that finds the shortest path between two points by combining actual distance traveled with estimated distance remaining.

### 7.2 How A* Works

A* evaluates paths using a cost function: f(n) = g(n) + h(n)

- **g(n):** Actual cost from start to current position
- **h(n):** Estimated cost from current position to goal (using Euclidean distance)

The algorithm maintains a priority queue of positions to explore, always expanding the most promising path first. This ensures finding the shortest path while avoiding unnecessary exploration of suboptimal routes.

### 7.3 Game Integration

The game world is represented as a navigation grid where each cell can be walkable or blocked by obstacles. Enemies use A* to:

- Calculate paths around buildings and obstacles
- Pursue the nearest goblin target
- Navigate to optimal attack positions

### 7.4 Dynamic Pathfinding

Since goblins move continuously, soldiers periodically recalculate their paths to ensure they track moving targets effectively. Path recalculation is triggered at regular intervals or when the target has moved significantly.

### 7.5 Optimizations

Several optimizations improve pathfinding performance:
- Path smoothing to create natural-looking movement
- Spatial partitioning for efficient obstacle detection
- Path caching to avoid redundant calculations

---

## 8. Evaluation Metrics

To compare the effectiveness of different AI algorithms, the following metrics are tracked:

| Metric | Description |
|--------|-------------|
| Win Rate | Percentage of games won against opponent |
| Average Score | Mean score achieved across multiple games |
| Survival Time | Average duration the agent stays alive |
| Kill Count | Number of enemies eliminated |
| Accuracy | Percentage of shots that hit targets |
| Health Efficiency | Score gained per health point spent |
| Decision Time | Computational time per decision |

### 8.1 Comparison Methodology

Multiple matches are conducted between algorithm pairs with positions swapped to eliminate bias. Statistical analysis determines which algorithm performs better across various metrics. The goal is to identify which AI approach provides the best balance of strategic play, survival, and scoring.

### 8.2 Expected Outcomes

| Algorithm | Expected Strength | Expected Weakness |
|-----------|-------------------|-------------------|
| Mini-Max | Strategic planning and anticipation | Computation time at deeper depths |
| Greedy | Speed and reactivity | Short-sighted decisions |
| Alpha-Beta | Deep planning efficiently | Still depth-limited |
| MCTS | Adaptability to uncertainty | Variance in results |

---

## 9. Conclusion

### 9.1 Summary

This project demonstrates the application of classical AI algorithms in a real-time competitive game environment. By building BloodShot from the ground up with integrated AI systems, the project showcases:

- Strategic decision-making under resource constraints
- Competitive AI agent behavior using game theory
- Intelligent enemy navigation using pathfinding
- Performance comparison framework for AI algorithms

The unique health-as-ammunition mechanic creates an ideal testing environment for AI algorithms, as agents must balance immediate tactical decisions with long-term strategic planning.

### 9.2 Key Contributions

- Implementation of Mini-Max algorithm in a resource-constrained action game
- Comparative framework for evaluating multiple AI decision-making approaches
- Integration of A* pathfinding for dynamic enemy behavior
- Comprehensive evaluation methodology for AI performance analysis

### 9.3 Future Work

- Neural network-based learning agents
- Reinforcement learning approaches
- Multi-agent coordination strategies
- Adaptive difficulty systems

---

## References

1. Russell, S., and Norvig, P. "Artificial Intelligence: A Modern Approach." 4th Edition, 2020.
2. Hart, P. E., Nilsson, N. J., and Raphael, B. "A Formal Basis for the Heuristic Determination of Minimum Cost Paths." IEEE Transactions, 1968.
3. Browne, C. et al. "A Survey of Monte Carlo Tree Search Methods." IEEE Transactions on Computational Intelligence and AI in Games, 2012.
4. Millington, I., and Funge, J. "Artificial Intelligence for Games." 2nd Edition, 2009.

---

**Project Status:** In Development  
**Last Updated:** February 2026  
**Engine Version:** Unity 2022.x LTS
