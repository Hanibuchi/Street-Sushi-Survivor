using UnityEngine;

public class CarSettings : MonoBehaviour
{
    public static CarSettings Instance { get; private set; }

    [Header("Spawn Probabilities")]
    [Range(0f, 1f)]
    [SerializeField] private float _rareCarProbability = 0.1f;

    [Header("Spawn Intervals")]
    [Tooltip("車同士がぶつからないための最低限の固定間隔（秒）")]
    [SerializeField] private float _fixedInterval = 2f;
    [Tooltip("ポアソン分布に使用する平均変動間隔（秒）")]
    [SerializeField] private float _averageRandomInterval = 3f;

    [Header("Explosion Settings")]
    [Tooltip("プレイヤーからこの距離以上離れている場合は爆発演出をスキップして即座に破棄する")]
    [SerializeField] private float _explosionDistanceThreshold = 50f;

    public float RareCarProbability => _rareCarProbability;
    public float FixedInterval => _fixedInterval;
    public float AverageRandomInterval => _averageRandomInterval;
    public float ExplosionDistanceThreshold => _explosionDistanceThreshold;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
