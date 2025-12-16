using System;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 성장 시스템 개별 능력치 데이터
    /// </summary>
    [Serializable]
    public class GrowthData
    {
        [Tooltip("능력치 종류")]
        public StatNames StatName;

        [Tooltip("최대 레벨")]
        public int MaxLevel;

        [Tooltip("능력치 포인트 초기 비용")]
        public int InitialCost;

        [Tooltip("능력치 포인트 비용 성장률 (레벨당 비용 증가 배율)")]
        public float CostGrowthRate;

        [Tooltip("능력치 레벨당 스탯 증가량")]
        public float StatIncreasePerLevel;

        public int TID => BitConvert.Enum32ToInt(StatName);
    }
}

