using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private Collider _explosionCollider;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip _explosionSE;

    [Header("Destroy Settings")]
    [SerializeField] private GameObject _rootObject;
    [SerializeField] private float _destroyDelay = 2f;

    private void Start()
    {
        PlayExplosionSE();
        AutoDestroy();
    }

    /// <summary>
    /// 指定されたオブジェクトを一定時間後に削除します。
    /// </summary>
    private void AutoDestroy()
    {
        if (_rootObject != null)
        {
            Destroy(_rootObject, _destroyDelay);
        }
    }

    /// <summary>
    /// 爆発音を再生します。
    /// </summary>
    private void PlayExplosionSE()
    {
        if (SoundManager.Instance != null && _explosionSE != null)
        {
            SoundManager.Instance.PlaySE(_explosionSE);
        }
    }

    /// <summary>
    /// 爆発の当たり判定を無効化します。
    /// プレイヤーが複数回ヒットするのを防ぐために外部から呼び出されます。
    /// </summary>
    public void DeactivateCollider()
    {
        if (_explosionCollider != null)
        {
            _explosionCollider.enabled = false;
        }
    }
}
