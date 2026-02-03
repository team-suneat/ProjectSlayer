using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "AccessorySummonConfig", menuName = "TeamSuneat/Config/Summon/Accessory")]
    public class AccessorySummonConfigAsset : XScriptableObject
    {
        [Tooltip("악세사리 소환 등급별 확률")]
        public SummonGradeGrowthData[] GradeGrowths;

        [InfoBox("악세사리 소환 레벨 확률")]
        [SerializeField]
        private Gacha LevelGacha = new();

        [LabelText("최대 소환 레벨")]
        public int MaxSummonLevel = 10;

        private static float Sum(float[] values)
        {
            float s = 0f;
            for (int i = 0; i < values.Length; i++)
            {
                s += values[i];
            }
            return s;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (GradeGrowths == null || GradeGrowths.Length != 6)
            {
                Log.Error("악세사리 소환 데이터는 정확히 6개 등급(일반~신화)이어야 합니다: {0}", name);
                return;
            }
            for (int i = 0; i < GradeGrowths.Length; i++)
            {
                SummonGradeGrowthData data = GradeGrowths[i];
                if (data == null)
                {
                    Log.Error("등급 데이터[{0}]가 null입니다: {1}", i, name);
                    continue;
                }

                if (data.Grade == GradeNames.None)
                {
                    Log.Error("등급[{0}] 이름이 설정되지 않았습니다: {1}", i, name);
                }
            }
            if (ExpectedByLevel != null && ExpectedByLevel.Length > 0 && ExpectedByLevel[0] != null)
            {
                float sumLv1 = Sum(ExpectedByLevel[0]);
                if (!Mathf.Approximately(sumLv1, 1f))
                {
                    Log.Warning("ExpectedByLevel 레벨1 총합이 100%가 아닙니다 ({0:P2}): {1}", sumLv1, name);
                }
            }
            LevelGacha.IsValid();
#endif
        }

        public float[] GetProbabilities(int level)
        {
            if (level < 1 || level > MaxSummonLevel)
            {
                level = 1;
            }
            float[] row = ExpectedByLevel[level - 1];
            float[] probabilities = new float[6];
            for (int i = 0; i < 6 && i < row.Length; i++)
            {
                probabilities[i] = row[i];
            }
            return probabilities;
        }

        public float GetProbability(int level, GradeNames grade)
        {
            int index = grade switch
            {
                GradeNames.Common => 0,
                GradeNames.Grand => 1,
                GradeNames.Rare => 2,
                GradeNames.Epic => 3,
                GradeNames.Legendary => 4,
                GradeNames.Mythic => 5,
                _ => -1
            };
            if (index < 0)
            {
                return 0f;
            }
            if (level < 1 || level > MaxSummonLevel)
            {
                level = 1;
            }
            float[] row = ExpectedByLevel[level - 1];
            if (index >= row.Length)
            {
                return 0f;
            }
            return row[index];
        }

        public int PickLevel()
        {
            int index = LevelGacha.Pick();
            return (int)LevelGacha.ResultValues[index];
        }

        private readonly float[][] ExpectedByLevel =
        {
            new[] { 0.685800f, 0.255000f, 0.054000f, 0.005149f, 0.000050f, 0.000001f }, // L1
            new[] { 0.542f, 0.328f, 0.112f, 0.0171197f, 0.0008f, 0.000003f }, // L2: 54.2, 32.8, 11.2, 1.71197, 0.08, 0.0003%
            new[] { 0.331f, 0.435f, 0.184f, 0.047982f, 0.002f, 0.000018f }, // L3: 33.1, 43.5, 18.4, 4.7982, 0.2, 0.0018%
            new[] { 0.1056f, 0.5584f, 0.235f, 0.0959f, 0.005f, 0.0001f }, // L4: 10.56, 55.84, 23.5, 9.59, 0.5, 0.01%
            new[] { 0.072f, 0.45f, 0.285f, 0.18575f, 0.007f, 0.00025f }, // L5: 7.2, 45, 28.5, 18.575, 0.7, 0.025%
            new[] { 0.0508f, 0.312f, 0.4005f, 0.22618f, 0.0102f, 0.00032f }, // L6: 5.08, 31.2, 40.05, 22.618, 1.02, 0.032%
            new[] { 0.0441f, 0.21f, 0.365f, 0.365f, 0.0154f, 0.0005f }, // L7: 4.41, 21, 36.5, 36.5, 1.54, 0.05%
            new[] { 0.0268f, 0.18f, 0.29f, 0.482f, 0.0205f, 0.0007f }, // L8: 2.68, 18, 29, 48.2, 2.05, 0.07%
            new[] { 0.0011f, 0.042f, 0.18f, 0.7357f, 0.04f, 0.001f }, // L9: 0.11, 4.2, 18, 73.57, 4, 0.1%
            new[] { 0.000100f, 0.005000f, 0.094900f, 0.828500f, 0.070000f, 0.001500f }, // L10
        };

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
            PerformRename("AccessorySummonConfig");
#endif
        }

        //

        [FoldoutGroup("#Custom Button", 1000)]
        [Button("기본값 자동 입력", ButtonSizes.Large)]
        private void FillDefaultValues()
        {
            if (!GradeGrowths.IsValid(5))
            {
                GradeGrowths = new SummonGradeGrowthData[]
                {
                    new() { Grade = GradeNames.Common },
                    new() { Grade = GradeNames.Grand },
                    new() { Grade = GradeNames.Rare },
                    new() { Grade = GradeNames.Epic },
                    new() { Grade = GradeNames.Legendary },
                    new() { Grade = GradeNames.Mythic },
                };
            }
            ApplyExpectedToGradeGrowths();
            FillDefaultLevelGacha();

            EditorUtility.SetDirty(this);
        }

        private void ApplyExpectedToGradeGrowths()
        {
            if (!GradeGrowths.IsValid(5))
            {
                Log.Error("GradeGrowths가 6개 등급이 아닙니다. 기본값 자동 입력을 먼저 실행하세요.");
                return;
            }
            for (int i = 0; i < 6; i++)
            {
                float[] byLevel = new float[ExpectedByLevel.Length];
                for (int l = 0; l < ExpectedByLevel.Length; l++)
                {
                    byLevel[l] = ExpectedByLevel[l][i];
                }
                GradeGrowths[i].SetProbabilityByLevel(byLevel);
            }
        }

        private void FillDefaultLevelGacha()
        {
            float[] probabilities = new float[] { 0.4f, 0.3f, 0.2f, 0.1f };
            float[] resultValues = new float[] { 0, 1, 2, 3 };
            LevelGacha.Set(probabilities, resultValues);
        }

#endif
    }
}