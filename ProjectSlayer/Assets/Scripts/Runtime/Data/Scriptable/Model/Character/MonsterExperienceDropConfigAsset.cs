using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "MonsterExperienceDropConfig", menuName = "TeamSuneat/Scriptable/Monster/ExperienceDrop")]
    public class MonsterExperienceDropConfigAsset : XScriptableObject
    {
        [Title("경험치")]
        [SuffixLabel("레벨 1 일반 몬스터의 경험치")]
        public int BaseExp = 15;

        [SuffixLabel("일반 몬스터 경험치 증가 배율 (레벨마다 이전 레벨 대비 증가 배율)")]
        public float ExpGrowthRate = 1.05f;

        [Title("골드")]
        [SuffixLabel("레벨 1 일반 몬스터의 최소 골드")]
        public int BaseMinGold = 13;

        [SuffixLabel("레벨 1 일반 몬스터의 최대 골드")]
        public int BaseMaxGold = 15;

        [SuffixLabel("일반 몬스터 골드 증가 배율 (레벨마다 이전 레벨 대비 증가 배율)")]
        public float GoldGrowthRate = 1.05f;

        [Title("강화 규브")]
        [SuffixLabel("레벨 1 일반 몬스터의 강화큐브")]
        public int BaseCube = 15;

        [SuffixLabel("일반 몬스터 강화큐브 증가 배율 (레벨마다 이전 레벨 대비 증가 배율)")]
        public float CubeGrowthRate = 1.03f;

        [SuffixLabel("일반 몬스터 강화큐브 드랍 확률")]
        public float CubeDropChance = 0.3f;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR

            if (BaseExp <= 0)
            {
                Log.Error("일반 몬스터 최대 경험치가 0 이하입니다: {0}", name);
            }
            if (ExpGrowthRate <= 1.0f)
            {
                Log.Error("일반 몬스터 경험치 증가 배율이 1.0 이하입니다: {0}", name);
            }

#endif
        }

        //
        public int GetGoldDrop(int level, bool isBoss, bool isTreasureBox)
        {
            int minGold = Mathf.RoundToInt(BaseMinGold * Mathf.Pow(GoldGrowthRate, level - 1));
            int maxGold = Mathf.RoundToInt(BaseMaxGold * Mathf.Pow(GoldGrowthRate, level - 1));
            if (isBoss || isTreasureBox)
            {
                int dropGold = 0;
                for (int i = 0; i < 10; i++)
                {
                    dropGold += Random.Range(minGold, maxGold + 1);
                }
                return dropGold;
            }
            else
            {
                return Random.Range(minGold, maxGold + 1);
            }
        }

        public int GetExpDrop(int level, bool isBoss, bool isTreasureBox)
        {
            int dropExp = Mathf.RoundToInt(BaseExp * Mathf.Pow(ExpGrowthRate, level - 1));
            if (isBoss || isTreasureBox)
            {
                return dropExp * 10;
            }
            else
            {
                return dropExp;
            }
        }

        public int GetCubeDrop(int level, bool isBoss, bool isTreasureBox)
        {
            int dropCube = Mathf.RoundToInt(BaseCube * Mathf.Pow(CubeGrowthRate, level - 1));
            if (isBoss || isTreasureBox)
            {
                return dropCube * 10;
            }
            else
            {
                return dropCube;
            }
        }

        public bool TryDropEnhancementCube()
        {
            if (CubeDropChance > 0)
            {
                return RandomEx.GetFloatValue() < CubeDropChance;
            }

            return false;
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
#if UNITY_EDITOR
            PerformRename("MonsterExperienceDropConfig");
#endif
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
            Debug.LogFormat("몬스터 경험치 드랍 설정 에셋의 갱신을 시작합니다.");

            base.RefreshAll();
            OnRefreshAll();

            Debug.LogFormat("몬스터 경험치 드랍 설정 에셋의 갱신을 종료합니다.");
        }

        protected override void CreateAll()
        {
            base.CreateAll();
            PathManager.UpdatePathMetaData();
        }

#endif
    }
}