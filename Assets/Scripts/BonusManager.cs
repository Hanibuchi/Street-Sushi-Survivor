using UnityEngine;
using System.Collections.Generic;

public enum BonusType
{
    MoveSpeed,
    DashCooldown,
    ShockwaveSize,
    SushiSensorRange,
    SushiSpawnRate,
    SushiDuration,
    RareSushiSpawnRate,
    WasabiSpawnRate,
    CarSpawnRate,
    RareCarSpawnRate,
    CarExplosionRange,
    DashSpeed,
    DashDuration
}

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance { get; private set; }

    [SerializeField] private BonusDataList _bonusDataList;

    private Dictionary<BonusType, int> _bonusPickCounts = new Dictionary<BonusType, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetBonusLevel(BonusType type)
    {
        if (_bonusPickCounts.TryGetValue(type, out int count))
        {
            return count;
        }
        return 0;
    }

    public List<BonusData> GetRandomBonuses(int count)
    {
        List<BonusData> result = new List<BonusData>();
        if (_bonusDataList == null || _bonusDataList.bonuses == null) return result;

        List<BonusData> pool = new List<BonusData>(_bonusDataList.bonuses);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    public string GetDynamicDescription(BonusData data)
    {
        if (data == null || data.upgradeValues == null || data.upgradeValues.Count == 0) return "";

        int currentLevel = GetBonusLevel(data.type);

        // 単位とベース表示値の設定
        string unit = "%";
        float baseValue = data.upgradeValues[0];

        float currentValue = baseValue;

        if (currentLevel > 0)
        {
            int index = Mathf.Min(currentLevel, data.upgradeValues.Count - 1);
            currentValue = data.upgradeValues[index];
        }

        // 最大レベルに達している場合
        if (currentLevel >= data.upgradeValues.Count - 1)
        {
            return $"{currentValue}{unit} (MAX)";
        }

        float nextValue = data.upgradeValues[currentLevel + 1];
        return $"{currentValue}{unit} -> {nextValue}{unit}";
    }

    public void ApplyBonus(BonusData bonus)
    {
        if (bonus == null || bonus.upgradeValues == null || bonus.upgradeValues.Count == 0) return;

        int currentLevel = GetBonusLevel(bonus.type);
        int nextIndex = Mathf.Min(currentLevel + 1, bonus.upgradeValues.Count - 1);
        float rawValue = bonus.upgradeValues[nextIndex];
        float multiplier = rawValue / bonus.upgradeValues[0];

        switch (bonus.type)
        {
            case BonusType.MoveSpeed:
                PlayerController.Instance.SetSpeed(multiplier);
                break;
            case BonusType.DashCooldown:
                PlayerController.Instance.SetDashCooldown(multiplier);
                break;
            case BonusType.ShockwaveSize:
                PlayerController.Instance.SetShockwaveSizeMultiplier(multiplier);
                break;
            case BonusType.SushiSensorRange:
                PlayerController.Instance.SetSushiSensorScaleMultiplier(multiplier);
                break;
            case BonusType.SushiSpawnRate:
                SushiSpawner.Instance.SetSpawnRateMultiplier(multiplier);
                break;
            case BonusType.SushiDuration:
                SushiSettings.Instance.SetDespawnTimeMultiplier(multiplier);
                break;
            case BonusType.RareSushiSpawnRate:
                SushiSpawner.Instance.SetRareSushiProbabilityMultiplier(multiplier);
                break;
            case BonusType.WasabiSpawnRate:
                SushiSpawner.Instance.SetWasabiProbabilityMultiplier(multiplier);
                break;
            case BonusType.CarSpawnRate:
                CarSettings.Instance.SetSpawnRateMultiplier(multiplier);
                break;
            case BonusType.RareCarSpawnRate:
                CarSettings.Instance.SetRareCarProbabilityMultiplier(multiplier);
                break;
            case BonusType.CarExplosionRange:
                CarSettings.Instance.SetExplosionScaleMultiplier(multiplier);
                break;
            case BonusType.DashSpeed:
                PlayerController.Instance.SetDashSpeedMultiplier(multiplier);
                break;
            case BonusType.DashDuration:
                PlayerController.Instance.SetDashDurationMultiplier(multiplier);
                break;
        }

        // カウントを増やす
        if (_bonusPickCounts.ContainsKey(bonus.type))
            _bonusPickCounts[bonus.type]++;
        else
            _bonusPickCounts[bonus.type] = 1;

        Debug.Log($"Applied Bonus: {bonus.bonusName} (Level {GetBonusLevel(bonus.type)})");
    }
}
