using UnityEngine;

public class SushiSettings : MonoBehaviour
{
    public static SushiSettings Instance { get; private set; }

    [Header("Sushi Settings")]
    [Tooltip("寿司が自動的にデスポーンするまでの時間（秒）")]
    [SerializeField] private float _despawnTime = 10f;

    public float DespawnTime => _despawnTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // シーンをまたぐ場合は DontDestroyOnLoad(gameObject); を追加
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDespawnTime(float despawnTime)
    {
        _despawnTime = despawnTime;
    }
}
