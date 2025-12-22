using UnityEngine;

public class Sushi : MonoBehaviour
{
    [Header("Sushi Properties")]
    [SerializeField] private int _points = 1;
    [SerializeField] private bool _isWasabi = false;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _rootObject;

    private bool _isProcessed = false;
    private float _timer = 0f;

    /// <summary>
    /// この寿司を食べた時に得られるポイント
    /// </summary>
    public int Points => _points;

    /// <summary>
    /// これがワサビ（障害物）かどうか
    /// </summary>
    public bool IsWasabi => _isWasabi;

    private void Update()
    {
        if (_isProcessed) return;

        // SushiSettingsが存在する場合のみタイマーを更新
        if (SushiSettings.Instance != null)
        {
            _timer += Time.deltaTime;
            if (_timer >= SushiSettings.Instance.DespawnTime)
            {
                Despawn();
            }
        }
    }

    /// <summary>
    /// 寿司を食べる処理を開始します。
    /// </summary>
    public void Eat()
    {
        if (_isProcessed) return;
        _isProcessed = true;

        if (_animator != null)
        {
            _animator.SetTrigger("Eat");
        }
        else
        {
            OnAnimationComplete();
        }
    }

    /// <summary>
    /// 寿司が時間経過で消える処理を開始します。
    /// </summary>
    private void Despawn()
    {
        if (_isProcessed) return;
        _isProcessed = true;

        if (_animator != null)
        {
            _animator.SetTrigger("Despawn");
        }
        else
        {
            OnAnimationComplete();
        }
    }

    /// <summary>
    /// アニメーションイベントから呼び出される削除メソッド。
    /// EatとDespawnの両方で共通して使用できます。
    /// </summary>
    public void OnAnimationComplete()
    {
        if (_rootObject != null)
        {
            Destroy(_rootObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
