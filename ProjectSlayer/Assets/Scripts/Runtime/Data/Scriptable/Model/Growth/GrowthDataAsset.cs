using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "GrowthData", menuName = "TeamSuneat/Scriptable/GrowthData")]
    public class GrowthDataAsset : XScriptableObject
    {
        [TableList(ShowIndexLabels = true)]
        [Tooltip("성장 시스템 능력치 데이터 목록")]
        public GrowthData[] DataArray;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (DataArray == null || DataArray.Length == 0)
            {
                Log.Error("성장 시스템 데이터가 설정되지 않았습니다: {0}", name);
                return;
            }

            for (int i = 0; i < DataArray.Length; i++)
            {
                GrowthData data = DataArray[i];
                if (data == null)
                {
                    Log.Error("성장 시스템 데이터[{0}]가 null입니다: {1}", i, name);
                    continue;
                }

                if (data.GrowthType == CharacterGrowthTypes.None)
                {
                    Log.Error("성장 시스템 데이터[{0}]의 성장 타입이 설정되지 않았습니다: {1}", i, name);
                }
                if (data.StatName == StatNames.None)
                {
                    Log.Error("성장 시스템 데이터[{0}]의 스탯 이름이 설정되지 않았습니다: {1}", i, name);
                }
                if (data.MaxLevel == 0)
                {
                    Log.Error("성장 시스템 데이터[{0}]의 최대 레벨이 설정되지 않았습니다: {1}", i, name);
                }
            }

            // 성장 타입 중복 체크
            IEnumerable<CharacterGrowthTypes> typeDuplicates = DataArray
                .Where(d => d != null && d.GrowthType != CharacterGrowthTypes.None)
                .GroupBy(d => d.GrowthType)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (CharacterGrowthTypes duplicate in typeDuplicates)
            {
                Log.Error("성장 시스템 데이터에 중복된 성장 타입이 있습니다: {0} in {1}", duplicate, name);
            }

            // 스탯 중복 체크
            IEnumerable<StatNames> statDuplicates = DataArray
                .Where(d => d != null && d.StatName != StatNames.None)
                .GroupBy(d => d.StatName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (StatNames duplicate in statDuplicates)
            {
                Log.Error("성장 시스템 데이터에 중복된 스탯이 있습니다: {0} in {1}", duplicate, name);
            }
#endif
        }

        // 스탯 이름으로 성장 데이터를 찾습니다.
        public GrowthData FindGrowthData(StatNames statName)
        {
            if (DataArray == null)
            {
                return null;
            }

            for (int i = 0; i < DataArray.Length; i++)
            {
                if (DataArray[i] != null && DataArray[i].StatName == statName)
                {
                    return DataArray[i];
                }
            }

            return null;
        }

        // 성장 타입으로 성장 데이터를 찾습니다.
        public GrowthData FindGrowthDataByType(CharacterGrowthTypes growthType)
        {
            if (DataArray == null)
            {
                return null;
            }

            for (int i = 0; i < DataArray.Length; i++)
            {
                if (DataArray[i] != null && DataArray[i].GrowthType == growthType)
                {
                    return DataArray[i];
                }
            }

            return null;
        }

        // 성장 타입에 해당하는 스탯 이름을 반환합니다.
        public static StatNames GetStatNameByGrowthType(CharacterGrowthTypes growthType)
        {
            return growthType switch
            {
                CharacterGrowthTypes.Strength => StatNames.Strength,
                CharacterGrowthTypes.HealthPoint => StatNames.HealthPoint,
                CharacterGrowthTypes.Vitality => StatNames.Vitality,
                CharacterGrowthTypes.Critical => StatNames.Critical,
                CharacterGrowthTypes.Luck => StatNames.Luck,
                CharacterGrowthTypes.AccuracyStat => StatNames.AccuracyStat,
                CharacterGrowthTypes.Dodge => StatNames.Dodge,
                _ => StatNames.None
            };
        }

        // 스탯 이름에 해당하는 성장 타입을 반환합니다.
        public static CharacterGrowthTypes GetGrowthTypeByStatName(StatNames statName)
        {
            return statName switch
            {
                StatNames.Strength => CharacterGrowthTypes.Strength,
                StatNames.HealthPoint => CharacterGrowthTypes.HealthPoint,
                StatNames.Vitality => CharacterGrowthTypes.Vitality,
                StatNames.Critical => CharacterGrowthTypes.Critical,
                StatNames.Luck => CharacterGrowthTypes.Luck,
                StatNames.AccuracyStat => CharacterGrowthTypes.AccuracyStat,
                StatNames.Dodge => CharacterGrowthTypes.Dodge,
                _ => CharacterGrowthTypes.None
            };
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

        public override bool RefreshWithoutSave()
        {
            bool hasChanged = base.RefreshWithoutSave();
            return hasChanged;
        }

        public override void Rename()
        {
            Rename("GrowthData");
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
#endif
            Debug.LogFormat("성장 시스템 데이터 에셋의 갱신을 시작합니다.");

            base.RefreshAll();
            OnRefreshAll();

            Debug.LogFormat("성장 시스템 데이터 에셋의 갱신을 종료합니다.");
        }

        protected override void CreateAll()
        {
            base.CreateAll();
            PathManager.UpdatePathMetaData();
        }

        [FoldoutGroup("#Button", false, 7)]
        [Button("모든 성장 능력치 데이터 자동 생성", ButtonSizes.Large)]
        private void CreateAllStatData()
        {
            if (DataArray != null && DataArray.Length > 0)
            {
                if (!EditorUtility.DisplayDialog("데이터 생성", "기존 데이터를 모두 교체하시겠습니까?", "예", "아니오"))
                {
                    return;
                }
            }

            // 성장 시스템 능력치만 생성
            StatNames[] growthStatNames = new StatNames[]
            {
                StatNames.Strength,
                StatNames.HealthPoint,
                StatNames.Vitality,
                StatNames.Critical,
                StatNames.Luck,
                StatNames.AccuracyStat,
                StatNames.Dodge
            };

            System.Collections.Generic.List<GrowthData> dataList = new();

            // 각 능력치별 기본값 설정
            System.Collections.Generic.Dictionary<StatNames, (int maxLevel, int initialCost, float costGrowthRate, float statIncrease)> defaultValues =
                new()
                {
                { StatNames.Strength, (1000, 1, 1.0f, 1f) },
                { StatNames.HealthPoint, (1000, 1, 1.0f, 1f) },
                { StatNames.Vitality, (1000, 1, 1.0f, 1f) },
                { StatNames.Critical, (200, 1, 1.0f, 1f) },
                { StatNames.Luck, (1000, 1, 1.0f, 1f) },
                { StatNames.AccuracyStat, (200, 1, 1.0f, 1f) },
                { StatNames.Dodge, (200, 1, 1.0f, 1f) }
            };

            for (int i = 0; i < growthStatNames.Length; i++)
            {
                StatNames statName = growthStatNames[i];
                if (statName == StatNames.None)
                {
                    continue;
                }

                if (defaultValues.TryGetValue(statName, out (int maxLevel, int initialCost, float costGrowthRate, float statIncrease) values))
                {
                    GrowthData data = new()
                    {
                        StatName = statName,
                        MaxLevel = values.maxLevel,
                        GrowthType = GetGrowthTypeByStatName(statName),
                        StatIncreasePerLevel = values.statIncrease
                    };

                    dataList.Add(data);
                }
            }

            DataArray = dataList.ToArray();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.LogFormat("모든 성장 능력치 데이터를 생성했습니다. 총 {0}개", dataList.Count);
        }

#endif
    }
}