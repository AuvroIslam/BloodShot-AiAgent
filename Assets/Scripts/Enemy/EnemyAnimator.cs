using UnityEngine;

public static class EnemyAnimationState
{
    public const string FrontWalk = "Front_Walking";
    public const string BackWalk = "Back_Walking";
    public const string LeftWalk = "Left_Walking";
    public const string RightWalk = "Right_Walking";
    public const string FrontAttack = "Front_Attacking";
    public const string BackAttack = "Back_Attacking";
    public const string LeftAttack = "Left_Attacking";
    public const string RightAttack = "Right_Attacking";

}

public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;
    private Enemy _enemy;
    private string _currentAnim;

    [SerializeField] private int _smoothFrames = 6; // number of frames to smooth transitions (converted to seconds)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        Animate();
    }

    private void Animate()
    {
        if (_animator == null || _enemy == null) return;

        Vector3 dir = _enemy.Direction;
        bool attacking = _enemy.IsAttacking;

        // Determine primary direction: horizontal if |x| > |y|, otherwise vertical.
        string nextAnim;
        if (attacking)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                nextAnim = dir.x > 0f ? EnemyAnimationState.RightAttack : EnemyAnimationState.LeftAttack;
            }
            else
            {
                nextAnim = dir.y > 0f ? EnemyAnimationState.BackAttack : EnemyAnimationState.FrontAttack;
            }
        }
        else
        {
            // If nearly zero direction, default to front walk.
            if (dir.sqrMagnitude <= 0.001f)
            {
                nextAnim = EnemyAnimationState.FrontWalk;
            }
            else if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                nextAnim = dir.x > 0f ? EnemyAnimationState.RightWalk : EnemyAnimationState.LeftWalk;
            }
            else
            {
                nextAnim = dir.y > 0f ? EnemyAnimationState.BackWalk : EnemyAnimationState.FrontWalk;
            }
        }

        // Only request animation change when needed.
        if (nextAnim != _currentAnim)
        {
            // If current animation is an attack, allow it to finish before changing.
            if (IsAttackState(_currentAnim) && IsCurrentStateStillPlaying(_animator, _currentAnim, layer: 0))
            {
                // still playing attack animation to completion; skip transition
                return;
            }

            _animator.PlayAnimation(nextAnim, _smoothFrames);
            _currentAnim = nextAnim;
        }
    }

    private static bool IsAttackState(string stateName)
    {
        if (string.IsNullOrEmpty(stateName)) return false;
        return stateName == EnemyAnimationState.FrontAttack
            || stateName == EnemyAnimationState.BackAttack
            || stateName == EnemyAnimationState.LeftAttack
            || stateName == EnemyAnimationState.RightAttack;
    }

    private static bool IsCurrentStateStillPlaying(Animator animator, string stateName, int layer = 0)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) return false;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        int stateHash = Animator.StringToHash(stateName);

        // If the current animator state matches the provided state name, check normalizedTime.
        // For non-looping attack clips normalizedTime goes from 0..1 as it plays. We wait until it's (nearly) done.
        if (stateInfo.shortNameHash == stateHash)
        {
            // normalizedTime < 1.0 means the state hasn't finished (for non-looping states).
            // Use a small epsilon to avoid floating point edge cases.
            const float finishThreshold = 0.98f;
            return stateInfo.normalizedTime < finishThreshold;
        }

        return false;
    }
}

public static class AnimatorExtensions
{
    public static void PlayAnimation(this Animator animator, string stateName, int smoothFrames = 6, int layer = 0)
    {
        if (animator == null || string.IsNullOrEmpty(stateName)) return;

        int stateHash = Animator.StringToHash(stateName);

        // Convert frames to seconds (assume 60 FPS). Clamp to a small positive value.
        float transitionDuration = Mathf.Max(0.01f, smoothFrames / 60f);

        // CrossFade to the target state on the specified layer.
        animator.CrossFade(stateHash, transitionDuration, layer);
    }
}
