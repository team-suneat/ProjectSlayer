using TeamSuneat.Data;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        protected override void OnStart()
        {
            base.OnStart();
            Life?.RegisterOnDeathEvent(OnDeath);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            Life?.UnregisterOnDeathEvent(OnDeath);
            UIManager.Instance?.GaugeManager?.UnregisterEnemy(this);
        }

        public virtual void OnBattleReady()
        {
            if (Life != null)
            {
                Life.Initialize();
            }

            if (Shield != null)
            {
                Shield.Initialize();
            }

            if (UseSpawnGaugeOnInit)
            {
                SpawnEnemyGauge();
            }

            Generate();
            RegisterVital();
            StartRegenerate();
        }

        public void RegisterVital()
        {
            VitalManager.Instance?.Add(this);
        }

        public void UnregisterVital()
        {
            VitalManager.Instance?.Remove(this);
        }

        public void Despawn()
        {
            if (Owner != null)
            {
                Owner.Despawn();
            }
        }

        public void OnLevelUp(StatSystem statSystem, int previousLife)
        {
            if (Life != null)
            {
                Life.RefreshMaxValue();

                if (previousLife > CurrentLife)
                {
                    // 레벨 업을 통해 능력치가 재조정됨에 따라 현재 생명력이 더 낮아졌다면, 되돌립니다.
                    CurrentLife = previousLife;
                }

                RefreshLifeGauge();
            }
        }

        public void OnLevelDown()
        {
            if (Life != null)
            {
                Life.RefreshMaxValue();
                RefreshLifeGauge();
            }
        }

        public bool CheckDamageImmunity(DamageResult damageResult)
        {
            if (Life.CheckInvulnerable())
            {
                Life.UseZero();
                return true;
            }
            else if (damageResult.IsEvasion)
            {
                Life.UseZero();
                return true;
            }

            return false;
        }

        public bool TakeDamage(DamageResult damageResult)
        {
            if (CurrentLife <= 0)
            {
                return false;
            }
            else if (damageResult.DamageValue <= 0)
            {
                return false;
            }
            else if (Shield != null && CurrentShield > 0)
            {
                if (CurrentShield >= damageResult.DamageValue)
                {
                    if (Shield.UseCurrentValue(damageResult.DamageValueToInt, damageResult))
                    {
                        Shield.SpawnShieldFloatyText(damageResult.DamageValueToInt);
                        damageResult.DamageValue = 0;

                        SendGlobalEventOfDamaged(damageResult);

                        return false;
                    }
                }
                else
                {
                    int reduceDamageValue = Shield.Current;
                    if (Shield.UseCurrentValue(damageResult.DamageValueToInt, damageResult))
                    {
                        Shield.SpawnShieldFloatyText(reduceDamageValue);
                        damageResult.DamageValue -= reduceDamageValue;

                        if (damageResult.DamageValue > 0)
                        {
                            Life.TakeDamage(damageResult, damageResult.Attacker);
                            SendGlobalEventOfDamaged(damageResult);
                        }
                    }
                    else
                    {
                        LogErrorUseShield();
                    }
                }
            }
            else if (damageResult.DamageValue > 0)
            {
                Life.TakeDamage(damageResult, damageResult.Attacker);
                SendGlobalEventOfDamaged(damageResult);

                return true;
            }
            else
            {
                LogErrorTakeDamageZero(damageResult.HitmarkName);
            }

            return false;
        }

        private void SendGlobalEventOfDamaged(DamageResult damageResult)
        {
            if (Owner == null)
            {
                return;
            }

            if (Owner.IsPlayer)
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_DAMAGED, damageResult);
            }
            else if (Owner.IsBoss)
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.BOSS_CHARACTER_DAMAGED, damageResult);
            }
            else
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.MONSTER_CHARACTER_DAMAGED, damageResult);
            }
        }

        // Damage Hitmark On Hit

        private void ApplyDamageInfo(DamageResult damageResult, Vital targetVital)
        {
            switch (damageResult.DamageType)
            {
                case DamageTypes.Heal:
                    {
                        int healValue = Mathf.CeilToInt(damageResult.DamageValue);

                        targetVital.Heal(healValue);
                    }
                    break;

                case DamageTypes.Charge:
                    {
                        int chargeValue = Mathf.CeilToInt(damageResult.DamageValue);

                        targetVital.Charge(chargeValue);
                    }
                    break;

                default:
                    {
                        if (!targetVital.CheckDamageImmunity(damageResult))
                        {
                            _ = targetVital.TakeDamage(damageResult);
                        }
                    }
                    break;
            }
        }

        // Event

        protected virtual void OnDeath(DamageResult damageResult)
        {
            DieEvent?.Invoke();
        }

        public void Heal(int value)
        {
            Life?.Heal(value);
        }

        public void Charge(int value)
        {
            _ = (Shield?.AddCurrentValue(value));
        }

        public void AddCurrentValue(VitalConsumeTypes consumeType, float value)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedLife:
                    {
                        Heal((int)value);
                    }
                    break;

                case VitalConsumeTypes.MaxLifePercent:
                    {
                        Heal(Mathf.RoundToInt(MaxLife * value));
                    }
                    break;

                case VitalConsumeTypes.CurrentLifePercent:
                    {
                        Heal(Mathf.RoundToInt(CurrentLife * value));
                    }
                    break;

                case VitalConsumeTypes.FixedShield:
                    {
                        Charge((int)value);
                    }
                    break;

                case VitalConsumeTypes.MaxShieldPercent:
                    {
                        Charge(Mathf.RoundToInt(MaxShield * value));
                    }
                    break;

                case VitalConsumeTypes.CurrentShieldPercent:
                    {
                        Charge(Mathf.RoundToInt(CurrentShield * value));
                    }
                    break;

                default:
                    {
                        LogErrorAddResource(consumeType, value);
                    }
                    break;
            }
        }

        public void AddCurrentValue(VitalConsumeTypes consumeType, int value)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedLife:
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.CurrentLifePercent:
                    {
                        Heal(value);
                    }
                    break;

                case VitalConsumeTypes.FixedShield:
                case VitalConsumeTypes.MaxShieldPercent:
                case VitalConsumeTypes.CurrentShieldPercent:
                    {
                        Charge(value);
                    }
                    break;

                default:
                    {
                        LogErrorAddResource(consumeType, value);
                    }
                    break;
            }
        }

        public void UseCurrentValue(HitmarkAssetData hitmarkAssetData, int value)
        {
            switch (hitmarkAssetData.ResourceConsumeType)
            {
                case VitalConsumeTypes.FixedLife:
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.CurrentLifePercent:
                    {
                        if (Life != null)
                        {
                            if (value > 0)
                            {
                                Life.Use(value, Owner, hitmarkAssetData.IgnoreDeathByConsume);
                                return;
                            }
                        }
                    }
                    break;

                case VitalConsumeTypes.FixedShield:
                case VitalConsumeTypes.MaxShieldPercent:
                case VitalConsumeTypes.CurrentShieldPercent:
                    {
                        if (Shield != null)
                        {
                            if (value > 0)
                            {
                                _ = Shield.UseCurrentValue(value);
                                return;
                            }
                        }
                    }
                    break;
            }

            LogErrorUseBattleResource(hitmarkAssetData, value);
        }

        public virtual void ProcessAbility()
        { }

        public virtual bool CheckConsumingPotion()
        {
            return true;
        }

        #region Get Value

        public float GetCurrent(VitalResourceTypes resourceType)
        {
            switch (resourceType)
            {
                case VitalResourceTypes.None:
                    return 0;

                case VitalResourceTypes.Life:
                    if (Life != null)
                    {
                        return Life.Current;
                    }
                    break;

                case VitalResourceTypes.Shield:
                    if (Shield != null)
                    {
                        return Shield.Current;
                    }
                    break;
            }

            LogErrorFindCurrentResource(resourceType);
            return 0f;
        }

        public int GetCurrent(VitalConsumeTypes consumeType)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.None:
                    return 0;

                case VitalConsumeTypes.CurrentLifePercent:
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.FixedLife:
                    return CurrentLife;

                case VitalConsumeTypes.CurrentShieldPercent:
                case VitalConsumeTypes.MaxShieldPercent:
                case VitalConsumeTypes.FixedShield:
                    return CurrentShield;
            }

            LogErrorFindCurrentResource(consumeType);

            return 0;
        }

        public int GetMax(VitalConsumeTypes consumeType)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.None:
                    return 0;

                case VitalConsumeTypes.CurrentLifePercent:
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.FixedLife:
                    return MaxLife;

                case VitalConsumeTypes.CurrentShieldPercent:
                case VitalConsumeTypes.MaxShieldPercent:
                case VitalConsumeTypes.FixedShield:
                    return MaxShield;
            }

            LogErrorFindMaxResource(consumeType);

            return 0;
        }

        public float GetRate(VitalResourceTypes resourceType)
        {
            switch (resourceType)
            {
                case VitalResourceTypes.None:
                    return 0;

                case VitalResourceTypes.Life:
                    if (Life != null)
                    {
                        return Life.Rate;
                    }
                    break;

                case VitalResourceTypes.Shield:
                    if (Shield != null)
                    {
                        return Shield.Rate;
                    }
                    break;
            }

            LogWarningFindResourceRate(resourceType);
            return 0f;
        }

        #endregion Get Value
    }
}