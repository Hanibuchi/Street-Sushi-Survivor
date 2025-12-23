using UnityEngine;

/// <summary>
/// ボーダーにアタッチし、指定したレイヤーを持つオブジェクトが接触した際に
/// そのルートオブジェクトを削除するコンポーネント。
/// </summary>
public class Border : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask targetLayers;

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject obj)
    {
        // レイヤーがマスクに含まれているか確認
        if (((1 << obj.layer) & targetLayers) != 0)
        {
            // rootをDestroyする
            GameObject rootObject = obj.transform.root.gameObject;
            Debug.Log($"Destroying object root: {rootObject.name}");
            Destroy(rootObject);
        }
    }
}
