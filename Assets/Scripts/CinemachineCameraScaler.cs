using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineFollow _follow;

    [Header("Settings")]
    [SerializeField] private float _followScaleMultiplier = 0.3f;
    
    private Vector3 _baseFollowOffset;

    private void Awake()
    {
        if (_follow == null) _follow = GetComponent<CinemachineFollow>();

        if (_follow != null)
        {
            _baseFollowOffset = _follow.FollowOffset;
        }
    }

    private void Start()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTotalPointsChanged += UpdateCameraScale;
            // 初期化
            UpdateCameraScale(GameSessionManager.Instance.TotalPoints);
        }
    }

    private void OnDestroy()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTotalPointsChanged -= UpdateCameraScale;
        }
    }

    private void UpdateCameraScale(int totalPoints)
    {
        if (PlayerController.Instance == null) return;

        float playerScale = PlayerController.Instance.CurrentScale;
        float growth = playerScale - 1f;
        
        // カメラの距離（Follow Offset）をスケールさせる
        // ベースのオフセットは維持し、成長分に対してのみマルチプライヤーを適用して加算する
        if (_follow != null)
        {
            _follow.FollowOffset = _baseFollowOffset + (_baseFollowOffset * growth * _followScaleMultiplier);
        }
    }
}
