using Sirenix.OdinInspector;
using TeamSuneat;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "MonsterStatConfig", menuName = "TeamSuneat/Scriptable/MonsterStatConfig")]
    public class MonsterStatConfigAsset : XScriptableObject
    {
        [Title("기본 능력치")]
        [Tooltip("레벨 1 몬스터의 기본 체력")]
        public int BaseHealth = 100;

        [Tooltip("레벨 1 몬스터의 기본 공격력")]
        public int BaseAttack = 10;

        [Title("보스 기본 능력치")]
        [Tooltip("레벨 1 보스의 기본 체력")]
        public int BaseBossHealth = 200;

        [Tooltip("레벨 1 보스의 기본 공격력")]
        public int BaseBossAttack = 20;

        [Title("보물 상자 기본 능력치")]
        [Tooltip("레벨 1 보물 상자의 기본 체력")]
        public int BaseTreasureChestHealth = 150;

        [Tooltip("레벨 1 보물 상자의 기본 공격력")]
        public int BaseTreasureChestAttack = 15;

        [Title("일반 스테이지 증가 배율")]
        [Tooltip("일반 스테이지에서 레벨마다 체력이 증가하는 배율")]
        public float NormalStageHealthGrowthRate = 1.05f;

        [Tooltip("일반 스테이지에서 레벨마다 공격력이 증가하는 배율")]
        public float NormalStageAttackGrowthRate = 1.03f;

        [Title("보스 스테이지 증가 배율")]
        [Tooltip("보스 스테이지에서 레벨마다 체력이 증가하는 배율")]
        public float BossStageHealthGrowthRate = 1.10f;

        [Tooltip("보스 스테이지에서 레벨마다 공격력이 증가하는 배율")]
        public float BossStageAttackGrowthRate = 1.08f;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (BaseHealth <= 0)
            {
                Log.Error("기본 체력이 0 이하입니다: {0}", name);
            }
            if (BaseAttack <= 0)
            {
                Log.Error("기본 공격력이 0 이하입니다: {0}", name);
            }
            if (NormalStageHealthGrowthRate <= 1.0f)
            {
                Log.Error("일반 스테이지 체력 증가 배율이 1.0 이하입니다: {0}", name);
            }
            if (NormalStageAttackGrowthRate <= 1.0f)
            {
                Log.Error("일반 스테이지 공격력 증가 배율이 1.0 이하입니다: {0}", name);
            }
            if (BossStageHealthGrowthRate <= 1.0f)
            {
                Log.Error("보스 스테이지 체력 증가 배율이 1.0 이하입니다: {0}", name);
            }
            if (BossStageAttackGrowthRate <= 1.0f)
            {
                Log.Error("보스 스테이지 공격력 증가 배율이 1.0 이하입니다: {0}", name);
            }
            if (BaseBossHealth <= 0)
            {
                Log.Error("보스 기본 체력이 0 이하입니다: {0}", name);
            }
            if (BaseBossAttack <= 0)
            {
                Log.Error("보스 기본 공격력이 0 이하입니다: {0}", name);
            }
            if (BaseTreasureChestHealth <= 0)
            {
                Log.Error("보물 상자 기본 체력이 0 이하입니다: {0}", name);
            }
            if (BaseTreasureChestAttack <= 0)
            {
                Log.Error("보물 상자 기본 공격력이 0 이하입니다: {0}", name);
            }
#endif
        }

        // 특정 레벨의 체력을 계산합니다.
        // 레벨 n 체력 = BaseHealth × GrowthRate^(n-1)
        public int GetHealth(int level, bool isBossStage, bool isTreasureChest = false)
        {
            if (level < 1)
            {
                if (isTreasureChest)
                {
                    return BaseTreasureChestHealth;
                }
                if (isBossStage)
                {
                    return BaseBossHealth;
                }
                return BaseHealth;
            }

            int baseHealth;
            if (isTreasureChest)
            {
                baseHealth = BaseTreasureChestHealth;
            }
            else if (isBossStage)
            {
                baseHealth = BaseBossHealth;
            }
            else
            {
                baseHealth = BaseHealth;
            }

            float growthRate = isBossStage ? BossStageHealthGrowthRate : NormalStageHealthGrowthRate;
            return Mathf.RoundToInt(baseHealth * Mathf.Pow(growthRate, level - 1));
        }

        // 특정 레벨의 공격력을 계산합니다.
        // 레벨 n 공격력 = BaseAttack × GrowthRate^(n-1)
        public int GetAttack(int level, bool isBossStage, bool isTreasureChest = false)
        {
            if (level < 1)
            {
                if (isTreasureChest)
                {
                    return BaseTreasureChestAttack;
                }
                if (isBossStage)
                {
                    return BaseBossAttack;
                }
                return BaseAttack;
            }

            int baseAttack;
            if (isTreasureChest)
            {
                baseAttack = BaseTreasureChestAttack;
            }
            else if (isBossStage)
            {
                baseAttack = BaseBossAttack;
            }
            else
            {
                baseAttack = BaseAttack;
            }

            float growthRate = isBossStage ? BossStageAttackGrowthRate : NormalStageAttackGrowthRate;
            return Mathf.RoundToInt(baseAttack * Mathf.Pow(growthRate, level - 1));
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
            Rename("MonsterStatConfig");
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
            Debug.LogFormat("몬스터 능력치 설정 에셋의 갱신을 시작합니다.");

            base.RefreshAll();
            OnRefreshAll();

            Debug.LogFormat("몬스터 능력치 설정 에셋의 갱신을 종료합니다.");
        }

        protected override void CreateAll()
        {
            base.CreateAll();
            PathManager.UpdatePathMetaData();
        }
#endif
    }
}

