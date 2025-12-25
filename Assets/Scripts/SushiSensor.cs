using UnityEngine;
using System.Collections.Generic;

public class SushiSensor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _attractionSpeed = 5f;
    [SerializeField] private LayerMask _sushiLayer;
    
    private List<Sushi> _trackedSushi = new List<Sushi>();

    private void OnTriggerEnter(Collider other)
    {
        // レイヤーで判定
        if (((1 << other.gameObject.layer) & _sushiLayer) != 0)
        {
            Sushi sushi = other.GetComponentInParent<Sushi>();
            if (sushi == null) sushi = other.GetComponentInChildren<Sushi>();

            if (sushi != null && !sushi.IsWasabi && !sushi.IsEaten)
            {
                if (!_trackedSushi.Contains(sushi))
                {
                    _trackedSushi.Add(sushi);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & _sushiLayer) != 0)
        {
            Sushi sushi = other.GetComponentInParent<Sushi>();
            if (sushi == null) sushi = other.GetComponentInChildren<Sushi>();

            if (sushi != null)
            {
                _trackedSushi.Remove(sushi);
            }
        }
    }

    private void Update()
    {
        // 追跡中の寿司をプレイヤーに引き寄せる
        for (int i = _trackedSushi.Count - 1; i >= 0; i--)
        {
            Sushi sushi = _trackedSushi[i];

            // 既に食べられたか、無効になった場合はリストから削除
            if (sushi == null || sushi.IsEaten)
            {
                _trackedSushi.RemoveAt(i);
                continue;
            }

            // プレイヤーに向かって移動（RootObjectを移動させる）
            Transform targetTransform = sushi.RootObject.transform;
            Vector3 direction = (transform.position - targetTransform.position).normalized;
            targetTransform.position += direction * _attractionSpeed * Time.deltaTime;
        }
    }
}
