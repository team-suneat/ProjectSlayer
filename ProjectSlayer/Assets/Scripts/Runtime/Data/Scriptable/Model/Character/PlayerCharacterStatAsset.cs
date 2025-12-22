using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "PlayerCharacterStat", menuName = "TeamSuneat/Scriptable/PlayerCharacterStat")]
    public class PlayerCharacterStatAsset : XScriptableObject
    {
        public int BaseAttack = 1; // 기본 공격력 (1)
        public int BaseHealth = 10; // 기본 체력 (10)
        public int BaseHealthRegen = 1; // 기본 체력 회복량 (1초에 1만큼 회복)
        public float BaseAttackSpeed = 1.0f; // 기본 공격 속도 (100%)
        public float BaseCriticalChance = 0.001f; // 기본 치명타 확률 (0.1%)
        public float BaseCriticalDamage = 0.01f; // 기본 치명타 피해 (1%)
        public int BaseMana = 100; // 기본 마나 (100)
        public int BaseManaRegen = 1; // 기본 마나 회복량 (1초에 1만큼 회복)
        public float BaseAccuracyChance = 0.75f; // 기본 명중률 (75%)
        public float BaseDodgeChance = 0.1f; // 기본 회피률 (10%)
        public float BaseGoldGain = 1.0f; // 기본 골드 획득량 배율 (100%)
        public float BaseXPGain = 1.0f; // 기본 경험치 획득량 배율 (100%)

        public override void Rename()
        {
#if UNITY_EDITOR
            PerformRename("PlayerCharacterStat");
#endif
        }
    }
}