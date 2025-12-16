using System;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 강화 시스템 개별 능력치 데이터
    /// </summary>
    [Serializable]
    public class EnhancementData
    {
        [Tooltip("능력치 종류")]
        public StatNames StatName;

        [Tooltip("최대 레벨")]
        public int MaxLevel;

        [Tooltip("능력치 초기값")]
        public float InitialValue;

        [Tooltip("능력치 성장값 (레벨당 증가량)")]
        public float GrowthValue;

        [Tooltip("초기 비용")]
        public int InitialCost;

        [Tooltip("비용 성장률 (레벨당 비용 증가 배율)")]
        public float CostGrowthRate;

        [Tooltip("요구 능력치 (None이면 요구사항 없음)")]
        public StatNames RequiredStatName = StatNames.None;

        [Tooltip("요구 능력치 레벨 (0이면 요구사항 없음)")]
        public int RequiredStatLevel = 0;

        public int TID => BitConvert.Enum32ToInt(StatName);

        /// <summary>
        /// 요구 능력치가 설정되어 있는지 확인합니다.
        /// </summary>
        public bool HasRequirement => RequiredStatName != StatNames.None && RequiredStatLevel > 0;
    }
}

