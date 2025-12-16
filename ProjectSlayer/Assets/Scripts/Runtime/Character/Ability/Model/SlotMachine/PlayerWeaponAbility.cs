using System.Collections;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerWeaponAbility : CharacterAbility
    {
        private const int MIN_HIT_COUNT = 1;
        private const int MIN_ATTACK_RANGE = 0;

        public override void Initialization()
        {
            base.Initialization();
        }

        public void Apply(ItemNames itemName, System.Action onCompleted = null)
        {
            if (itemName == ItemNames.None)
            {
                onCompleted?.Invoke();
                return;
            }

            if (Owner == null)
            {
                Log.Warning(LogTags.UI_SlotMachine, "플레이어 캐릭터를 찾을 수 없습니다.");
                onCompleted?.Invoke();
                return;
            }

            WeaponData weaponData = JsonDataManager.FindWeaponDataClone(itemName);
            if (weaponData == null || weaponData.Name == ItemNames.None)
            {
                Log.Warning(LogTags.UI_SlotMachine, "무기 데이터를 찾을 수 없습니다: {0}", itemName);
                onCompleted?.Invoke();
                return;
            }

            if (weaponData.IsBlock)
            {
                Log.Warning(LogTags.UI_SlotMachine, "현재 빌드에서 차단된 무기입니다: {0}", weaponData.Name);
                onCompleted?.Invoke();
                return;
            }

            if (weaponData.RewardCurrency != CurrencyNames.None)
            {
                ApplyRewardCurrency(weaponData);
                onCompleted?.Invoke();
                return;
            }

            if (weaponData.AttackRange <= MIN_ATTACK_RANGE)
            {
                StartCoroutine(ApplyWeaponEffectToCharacterCoroutine(Owner, weaponData, onCompleted));
                return;
            }

            // 사거리가 있는 무기는 현재 타일 시스템 없이 처리 불가
            Log.Warning(LogTags.UI_SlotMachine, "사거리가 있는 무기는 타일 시스템이 필요합니다. AttackRange={0}", weaponData.AttackRange);
            onCompleted?.Invoke();
        }


        private void ApplyRewardCurrency(WeaponData weaponData)
        {
            if (weaponData.RewardCurrency == CurrencyNames.None)
            {
                return;
            }

            VWeapon weaponInfo = ProfileInfo.Weapon.FindWeapon(weaponData.Name);
            if (weaponInfo == null)
            {
                Log.Warning(LogTags.Weapon, "무기를 찾을 수 없습니다: {0}", weaponData.Name);
                return;
            }

            int amount = weaponData.Damage;
            int additionalAmount = 0;

            switch (weaponData.RewardCurrency)
            {
                case CurrencyNames.Gold:
                    additionalAmount = Owner.Stat.FindValueOrDefaultToInt(StatNames.InstantGold);
                    break;

                case CurrencyNames.Gem:
                    additionalAmount = Owner.Stat.FindValueOrDefaultToInt(StatNames.InstantGem);
                    break;
            }

            ProfileInfo.Currency.Add(weaponData.RewardCurrency, amount + additionalAmount);
        }

        private IEnumerator ApplyWeaponEffectToCharacter(Character targetCharacter, WeaponData weaponData)
        {
            if (targetCharacter == null || targetCharacter.MyVital == null)
            {
                yield break;
            }

            int hitCount = Mathf.Max(MIN_HIT_COUNT, weaponData.MultiHitCount);

            Log.Info(LogTags.Weapon, "무기 효과 적용: 대상={0}, HitCount={1}", targetCharacter.Name.ToLogString(), hitCount.ToSelectString());

            Owner.SetTarget(targetCharacter);
            AttackEntity attackEntity = Owner.Attack.FindEntity(weaponData.Hitmark);
            if (attackEntity != null)
            {
                attackEntity.SetTarget(targetCharacter.MyVital);
            }
            float? weaponDamageOverride = weaponData.Damage;
            for (int i = 0; i < hitCount; i++)
            {
                Owner.Attack.Activate(weaponData.Hitmark, weaponDamageOverride);
                // 각 공격 사이에 작은 지연 추가
                if (i < hitCount - 1)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator ApplyWeaponEffectToCharacterCoroutine(Character targetCharacter, WeaponData weaponData, System.Action onCompleted)
        {
            yield return StartCoroutine(ApplyWeaponEffectToCharacter(targetCharacter, weaponData));
            onCompleted?.Invoke();
        }

    }
}