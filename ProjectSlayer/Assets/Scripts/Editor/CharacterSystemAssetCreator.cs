using System.Collections.Generic;
using System.IO;
using TeamSuneat;
using TeamSuneat.Data;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Editor
{
    /// <summary>
    /// 캐릭터 시스템 관련 ScriptableObject 에셋을 생성하는 Editor 스크립트
    /// </summary>
    public static class CharacterSystemAssetCreator
    {
        private const string TARGET_FOLDER = "Assets/Addressables/Scriptable/Character";

        [MenuItem("TeamSuneat/Create Character System Assets")]
        public static void CreateCharacterSystemAssets()
        {
            // 폴더가 없으면 생성
            if (!Directory.Exists(TARGET_FOLDER))
            {
                Directory.CreateDirectory(TARGET_FOLDER);
                AssetDatabase.Refresh();
            }

            CreateEnhancementDataAsset();
            CreateGrowthDataAsset();
            CreateExperienceConfigAsset();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("캐릭터 시스템 에셋 생성이 완료되었습니다.");
        }

        private static void CreateEnhancementDataAsset()
        {
            string assetPath = Path.Combine(TARGET_FOLDER, "EnhancementData.asset");
            assetPath = assetPath.Replace("\\", "/");

            // 이미 존재하면 삭제
            if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            EnhancementDataAsset asset = ScriptableObject.CreateInstance<EnhancementDataAsset>();
            asset.NameString = "EnhancementData";

            // 기본 데이터 생성
            StatNames[] statNames = EnumEx.GetValues<StatNames>();
            List<EnhancementData> dataList = new List<EnhancementData>();

            for (int i = 1; i < statNames.Length; i++)
            {
                if (statNames[i] == StatNames.None)
                {
                    continue;
                }

                EnhancementData data = new EnhancementData
                {
                    StatName = statNames[i],
                    MaxLevel = 100,
                    InitialValue = 0f,
                    GrowthValue = 1f,
                    InitialCost = 100,
                    CostGrowthRate = 1.1f,
                    RequiredStatName = StatNames.None,
                    RequiredStatLevel = 0
                };

                dataList.Add(data);
            }

            asset.DataArray = dataList.ToArray();

            AssetDatabase.CreateAsset(asset, assetPath);
            EditorUtility.SetDirty(asset);

            Debug.LogFormat("EnhancementData 에셋을 생성했습니다: {0} (총 {1}개 데이터)", assetPath, dataList.Count);
        }

        private static void CreateGrowthDataAsset()
        {
            string assetPath = Path.Combine(TARGET_FOLDER, "GrowthData.asset");
            assetPath = assetPath.Replace("\\", "/");

            // 이미 존재하면 삭제
            if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            GrowthDataAsset asset = ScriptableObject.CreateInstance<GrowthDataAsset>();
            asset.NameString = "GrowthData";

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

            List<GrowthData> dataList = new List<GrowthData>();

            // 각 능력치별 기본값 설정
            Dictionary<StatNames, (int maxLevel, int initialCost, float costGrowthRate, float statIncrease)> defaultValues =
                new Dictionary<StatNames, (int, int, float, float)>
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

                if (defaultValues.TryGetValue(statName, out var values))
                {
                    GrowthData data = new GrowthData
                    {
                        StatName = statName,
                        MaxLevel = values.maxLevel,
                        InitialCost = values.initialCost,
                        CostGrowthRate = values.costGrowthRate,
                        StatIncreasePerLevel = values.statIncrease
                    };

                    dataList.Add(data);
                }
            }

            asset.DataArray = dataList.ToArray();

            AssetDatabase.CreateAsset(asset, assetPath);
            EditorUtility.SetDirty(asset);

            Debug.LogFormat("GrowthData 에셋을 생성했습니다: {0} (총 {1}개 데이터)", assetPath, dataList.Count);
        }

        private static void CreateExperienceConfigAsset()
        {
            string assetPath = Path.Combine(TARGET_FOLDER, "ExperienceConfig.asset");
            assetPath = assetPath.Replace("\\", "/");

            // 이미 존재하면 삭제
            if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            ExperienceConfigAsset asset = ScriptableObject.CreateInstance<ExperienceConfigAsset>();
            asset.NameString = "ExperienceConfig";

            // 기본값 설정
            asset.InitialExperienceRequired = 120;
            asset.ExperienceGrowthRate = 1.01f;
            asset.StatPointPerLevel = 3;

            AssetDatabase.CreateAsset(asset, assetPath);
            EditorUtility.SetDirty(asset);

            Debug.LogFormat("ExperienceConfig 에셋을 생성했습니다: {0}", assetPath);
        }
    }
}

