using UnityEngine;
using Unity.Cinemachine;

public class ResultCameraScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private CinemachineFollow _follow;
    [SerializeField] private CinemachineRotationComposer _rotationComposer;

    [Header("Settings")]
    [SerializeField] private float _followScaleMultiplier = 0.3f;
    [SerializeField] private float _rotationOffsetScaleMultiplier = 0.3f;
    [SerializeField] private bool _debugContinuousUpdate = false;
    
    private Vector3 _baseFollowOffset;
    private Vector3 _baseRotationOffset;

    private void Awake()
    {
        if (_follow == null) _follow = GetComponent<CinemachineFollow>();
        if (_rotationComposer == null) _rotationComposer = GetComponent<CinemachineRotationComposer>();

        if (_follow != null)
        {
            _baseFollowOffset = _follow.FollowOffset;
        }

        if (_rotationComposer != null)
        {
            _baseRotationOffset = _rotationComposer.TargetOffset;
        }
    }

    private void Start()
    {
        UpdateCameraScale();
    }

    private void Update()
    {
        if (_debugContinuousUpdate)
        {
            UpdateCameraScale();
        }
    }

    /// <summary>
    /// ターゲットのTransformのスケールに基づいてカメラのオフセットを更新します。
    /// </summary>
    public void UpdateCameraScale()
    {
        if (_targetTransform == null) return;

        // ターゲットのXスケールを基準にする
        float currentScale = _targetTransform.localScale.x;
        float growth = currentScale - 1f;
        
        // ベースのオフセットを基準に、スケールの成長分に合わせて距離を離す
        if (_follow != null)
        {
            _follow.FollowOffset = _baseFollowOffset + (_baseFollowOffset * growth * _followScaleMultiplier);
        }

        // RotationComposerのTrackedObjectOffsetもスケールさせる
        if (_rotationComposer != null)
        {
            _rotationComposer.TargetOffset = _baseRotationOffset + (_baseRotationOffset * growth * _rotationOffsetScaleMultiplier);
        }
    }
}
