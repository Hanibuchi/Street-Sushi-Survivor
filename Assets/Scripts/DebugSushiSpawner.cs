using UnityEngine;
using System.Collections;

public class DebugSushiSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] _sushiPrefabs;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private Vector3 _spawnAreaSize = new Vector3(10f, 0f, 10f);
    [SerializeField] private bool _autoStart = true;

    private Coroutine _spawnCoroutine;

    private void Start()
    {
        if (_autoStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (_spawnCoroutine == null)
        {
            _spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnSushi();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnSushi()
    {
        if (_sushiPrefabs == null || _sushiPrefabs.Length == 0) return;

        // ランダムなプレハブを選択
        GameObject prefab = _sushiPrefabs[Random.Range(0, _sushiPrefabs.Length)];

        // スポーン範囲内のランダムな位置を計算
        Vector3 randomOffset = new Vector3(
            Random.Range(-_spawnAreaSize.x / 2f, _spawnAreaSize.x / 2f),
            Random.Range(-_spawnAreaSize.y / 2f, _spawnAreaSize.y / 2f),
            Random.Range(-_spawnAreaSize.z / 2f, _spawnAreaSize.z / 2f)
        );

        Vector3 spawnPosition = transform.position + randomOffset;

        // スポーン
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        // スポーン範囲をエディタ上で可視化
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _spawnAreaSize);
    }
}
