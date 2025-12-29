using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBonusData", menuName = "SushiSurvivor/BonusData")]
public class BonusData : ScriptableObject
{
    public string bonusName;
    [TextArea] public string description;
    public BonusType type;
    public List<float> upgradeValues = new List<float> { 100f };
}