using UnityEngine;

public class AIGoblinAnimator : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private AIGoblinController _controller;
    
    [SerializeField] private int _smoothFrames = 6;
    [SerializeField] private float _walkThreshold = 0.05f;

    private string _currentBodyAnim;
    private string _currentHeadAnim;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if (_controller == null)
        {
            _controller = GetComponent<AIGoblinController>();
        }
    }

    private void Update()
    {
        Animate();
    }

    private void Animate()
    {
        if (_animator == null || _controller == null) return;

        Vector2 moveDir = _controller.CurrentMoveDirection;
        bool isWalking = moveDir.sqrMagnitude > (_walkThreshold * _walkThreshold);

        // For facing, default to movement direction. 
        // If firing recently, we could face the fire target, but for simplicity we rely on movement 
        // or just face right/left based on the object's flip.
        Vector3 faceDir = new Vector3(moveDir.x, moveDir.y, 0f);

        // --- BODY ---
        string nextBodyAnim = _currentBodyAnim;
        if (isWalking)
        {
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
            {
                nextBodyAnim = moveDir.x > 0f ? PlayerAnimationState.BodyRightWalk : PlayerAnimationState.BodyLeftWalk;
            }
            else
            {
                nextBodyAnim = moveDir.y > 0f ? PlayerAnimationState.BodyBackWalk : PlayerAnimationState.BodyWalkIdle;
            }
        }
        else
        {
            // Just idle in whatever the last direction was, defaulting to Front
            if (string.IsNullOrEmpty(_currentBodyAnim) || _currentBodyAnim.Contains("Walking"))
            {
                 nextBodyAnim = PlayerAnimationState.BodyFrontIdle;
                 if (_currentBodyAnim == PlayerAnimationState.BodyRightWalk) nextBodyAnim = PlayerAnimationState.BodyRightIdle;
                 if (_currentBodyAnim == PlayerAnimationState.BodyLeftWalk) nextBodyAnim = PlayerAnimationState.BodyLeftIdle;
                 if (_currentBodyAnim == PlayerAnimationState.BodyBackWalk) nextBodyAnim = PlayerAnimationState.BodyBackIdle;
            }
        }

        if (nextBodyAnim != _currentBodyAnim)
        {
            // Using assumed extension method PlayAnimation if it exists, otherwise standard CrossFade
            _animator.CrossFade(nextBodyAnim, _smoothFrames / 60f, 0);
            _currentBodyAnim = nextBodyAnim;
        }

        // --- HEAD ---
        string nextHeadAnim = _currentHeadAnim;
        if (isWalking)
        {
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
            {
                nextHeadAnim = moveDir.x > 0f ? PlayerAnimationState.HeadRightWalk : PlayerAnimationState.HeadLeftWalk;
            }
            else
            {
                nextHeadAnim = moveDir.y > 0f ? PlayerAnimationState.HeadBackWalk : PlayerAnimationState.HeadWalkIdle;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(_currentHeadAnim) || _currentHeadAnim.Contains("Walking"))
            {
                 nextHeadAnim = PlayerAnimationState.HeadFrontIdle;
                 if (_currentHeadAnim == PlayerAnimationState.HeadRightWalk) nextHeadAnim = PlayerAnimationState.HeadRightIdle;
                 if (_currentHeadAnim == PlayerAnimationState.HeadLeftWalk) nextHeadAnim = PlayerAnimationState.HeadLeftIdle;
                 if (_currentHeadAnim == PlayerAnimationState.HeadBackWalk) nextHeadAnim = PlayerAnimationState.HeadBackIdle;
            }
        }

        if (nextHeadAnim != _currentHeadAnim)
        {
             _animator.CrossFade(nextHeadAnim, _smoothFrames / 60f, 1);
            _currentHeadAnim = nextHeadAnim;
        }
    }
}
