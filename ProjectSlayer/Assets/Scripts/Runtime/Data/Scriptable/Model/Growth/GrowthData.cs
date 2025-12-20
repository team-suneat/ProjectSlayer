using System;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 성장 시스템 개별 능력치 데이터
    /// </summary>
    [Serializable]
    public class GrowthData
    {
        [Tooltip("성장 타입 종류")]
        public CharacterGrowthTypes GrowthType;

        [Tooltip("능력치 종류")]
        public StatNames StatName;

        [Tooltip("최대 레벨")]
        public int MaxLevel;

        [Tooltip("능력치 레벨당 능력치 증가량")]
        public float StatIncreasePerLevel;

        public int TID => BitConvert.Enum32ToInt(StatName);
    }
}