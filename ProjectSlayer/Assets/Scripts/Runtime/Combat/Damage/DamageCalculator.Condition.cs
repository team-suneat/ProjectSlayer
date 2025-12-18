using TeamSuneat.Data;
using TeamSuneat.Setting;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        private bool TryCheatDamage(HitmarkAssetData damageAsset, ref DamageResult damageResult)
        {
            // 치트 피해 관련 로직이 필요할 경우 여기에 구현
            return false;
        }

        // 명중/회피 판정: (명중률 - 적 회피율)에 따른 판정
        private bool DetermineEvasion(DamageResult damageResult)
        {
            if (damageResult.Asset.IgnoreEvasion)
            {
                return false;
            }

            if (Attacker == null || damageResult.TargetCharacter == null)
            {
                return false;
            }

            // 공격자 명중률
            float attackerAccuracy = Attacker.Stat.FindValueOrDefault(StatNames.AccuracyChance);
            // 피격자 회피율
            float targetEvasion = damageResult.TargetCharacter.Stat.FindValueOrDefault(StatNames.DodgeChance);

            // 명중 판정: 명중률 - 적 회피율
            float hitChance = attackerAccuracy - targetEvasion;

            // 명중 실패 (회피 성공) 판정
            if (RandomEx.GetFloatValue() >= hitChance)
            {
                SpawnFloatyText("Evasion");
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_DODGE, damageResult);
                LogEvasionApplied(hitChance, attackerAccuracy, targetEvasion);
                return true;
            }

            LogEvasionNotApplied(hitChance, attackerAccuracy, targetEvasion);
            return false;
        }

        private bool DetermineImmuneCC(bool isCrowdControl)
        {
            if (!isCrowdControl)
            {
                return false;
            }
            else if (TargetCharacter == null)
            {
                return false;
            }
            else if (TargetCharacter.IsPlayer && GameSetting.Instance.Cheat.NotCrowdControl)
            {
                SpawnFloatyText("Immune");
                return true;
            }
            else if (TargetCharacter.IgnoreCrowdControl)
            {
                SpawnFloatyText("Immune");
                return true;
            }

            return false;
        }

        private bool DetermineCritical(HitmarkAssetData hitmarkAssetData)
        {
            if (Attacker == null || TargetCharacter == null)
            {
                return false;
            }

            if (Attacker != TargetCharacter && Attacker.IsPlayer)
            {
                if (GameSetting.Instance.Cheat.CriticalType == GameCheat.CriticalTypes.Critical)
                {
                    LogInfoCriticalAppliedByCheat();
                    return true;
                }
                else if (GameSetting.Instance.Cheat.CriticalType == GameCheat.CriticalTypes.NoCritical)
                {
                    LogInfoNoCriticalAppliedByCheat();
                    return false;
                }
            }

            float criticalChance = 0f;
            float targetCriticalChance = 0f;

            TryAddAttackerStatValue(StatNames.CriticalChance, ref criticalChance);

            GameDefineAssetData defineAssetData = ScriptableDataManager.Instance.GetGameDefine().Data;
            float resultCriticalChance = criticalChance + targetCriticalChance;

            // 치명타 확률이 0% 이하이면 치명타 발생하지 않음
            if (resultCriticalChance <= 0f)
            {
                LogCriticalHitNotApplied(resultCriticalChance, criticalChance, targetCriticalChance);
                return false;
            }

            float randomValue = RandomEx.GetFloatValue();
            if (randomValue < resultCriticalChance)
            {
                LogCriticalHitApplied(resultCriticalChance, criticalChance, targetCriticalChance);
                return true;
            }

            LogCriticalHitNotApplied(resultCriticalChance, criticalChance, targetCriticalChance);
            return false;
        }

        private bool DetermineExecute(DamageResult damageResult)
        {
            if (damageResult.TargetVital != null)
            {
                if (damageResult.TargetVital.HealthRate <= damageResult.Asset.ExecutionConditionalTargetHealthRate)
                {
                    LogExecutionApplied(damageResult.TargetVital.HealthRate, damageResult.Asset.ExecutionConditionalTargetHealthRate);
                    return true;
                }
            }

            return false;
        }

        // 회심의 일격 판정: 회심의 일격 확률에 따른 랜덤 판정
        private bool DetermineDevastatingStrike(DamageResult damageResult)
        {
            if (Attacker == null || TargetCharacter == null)
            {
                return false;
            }

            // 플레이어만 회심의 일격 사용 가능
            if (!Attacker.IsPlayer)
            {
                return false;
            }

            // 회심의 일격 확률 가져오기
            float devastatingStrikeChance = Attacker.Stat.FindValueOrDefault(StatNames.DevastatingStrikeChance);

            // 회심의 일격 확률이 0% 이하이면 발생하지 않음
            if (devastatingStrikeChance <= 0f)
            {
                LogDevastatingStrikeNotApplied(devastatingStrikeChance);
                return false;
            }

            float randomValue = RandomEx.GetFloatValue();
            if (randomValue < devastatingStrikeChance)
            {
                LogDevastatingStrikeApplied(devastatingStrikeChance);
                return true;
            }

            LogDevastatingStrikeNotApplied(devastatingStrikeChance);
            return false;
        }
    }
}