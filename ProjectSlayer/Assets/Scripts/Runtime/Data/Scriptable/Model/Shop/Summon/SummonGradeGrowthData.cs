using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// 소환 등급별 레벨별 확률 데이터 (무기/악세사리/스킬 카드 공통)
    /// </summary>
    [System.Serializable]
    public class SummonGradeGrowthData
    {
        [LabelText("등급")]
        public GradeNames Grade = GradeNames.None;

        [LabelText("레벨별 확률 (1~10)")]
        [SerializeField]
        private float[] _probabilityByLevel = new float[10];

        public float GetProbability(int level)
        {
            if (level < 1 || _probabilityByLevel == null)
            {
                return 0f;
            }
            int index = level - 1;
            if (index >= _probabilityByLevel.Length)
            {
                return 0f;
            }
            return _probabilityByLevel[index];
        }

        public void SetProbabilityByLevel(float[] probabilities)
        {
            if (probabilities == null || probabilities.Length == 0)
            {
                return;
            }
            _probabilityByLevel = new float[Mathf.Min(10, probabilities.Length)];
            for (int i = 0; i < _probabilityByLevel.Length; i++)
            {
                _probabilityByLevel[i] = probabilities[i];
            }
        }
    }
}
