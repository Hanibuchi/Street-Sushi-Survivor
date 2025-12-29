using System.Collections;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Car Prefabs")]
    [SerializeField] private GameObject[] _normalCars;
    [SerializeField] private GameObject[] _rareCars;

    [Header("Spawn Point")]
    [SerializeField] private Transform _spawnPoint;

    private void Start()
    {
        if (_spawnPoint == null) _spawnPoint = transform;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // セッションが開始されていない場合は待機
            if (GameSessionManager.Instance == null || !GameSessionManager.Instance.IsSessionActive)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            if (CarSettings.Instance == null)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            // 1. 固定間隔待機
            yield return new WaitForSeconds(CarSettings.Instance.FixedInterval);

            // 2. ポアソン分布に基づいたランダム間隔待機
            // ポアソン過程におけるイベント間隔は指数分布に従う
            // 指数分布の乱数生成: -log(1 - u) * average (uは0~1の乱数)
            float randomInterval = -Mathf.Log(1f - Random.value) * CarSettings.Instance.AverageRandomInterval;
            yield return new WaitForSeconds(randomInterval);

            // 3. 車のスポーン
            SpawnCar();
        }
    }

    private void SpawnCar()
    {
        if (CarSettings.Instance == null) return;

        GameObject prefabToSpawn = null;
        float roll = Random.value;

        if (roll < CarSettings.Instance.RareCarProbability && _rareCars.Length > 0)
        {
            // レア車をスポーン
            prefabToSpawn = _rareCars[Random.Range(0, _rareCars.Length)];
        }
        else if (_normalCars.Length > 0)
        {
            // 普通の車をスポーン
            prefabToSpawn = _normalCars[Random.Range(0, _normalCars.Length)];
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, _spawnPoint.position, _spawnPoint.rotation);
        }
    }
}
