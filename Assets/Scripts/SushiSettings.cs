using UnityEngine;

public class SushiSettings : MonoBehaviour
{
    public static SushiSettings Instance { get; private set; }

    [Header("Sushi Settings")]
    [Tooltip("寿司が自動的にデスポーンするまでの時間（秒）")]
    [SerializeField] private float _despawnTime = 10f;

    private float _baseDespawnTime;

    public float DespawnTime => _despawnTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _baseDespawnTime = _despawnTime;
            // シーンをまたぐ場合は DontDestroyOnLoad(gameObject); を追加
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDespawnTimeMultiplier(float multiplier)
    {
        _despawnTime = _baseDespawnTime * multiplier;
    }
}
