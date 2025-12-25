using UnityEngine;
using System.Collections.Generic;

public enum BonusType
{
    MoveSpeed,
    DashCooldown,
    ShockwaveSize,
    TimeExtension,
    SushiPoints
}

[System.Serializable]
public class BonusOption
{
    public BonusType type;
    public string description;
    public float value;
}

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance { get; private set; }

    [SerializeField] private List<BonusData> _allBonuses;

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

    public List<BonusData> GetRandomBonuses(int count)
    {
        List<BonusData> result = new List<BonusData>();
        List<BonusData> pool = new List<BonusData>(_allBonuses);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    public void ApplyBonus(BonusData bonus)
    {
        if (bonus == null) return;

        switch (bonus.type)
        {
            case BonusType.MoveSpeed:
                PlayerController.Instance.SetSpeed(PlayerController.Instance.Speed);
                break;
            case BonusType.DashCooldown:
                PlayerController.Instance.SetDashCooldown(Mathf.Max(1f, PlayerController.Instance.DashCooldown));
                break;
            case BonusType.ShockwaveSize:
                PlayerController.Instance.SetShockwaveSizeMultiplier(PlayerController.Instance.ShockwaveSizeMultiplier);
                break;
            case BonusType.TimeExtension:
                GameSessionManager.Instance.AddTime(0);
                break;
        }
        Debug.Log($"Applied Bonus: {bonus.bonusName}");
    }
}
