using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "SummonLevelConfig", menuName = "TeamSuneat/Config/Summon/SummonLevel")]
    public class SummonLevelConfigAsset : XScriptableObject
    {
        [InfoBox("소환 레벨별 필요 소환량 및 보상. 레벨은 오름차순으로 정렬되어 있어야 합니다. 기본값은 인스펙터 '기본값 자동 입력' 버튼으로 채울 수 있습니다.")]
        [SerializeField]
        private SummonLevelRewardData[] _entries;

        public int EntryCount => _entries != null ? _entries.Length : 0;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (_entries == null || _entries.Length == 0)
            {
                Log.Error("소환 레벨 데이터가 비어 있습니다: {0}", name);
                return;
            }
            for (int i = 0; i < _entries.Length - 1; i++)
            {
                if (_entries[i].RequiredSummonCount >= _entries[i + 1].RequiredSummonCount)
                {
                    Log.Error("필요 소환량은 오름차순이어야 합니다. 인덱스 {0}~{1}: {2}", i, i + 1, name);
                }
            }
#endif
        }

        public float GetLevelFromSummonCount(int totalSummonCount)
        {
            if (_entries == null || _entries.Length == 0)
            {
                return 1f;
            }
            float level = 1f;
            for (int i = 0; i < _entries.Length; i++)
            {
                if (totalSummonCount >= _entries[i].RequiredSummonCount)
                {
                    level = _entries[i].Level;
                }
                else
                {
                    break;
                }
            }
            return level;
        }

        public int GetRequiredSummonCountForLevel(float level)
        {
            if (_entries == null || _entries.Length == 0)
            {
                return 0;
            }
            for (int i = _entries.Length - 1; i >= 0; i--)
            {
                if (_entries[i].Level <= level)
                {
                    return _entries[i].RequiredSummonCount;
                }
            }
            return _entries[0].RequiredSummonCount;
        }

        public bool TryGetRewardForLevel(float level, out GradeNames grade, out int quality, out int count)
        {
            grade = GradeNames.None;
            quality = 1;
            count = 0;
            if (_entries == null)
            {
                return false;
            }
            for (int i = 0; i < _entries.Length; i++)
            {
                if (Mathf.Approximately(_entries[i].Level, level) && _entries[i].HasReward)
                {
                    grade = _entries[i].RewardGrade;
                    quality = _entries[i].RewardQuality;
                    count = _entries[i].RewardCount;
                    return true;
                }
            }
            return false;
        }

        public SummonLevelRewardData GetEntry(int index)
        {
            if (_entries == null || index < 0 || index >= _entries.Length)
            {
                return null;
            }
            return _entries[index];
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
            PerformRename("SummonLevelConfig");
        }

        [FoldoutGroup("#Custom Button", 1000)]
        [Button("기본값 자동 입력 (레벨 2~10)", ButtonSizes.Large)]
        private void FillDefaultValues()
        {
            _entries = CreateDefaultEntries();
            EditorUtility.SetDirty(this);
        }

        private static SummonLevelRewardData[] CreateDefaultEntries()
        {
            return new SummonLevelRewardData[]
            {
                new(2f, 100, GradeNames.Rare, 2, 4),
                new(3f, 250, GradeNames.Epic, 2, 4),
                new(4f, 1000, GradeNames.Legendary, 2, 4),
                new(5f, 4000, GradeNames.Mythic, 4, 2),
                new(6f, 12000, GradeNames.Mythic, 1, 1),
                new(7f, 25000, GradeNames.Mythic, 1, 2),
                new(7.5f, 27500, GradeNames.Mythic, 1, 3),
                new(8f, 55000, GradeNames.Mythic, 1, 3),
                new(9f, 88000, GradeNames.None, 1, 0),
                new(10f, 100000, GradeNames.None, 1, 0),
            };
        }

#endif
    }
}