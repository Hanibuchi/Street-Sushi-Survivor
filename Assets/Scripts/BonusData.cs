using UnityEngine;

[CreateAssetMenu(fileName = "NewBonusData", menuName = "SushiSurvivor/BonusData")]
public class BonusData : ScriptableObject
{
    public string bonusName;
    [TextArea] public string description;
    public BonusType type;
}