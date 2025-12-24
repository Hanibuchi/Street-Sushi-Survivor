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

    [SerializeField] private List<BonusOption> _allBonuses;

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

    public List<BonusOption> GetRandomBonuses(int count)
    {
        List<BonusOption> result = new List<BonusOption>();
        List<BonusOption> pool = new List<BonusOption>(_allBonuses);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    public void ApplyBonus(BonusOption bonus)
    {
        switch (bonus.type)
        {
            case BonusType.MoveSpeed:
                PlayerController.Instance.SetSpeed(PlayerController.Instance.Speed + bonus.value);
                break;
            case BonusType.DashCooldown:
                PlayerController.Instance.SetDashCooldown(Mathf.Max(1f, PlayerController.Instance.DashCooldown - bonus.value));
                break;
            case BonusType.ShockwaveSize:
                PlayerController.Instance.SetShockwaveSizeMultiplier(PlayerController.Instance.ShockwaveSizeMultiplier + bonus.value);
                break;
            case BonusType.TimeExtension:
                GameSessionManager.Instance.AddTime(bonus.value);
                break;
        }
        Debug.Log($"Applied Bonus: {bonus.description}");
    }
}
