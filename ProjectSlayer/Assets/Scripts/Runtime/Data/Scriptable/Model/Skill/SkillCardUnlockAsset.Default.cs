using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace TeamSuneat.Data
{
    public partial class SkillCardUnlockAsset
    {
        [FoldoutGroup("#Button")]
        [Button("기본값 설정", ButtonSizes.Medium)]
        public void SetDefaultValues()
        {
            UnlockDataList ??= new List<SkillCardUnlockAssetData>();

            UnlockDataList.Clear();

            // 스킬 카드 해금 데이터 초기 설정
            // 레벨 10
            AddUnlockData(SkillNames.FlameSlash, 10);
            AddUnlockData(SkillNames.EarthBlessing, 10);

            // 레벨 15
            AddUnlockData(SkillNames.FlowingBlade, 15);
            AddUnlockData(SkillNames.AccelerationSword, 15);

            // 레벨 20
            AddUnlockData(SkillNames.FireSword, 20);
            AddUnlockData(SkillNames.StoneStrike, 20);

            // 레벨 25
            AddUnlockData(SkillNames.WindingBlade, 25);
            AddUnlockData(SkillNames.LightningSlash, 25);

            // 레벨 30
            // 타입별 보너스 (불 속성) - 불 속성석 10개 필요
            AddUnlockData(SkillNames.BurningSword, 30, requiredCurrencyName: CurrencyNames.AttributeStoneFire, requiredCurrencyCount: 10);
            AddUnlockData(SkillNames.EarthWill, 30);

            // 레벨 35
            AddUnlockData(SkillNames.IceStone, 35);
            AddUnlockData(SkillNames.WindSword, 35);

            // 레벨 40
            AddUnlockData(SkillNames.HeatWave, 40);
            AddUnlockData(SkillNames.LightningFast, 40);

            // 레벨 45
            AddUnlockData(SkillNames.WaveSlash, 45);

            // 레벨 50
            AddUnlockData(SkillNames.ThunderStrike, 50);
            // 강철 의지 (땅 속성) - 땅 속성석 20개 필요
            AddUnlockData(SkillNames.SteelWill, 50, requiredCurrencyName: CurrencyNames.AttributeStoneEarth, requiredCurrencyCount: 20);

            // 레벨 60
            // 워터 쇼워 (물 속성) - 물 속성석 20개 필요
            AddUnlockData(SkillNames.IceShower, 60, requiredCurrencyName: CurrencyNames.AttributeStoneWater, requiredCurrencyCount: 20);
            // 파워 스트라이크 (땅 속성) - 땅 속성석 20개 필요
            AddUnlockData(SkillNames.PowerStrike, 60, requiredCurrencyName: CurrencyNames.AttributeStoneEarth, requiredCurrencyCount: 20);

            // 레벨 70
            // 화염 슬래시 (불 속성) - 불 속성석 20개 필요
            AddUnlockData(SkillNames.FireSlash, 70, requiredCurrencyName: CurrencyNames.AttributeStoneFire, requiredCurrencyCount: 20);
            // 번개 슬래시 (바람 속성) - 바람 속성석 20개 필요
            AddUnlockData(SkillNames.ThunderSlash, 70, requiredCurrencyName: CurrencyNames.AttributeStoneWind, requiredCurrencyCount: 20);

            // 레벨 80
            AddUnlockData(SkillNames.DancingWave, 80);

            // 레벨 90
            AddUnlockData(SkillNames.FireWave, 90);
            AddUnlockData(SkillNames.HighSpeedMovement, 90);

            // 레벨 100
            // 명상 (물 속성) - 물 속성석 50개 필요
            AddUnlockData(SkillNames.Meditation, 100, requiredCurrencyName: CurrencyNames.AttributeStoneWater, requiredCurrencyCount: 50);
            AddUnlockData(SkillNames.PowerImpact, 100);

            // 레벨 120
            // 지옥 화염 슬래시 (불 속성) - 불 속성석 300개 필요
            AddUnlockData(SkillNames.HellfireSlash, 120, requiredCurrencyName: CurrencyNames.AttributeStoneFire, requiredCurrencyCount: 300);

            // 레벨 140
            // 아수라 번개 슬래시 (바람 속성) - 바람 속성석 300개 필요
            AddUnlockData(SkillNames.AsuraLightningSlash, 140, requiredCurrencyName: CurrencyNames.AttributeStoneWind, requiredCurrencyCount: 300);
            // 생명 마나 (땅 속성) - 땅 속성석 50개 필요
            AddUnlockData(SkillNames.LifeMana, 140, requiredCurrencyName: CurrencyNames.AttributeStoneEarth, requiredCurrencyCount: 50);

            // 레벨 160
            // 아이스 타임 (물 속성) - 물 속성석 300개 필요
            AddUnlockData(SkillNames.IceTime, 160, requiredCurrencyName: CurrencyNames.AttributeStoneWater, requiredCurrencyCount: 300);

            // 레벨 180
            // 기가 스트라이크 (땅 속성) - 땅 속성석 300개 필요
            AddUnlockData(SkillNames.GigaStrike, 180, requiredCurrencyName: CurrencyNames.AttributeStoneEarth, requiredCurrencyCount: 300);

            // 레벨 200
            // 분노 (불 속성) - 불 속성석 100개 필요
            AddUnlockData(SkillNames.Rage, 200, requiredCurrencyName: CurrencyNames.AttributeStoneFire, requiredCurrencyCount: 100);
            AddUnlockData(SkillNames.RedThunder, 200);

            // 레벨 250
            AddUnlockData(SkillNames.ManaBlessing, 250);

            // 레벨 300
            AddUnlockData(SkillNames.TrueHeatWave, 300);
            AddUnlockData(SkillNames.ThunderGod, 300);

            // 레벨 350
            AddUnlockData(SkillNames.Blizzard, 350);

            // 레벨 400
            AddUnlockData(SkillNames.FirePillar, 400);

            // 레벨 450
            AddUnlockData(SkillNames.Swiftness, 450);

            // 레벨 500
            AddUnlockData(SkillNames.BeastHunt, 500);

            // 특별 해금 (레벨 0으로 설정 - 특별 조건 필요)
            // 기가 임팩트 - 전사 1레벨 및 명상석 4개 필요 (무기/방어구 없음이므로 ItemNames.None으로 설정)
            AddUnlockData(SkillNames.GigaImpact, 0);
            // 워리어 번 - 전사 1레벨 및 명상석 4개 필요 (무기/방어구 없음이므로 ItemNames.None으로 설정)
            AddUnlockData(SkillNames.WarriorBurn, 0);
            // 급류 - 전사 4레벨 및 명상석 4개 필요 (무기/방어구 없음이므로 ItemNames.None으로 설정)
            AddUnlockData(SkillNames.Torrent, 0);
            // 초인적 힘 - 전사 3레벨 및 명상석 4개 필요 (무기/방어구 없음이므로 ItemNames.None으로 설정)
            AddUnlockData(SkillNames.SuperhumanStrength, 0);

            // 매혹 해금 (레벨 0으로 설정 - 특별 조건 필요)
            // 레이브 - 매혹 스킬 1개 또는 전사 1레벨 및 스킬 1개 필요 (무기/방어구 없음이므로 ItemNames.None으로 설정)
            AddUnlockData(SkillNames.Rave, 0);
            // 만트라 - 매혹 명상석 1개 또는 전사 1레벨 및 명상석 1개 필요 (무기/방어구 없음이므로 ItemNames.None으로 설정)
            AddUnlockData(SkillNames.Mantra, 0);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            Log.Info(LogTags.ScriptableData, "[SkillCardUnlock] 기본값이 설정되었습니다. 총 {0}개의 스킬: {1}", UnlockDataList.Count, name);
        }

        private void AddUnlockData(SkillNames skillName, int unlockLevel, ItemNames requiredItemName = ItemNames.None, int requiredItemCount = 0, CurrencyNames requiredCurrencyName = CurrencyNames.None, int requiredCurrencyCount = 0)
        {
            UnlockDataList.Add(new SkillCardUnlockAssetData
            {
                SkillName = skillName,
                UnlockLevel = unlockLevel,
                RequiredItemName = requiredItemName,
                RequiredItemCount = requiredItemCount,
                RequiredCurrencyName = requiredCurrencyName,
                RequiredCurrencyCount = requiredCurrencyCount
            });
        }
    }
}