using UnityEngine;
using Unity.Cinemachine; // Unity 6 uses Cinemachine 3 by default

public class AICameraController : MonoBehaviour
{
    [Tooltip("The Cinemachine Camera to control. Will auto-find if empty.")]
    [SerializeField] private CinemachineCamera _cineCamera;
    
    [Tooltip("The agents to cycle through. Will auto-find tag 'Agent' if empty.")]
    [SerializeField] private Transform[] _agents;
    
    private int _currentIndex = -1;

    private void Start()
    {
        ResolveReferences();
        SyncIndexWithCurrentCameraTarget();
    }

    private void ResolveReferences()
    {
        // Try to find the camera automatically if not assigned
        if (_cineCamera == null)
        {
            _cineCamera = Object.FindFirstObjectByType<CinemachineCamera>();
            if (_cineCamera == null)
            {
                Debug.LogWarning("AICameraController: Could not find a CinemachineCamera in the scene.");
            }
        }

        // Try to populate agents automatically if the list is empty
        if (_agents == null || _agents.Length == 0)
        {
            GameObject[] foundAgents = GameObject.FindGameObjectsWithTag("Agent");
            if (foundAgents.Length > 0)
            {
                _agents = new Transform[foundAgents.Length];
                for (int i = 0; i < foundAgents.Length; i++)
                {
                    _agents[i] = foundAgents[i].transform;
                }
            }
            else
            {
                Debug.LogWarning("AICameraController: No objects with tag 'Agent' found.");
            }
        }
    }

    private void SyncIndexWithCurrentCameraTarget()
    {
        if (_cineCamera == null || _agents == null || _agents.Length == 0)
            return;

        Transform currentTarget = _cineCamera.Target.TrackingTarget;
        _currentIndex = -1;

        for (int i = 0; i < _agents.Length; i++)
        {
            if (_agents[i] == currentTarget)
            {
                _currentIndex = i;
                return;
            }
        }
    }

    /// <summary>
    /// Cycles the camera target to the next agent in the list.
    /// Link this method to your UI Button's OnClick event.
    /// </summary>
    public void ToggleCamera()
    {
        ResolveReferences();
        if (_cineCamera == null || _agents == null || _agents.Length == 0)
            return;

        // Keep index aligned even if camera target changed from inspector/another script.
        SyncIndexWithCurrentCameraTarget();

        // Move to the next index, wrapping around to 0 if at the end
        _currentIndex = (_currentIndex + 1) % _agents.Length;
        Transform newTarget = _agents[_currentIndex];

        if (newTarget != null)
        {
            // In Cinemachine 3, the Follow target is set via Target.TrackingTarget
            _cineCamera.Target.TrackingTarget = newTarget;
            
            Debug.Log($"[Camera] Switched focus to: {newTarget.gameObject.name}");
        }
    }
}
