using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "SkillCardSummonConfig", menuName = "TeamSuneat/Config/Summon/SkillCard")]
    public class SkillCardSummonConfigAsset : XScriptableObject
    {
        [InfoBox("스킬 카드 소환 등급별 확률")]
        [SerializeField]
        private Gacha GradeGacha = new();

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            GradeGacha.IsValid();
#endif
        }

        public GradeNames PickGrade()
        {
            int index = GradeGacha.Pick();
            return (GradeNames)GradeGacha.ResultValues[index];
        }

        public float[] GetGradeProbabilities()
        {
            if (GradeGacha?.Probabilities == null || GradeGacha.Probabilities.Count == 0)
            {
                return new float[6];
            }
            float[] result = new float[6];
            int count = Mathf.Min(6, GradeGacha.Probabilities.Count);
            for (int i = 0; i < count; i++)
            {
                result[i] = GradeGacha.Probabilities[i];
            }
            return result;
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();
        }

        public override void Refresh()
        {
            base.Refresh();
        }

        public override void Rename()
        {
#if UNITY_EDITOR
            PerformRename("SkillCardSummonConfig");
#endif
        }

        [FoldoutGroup("#Custom Button", 1000)]
        [Button("기본값 자동 입력", ButtonSizes.Large)]
        private void FillDefaultValues()
        {
            float[] probabilities = new float[] { 0.40f, 0.30f, 0.20f, 0.08f, 0.01f, 0.01f };
            float[] resultValues = new float[] { 0f, 1f, 2f, 3f, 4f, 5f };
            GradeGacha.Set(probabilities, resultValues);
            EditorUtility.SetDirty(this);
        }

#endif
    }
}