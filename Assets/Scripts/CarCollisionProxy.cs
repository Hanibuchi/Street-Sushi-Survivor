using UnityEngine;

/// <summary>
/// 衝突イベントを別のCarクラスへ転送するためのプロキシクラス
/// </summary>
public class CarCollisionProxy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Car _car;

    private void OnCollisionEnter(Collision collision)
    {
        if (_car != null)
        {
            // インスペクタから渡されたCarのOnCollisionEnterを呼び出す
            _car.OnCollisionEnter(collision);
        }
    }
}
