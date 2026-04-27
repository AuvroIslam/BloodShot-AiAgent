using UnityEngine;
using Singletons;

public static class PlayerAnimationState
{
    public const string BodyFrontIdle = "Body_Front_Idle";
    public const string BodyWalkIdle = "Body_Front_Walking";
    public const string BodyBackIdle = "Body_Back_Idle";
    public const string BodyBackWalk = "Body_Back_Walking";
    public const string BodyLeftIdle = "Body_Left_Idle";
    public const string BodyLeftWalk = "Body_Left_Walking";
    public const string BodyRightIdle = "Body_Right_Idle";
    public const string BodyRightWalk = "Body_Right_Walking";


    public const string HeadFrontIdle = "Head_Front_Idle";
    public const string HeadWalkIdle = "Head_Front_Walking";
    public const string HeadBackIdle = "Head_Back_Idle";
    public const string HeadBackWalk = "Head_Back_Walking";
    public const string HeadLeftIdle = "Head_Left_Idle";
    public const string HeadLeftWalk = "Head_Left_Walking";
    public const string HeadRightIdle = "Head_Right_Idle";
    public const string HeadRightWalk = "Head_Right_Walking";

}
public class PlayerAnimator : MonoBehaviour
{
    private Animator _playerAnimator;

    [SerializeField] private int _smoothFrames = 6;
    [SerializeField] private float _walkThreshold = 0.05f; // input magnitude to consider walking

    private string _currentBodyAnim;
    private string _currentHeadAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }

    private void Animate()
    {
        if (_playerAnimator == null) return;
        if (InputHandler.Instance == null) return;

        // Movement input determines walking state
        Vector2 moveDir = InputHandler.Instance.MoveDirection;
        bool isWalking = moveDir.sqrMagnitude > (_walkThreshold * _walkThreshold);

        // Mouse position (world) used for facing
        Vector3 faceDir = Vector3.up;
        var cam = Camera.main;
        if (cam != null)
        {
            Vector3 mouseScreen = InputHandler.Instance.MousePosition;
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = transform.position.z;
            faceDir = (mouseWorld - transform.position);
        }
        else
        {
            // fallback to movement direction if camera not available
            faceDir = new Vector3(moveDir.x, moveDir.y, 0f);
        }

        // --- BODY: movement first; if not moving, use mouse direction for idle facing ---
        string nextBodyAnim;
        if (isWalking)
        {
            // Choose body walking based on movement vector (prefer movement for body)
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
            // No movement -> body uses mouse/world facing for idle orientation
            if (Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y))
            {
                nextBodyAnim = faceDir.x > 0f ? PlayerAnimationState.BodyRightIdle : PlayerAnimationState.BodyLeftIdle;
            }
            else
            {
                nextBodyAnim = faceDir.y > 0f ? PlayerAnimationState.BodyBackIdle : PlayerAnimationState.BodyFrontIdle;
            }
        }

        // Apply body animation on base layer (layer 0)
        if (nextBodyAnim != _currentBodyAnim)
        {
            _playerAnimator.PlayAnimation(nextBodyAnim, _smoothFrames, layer: 0);
            _currentBodyAnim = nextBodyAnim;
        }

        // --- HEAD: influenced by mouse position primarily, but reflect walking vs idle based on movement ---
        string nextHeadAnim;
        if (Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y))
        {
            // horizontal facing by mouse
            if (faceDir.x > 0f)
                nextHeadAnim = isWalking ? PlayerAnimationState.HeadRightWalk : PlayerAnimationState.HeadRightIdle;
            else
                nextHeadAnim = isWalking ? PlayerAnimationState.HeadLeftWalk : PlayerAnimationState.HeadLeftIdle;
        }
        else
        {
            // vertical facing by mouse
            if (faceDir.y > 0f)
                nextHeadAnim = isWalking ? PlayerAnimationState.HeadBackWalk : PlayerAnimationState.HeadBackIdle;
            else
                nextHeadAnim = isWalking ? PlayerAnimationState.HeadWalkIdle : PlayerAnimationState.HeadFrontIdle;
        }

        // Apply head animation on layer 1
        if (nextHeadAnim != _currentHeadAnim)
        {
            _playerAnimator.PlayAnimation(nextHeadAnim, _smoothFrames, layer: 1);
            _currentHeadAnim = nextHeadAnim;
        }
    }
}
