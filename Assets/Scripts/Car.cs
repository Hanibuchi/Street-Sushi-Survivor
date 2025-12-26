using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private LayerMask _collisionLayer;

    [Header("Explosion Settings")]
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private Rigidbody _carRigidbody;
    [SerializeField] private float _explosionForce = 20f;
    [SerializeField] private GameObject _rootObject;
    [SerializeField] private float _destroyDelay = 5f;

    [Header("Sushi Settings")]
    [SerializeField] private GameObject _sushiOnRoof;
    [SerializeField] private GameObject _sushiPrefab;

    [Header("Animation & Audio")]
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioClip _hornClip;

    private bool _isExploded = false;
    private float _currentSpeed;
    private float _originalSpeed;

    public float CurrentSpeed => _currentSpeed;

    private void Start()
    {
        _originalSpeed = _moveSpeed;
        _currentSpeed = _moveSpeed;
    }

    public void SetSpeed(float speed)
    {
        // Debug.Log($"Speed set to: {speed}");
        _currentSpeed = speed;
    }

    public void ResetSpeed()
    {
        // Debug.Log($"Speed reset to: {_originalSpeed}");
        _currentSpeed = _originalSpeed;
    }

    private void FixedUpdate()
    {
        if (_isExploded) return;

        // Rigidbodyを使用して前進し続ける
        if (_carRigidbody != null)
        {
            Vector3 nextPosition = _carRigidbody.position + transform.forward * _currentSpeed * Time.fixedDeltaTime;
            _carRigidbody.MovePosition(nextPosition);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (_isExploded) return;
        HandleCollision(collision.gameObject, collision.contacts[0].point);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_isExploded) return;
        HandleCollision(other.gameObject, transform.position);
    }

    void HandleCollision(GameObject obj, Vector3 pos)
    {
        // 指定したLayer以外のオブジェクトと衝突したら爆発
        if (((1 << obj.layer) & _collisionLayer) != 0)
        {
            Explode();
        }
    }

    /// <summary>
    /// クラクションを鳴らすアニメーションを再生します。
    /// </summary>
    public void TriggerHornAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Horn");
        }
    }

    /// <summary>
    /// 熊にぶつかった時に呼び出される爆発演出メソッド
    /// </summary>
    public void Explode()
    {
        if (_isExploded) return;
        _isExploded = true;

        // プレイヤーから遠い場合は演出なしで即座に破棄
        if (PlayerController.Instance != null && CarSettings.Instance != null)
        {
            float distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
            if (distance > CarSettings.Instance.ExplosionDistanceThreshold)
            {
                if (_rootObject != null) Destroy(_rootObject);
                else Destroy(gameObject);
                return;
            }
        }

        // 屋根の上の寿司を非表示にする
        if (_sushiOnRoof != null)
        {
            _sushiOnRoof.SetActive(false);
        }

        // 爆発アニメーションの再生
        if (_animator != null)
        {
            _animator.SetTrigger("Explode");
        }

        // 爆発エフェクトの生成
        if (_explosionPrefab != null)
        {
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            if (CarSettings.Instance != null)
            {
                explosion.transform.localScale *= CarSettings.Instance.ExplosionScaleMultiplier;
            }
        }

        // 寿司の召喚
        if (_sushiPrefab != null)
        {
            GameObject spawnedSushi = Instantiate(_sushiPrefab, transform.position, Quaternion.identity);
            Sushi sushiComponent = spawnedSushi.GetComponentInChildren<Sushi>();
            if (sushiComponent != null)
            {
                sushiComponent.SetIdle();
            }
        }

        // Rigidbodyを有効にして吹き飛ばす
        if (_carRigidbody != null)
        {
            _carRigidbody.isKinematic = false;
            _carRigidbody.useGravity = true;

            // 上方向へのランダムな力を加えて派手に飛ばす
            Vector3 force = Vector3.up * _explosionForce;
            force += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * (_explosionForce * 0.5f);

            _carRigidbody.AddForce(force, ForceMode.Impulse);
            _carRigidbody.AddTorque(Random.insideUnitSphere * _explosionForce, ForceMode.Impulse);
        }

        // 一定時間後に削除用アニメーションをトリガー
        Invoke(nameof(TriggerDestroyAnimation), _destroyDelay);
    }

    /// <summary>
    /// 削除用アニメーションを再生します。
    /// </summary>
    private void TriggerDestroyAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Destroy");
        }
        else
        {
            // アニメーターがない場合は即座に削除
            OnDestroyAnimationComplete();
        }
    }

    /// <summary>
    /// アニメーションイベントから呼び出される最終的な削除メソッド
    /// </summary>
    public void OnDestroyAnimationComplete()
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

    /// <summary>
    /// アニメーションイベントから呼び出されるクラクション再生メソッド
    /// </summary>
    public void PlayHorn()
    {
        // Debug.Log($"SoundManager: {SoundManager.Instance != null}, HornClip: {_hornClip != null}");
        if (SoundManager.Instance != null && _hornClip != null)
        {
            SoundManager.Instance.PlaySE(_hornClip);
        }
    }
}
