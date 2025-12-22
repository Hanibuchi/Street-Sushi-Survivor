using UnityEngine;

public class CarSensor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Car _car;

    [Header("Settings")]
    [Tooltip("一度鳴らしてから次に鳴らせるまでの間隔")]
    [SerializeField] private float _hornCooldown = 1f;
    
    private float _lastHornTime = -100f;

    private void OnTriggerEnter(Collider other)
    {
        if (_car == null) return;

        // 熊（Player）や障害物に反応するようにタグなどでフィルタリングすることも可能です
        // ここではシンプルに何かが入ったら反応するようにします
        
        if (Time.time >= _lastHornTime + _hornCooldown)
        {
            _car.TriggerHornAnimation();
            _lastHornTime = Time.time;
        }
    }
}
