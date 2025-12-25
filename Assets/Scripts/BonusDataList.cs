using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBonusDataList", menuName = "SushiSurvivor/BonusDataList")]
public class BonusDataList : ScriptableObject
{
    public List<BonusData> bonuses = new List<BonusData>();
}
