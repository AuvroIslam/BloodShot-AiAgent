using UnityEngine;
using Health;
using Singletons;

public abstract class AIAlgorithm : MonoBehaviour
{
    protected AIGoblinController Controller;
    protected HealthSystem Health;
    protected Vector2 StartingPosition;

    public virtual void Initialize(AIGoblinController controller, HealthSystem health)
    {
        Controller = controller;
        Health = health;
        StartingPosition = transform.position;
    }

    /// <summary>
    /// Evaluates the game state and tells the AIGoblinController what to do.
    /// Called every frame by the AIGoblinController.
    /// </summary>
    public abstract void DecideAction();
}
