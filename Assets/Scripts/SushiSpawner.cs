using UnityEngine;
using System.Collections;

public class SushiSpawner : MonoBehaviour
{
    public static SushiSpawner Instance { get; private set; }

    [Header("Spawn Area")]
    [SerializeField] private Transform _corner1;
    [SerializeField] private Transform _corner2;

    [Header("Spawn Settings")]
    [SerializeField] private float _meanSpawnInterval = 2.0f;
    [SerializeField] private float _rareSushiProbability = 0.1f;
    [SerializeField] private float _wasabiProbability = 0.05f;
    [SerializeField] private float _raycastHeight = 20f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] _normalSushiPrefabs;
    [SerializeField] private GameObject[] _rareSushiPrefabs;
    [SerializeField] private GameObject[] _wasabiPrefabs;

    private float _nextSpawnTime;
    private bool _isSpawning = true;

    private float _baseMeanSpawnInterval;
    private float _baseRareSushiProbability;
    private float _baseWasabiProbability;

    // スポーン範囲の決定
    float minX;
    float maxX;
    float minZ;
    float maxZ;

    /// <summary>
    /// 高級寿司がスポーンする確率。
    /// </summary>
    public float RareSushiProbability
    {
        get => _rareSushiProbability;
        set => _rareSushiProbability = Mathf.Clamp01(value);
    }

    /// <summary>
    /// ワサビがスポーンする確率。
    /// </summary>
    public float WasabiProbability
    {
        get => _wasabiProbability;
        set => _wasabiProbability = Mathf.Clamp01(value);
    }

    /// <summary>
    /// 平均スポーン間隔。
    /// </summary>
    public float MeanSpawnInterval
    {
        get => _meanSpawnInterval;
        set => _meanSpawnInterval = Mathf.Max(0.0001f, value);
    }

    /// <summary>
    /// スポーンが有効かどうか。
    /// </summary>
    public bool IsSpawning
    {
        get => _isSpawning;
        set
        {
            _isSpawning = value;
            if (_isSpawning)
            {
                // 再開時に次のスポーン時間を再計算（即座にスポーンするのを防ぐため）
                CalculateNextSpawnTime();
            }
        }
    }

    /// <summary>
    /// スポーンを開始します。
    /// </summary>
    public void StartSpawning() => IsSpawning = true;

    /// <summary>
    /// スポーンを停止します。
    /// </summary>
    public void StopSpawning() => IsSpawning = false;

    public void SetSpawnRateMultiplier(float multiplier)
    {
        // 間隔を短くすることで湧き率を上げる
        MeanSpawnInterval = _baseMeanSpawnInterval / multiplier;
    }

    public void SetRareSushiProbabilityMultiplier(float multiplier)
    {
        RareSushiProbability = _baseRareSushiProbability * multiplier;
    }

    public void SetWasabiProbabilityMultiplier(float multiplier)
    {
        WasabiProbability = _baseWasabiProbability * multiplier;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _baseMeanSpawnInterval = _meanSpawnInterval;
            _baseRareSushiProbability = _rareSushiProbability;
            _baseWasabiProbability = _wasabiProbability;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CalculateNextSpawnTime();

        // スポーン範囲の決定
        minX = Mathf.Min(_corner1.position.x, _corner2.position.x);
        maxX = Mathf.Max(_corner1.position.x, _corner2.position.x);
        minZ = Mathf.Min(_corner1.position.z, _corner2.position.z);
        maxZ = Mathf.Max(_corner1.position.z, _corner2.position.z);
    }

    private void Update()
    {
        if (!_isSpawning) return;

        if (Time.time >= _nextSpawnTime)
        {
            SpawnSushi();
            CalculateNextSpawnTime();
        }
    }

    private void CalculateNextSpawnTime()
    {
        // ポアソン分布（イベントの間隔は指数分布に従う）
        // t = -ln(1 - u) * mean (uは0から1の乱数)
        float u = Random.value;
        // Random.valueが1.0だとLog(0)で無限になるため回避
        if (u >= 1.0f) u = 0.999999f;

        float interval = -Mathf.Log(1.0f - u) * _meanSpawnInterval;
        _nextSpawnTime = Time.time + interval;
    }

    private void SpawnSushi()
    {
        if (_corner1 == null || _corner2 == null) return;

        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        Vector3 rayOrigin = new Vector3(randomX, _raycastHeight, randomZ);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
        {
            GameObject prefabToSpawn = SelectSushiPrefab();
            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, hit.point, Quaternion.identity);
            }
        }
    }

    private GameObject SelectSushiPrefab()
    {
        float totalProb = _rareSushiProbability + _wasabiProbability;
        float rand = Random.value;

        if (totalProb > 1.0f)
        {
            // 合計が1を超える場合は比率で決定
            float ratio = _rareSushiProbability / totalProb;
            if (rand < ratio)
            {
                return GetRandomPrefab(_rareSushiPrefabs);
            }
            else
            {
                return GetRandomPrefab(_wasabiPrefabs);
            }
        }
        else
        {
            // 通常の確率計算
            if (rand < _rareSushiProbability)
            {
                return GetRandomPrefab(_rareSushiPrefabs);
            }
            else if (rand > 1.0f - _wasabiProbability)
            {
                return GetRandomPrefab(_wasabiPrefabs);
            }
            else
            {
                return GetRandomPrefab(_normalSushiPrefabs);
            }
        }
    }

    private GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        if (prefabs != null && prefabs.Length > 0)
        {
            return prefabs[Random.Range(0, prefabs.Length)];
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        if (_corner1 != null && _corner2 != null)
        {
            Gizmos.color = Color.yellow;
            float minX = Mathf.Min(_corner1.position.x, _corner2.position.x);
            float maxX = Mathf.Max(_corner1.position.x, _corner2.position.x);
            float minZ = Mathf.Min(_corner1.position.z, _corner2.position.z);
            float maxZ = Mathf.Max(_corner1.position.z, _corner2.position.z);

            Vector3 center = new Vector3((minX + maxX) / 2, _corner1.position.y, (minZ + maxZ) / 2);
            Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
