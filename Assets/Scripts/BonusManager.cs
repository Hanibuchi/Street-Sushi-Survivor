using UnityEngine;
using System.Collections.Generic;

public enum BonusType
{
    MoveSpeed,
    DashCooldown,
    ShockwaveSize,
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
        string unit = "%";
        float baseValue = 100f;

        float currentValue = baseValue;

        if (currentLevel > 0)
        {
            int index = Mathf.Min(currentLevel - 1, data.upgradeValues.Count - 1);
            currentValue = data.upgradeValues[index];
        }

        // 最大レベルに達している場合
        if (currentLevel >= data.upgradeValues.Count)
        {
            return $"{currentValue}{unit} (MAX)";
        }

        float nextValue = data.upgradeValues[currentLevel];
        return $"{currentValue}{unit} -> {nextValue}{unit}";
    }

    public void ApplyBonus(BonusData bonus)
    {
        if (bonus == null || bonus.upgradeValues == null || bonus.upgradeValues.Count == 0) return;

        int currentLevel = GetBonusLevel(bonus.type);
        int nextIndex = Mathf.Min(currentLevel, bonus.upgradeValues.Count - 1);
        float valueMultiplier = bonus.upgradeValues[nextIndex] / 100f;

        switch (bonus.type)
        {
            case BonusType.MoveSpeed:
                PlayerController.Instance.SetSpeed(valueMultiplier);
                break;
            case BonusType.DashCooldown:
                PlayerController.Instance.SetDashCooldown(valueMultiplier);
                break;
            case BonusType.ShockwaveSize:
                PlayerController.Instance.SetShockwaveSizeMultiplier(valueMultiplier);
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
