using Sirenix.OdinInspector;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 소환 레벨별 필요 소환량 및 보상 데이터 (한 행)
    /// </summary>
    [System.Serializable]
    public class SummonLevelRewardData
    {
        [LabelText("소환 레벨")]
        [SerializeField]
        private float _level = 1f;

        [LabelText("필요 소환량")]
        [SerializeField]
        private int _requiredSummonCount;

        [LabelText("보상 등급 (None이면 보상 없음)")]
        [SerializeField]
        private GradeNames _rewardGrade = GradeNames.None;

        [LabelText("보상 품질 (1~4등급, 1=최고)")]
        [Range(1, 4)]
        [SerializeField]
        private int _rewardQuality = 1;

        [LabelText("보상 수량")]
        [Min(0)]
        [SerializeField]
        private int _rewardCount;

        public float Level => _level;
        public int RequiredSummonCount => _requiredSummonCount;
        public GradeNames RewardGrade => _rewardGrade;
        public int RewardQuality => _rewardQuality;
        public int RewardCount => _rewardCount;

        public bool HasReward => _rewardGrade != GradeNames.None && _rewardCount > 0;

        public SummonLevelRewardData(float level, int requiredSummonCount, GradeNames rewardGrade = GradeNames.None, int rewardQuality = 1, int rewardCount = 0)
        {
            _level = level;
            _requiredSummonCount = requiredSummonCount;
            _rewardGrade = rewardGrade;
            _rewardQuality = Mathf.Clamp(rewardQuality, 1, 4);
            _rewardCount = Mathf.Max(0, rewardCount);
        }
    }
}
