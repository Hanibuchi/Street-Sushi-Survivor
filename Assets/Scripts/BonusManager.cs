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

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance { get; private set; }

    [SerializeField] private BonusDataList _bonusDataList;

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

    public string GetDynamicDescription(BonusType type)
    {
        switch (type)
        {
            case BonusType.MoveSpeed:
                return "Increases your movement speed.";
            case BonusType.DashCooldown:
                return "Reduces the cooldown time of your dash.";
            case BonusType.ShockwaveSize:
                return "Increases the size of your shockwave attack.";
            case BonusType.TimeExtension:
                return "Extends the time limit for the current round.";
            case BonusType.SushiPoints:
                return "Increases the points gained from eating sushi.";
            default:
                return "";
        }
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
