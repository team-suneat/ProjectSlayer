using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerAttackAbility : CharacterAbility
    {
        [SerializeField]
        private HitmarkNames _hitmarkOverride = HitmarkNames.None;

        private PlayerCharacter _player;

        public override void Initialization()
        {
            base.Initialization();
            EnsureReferences();
        }

        public bool CanAttackTarget()
        {
            if (!EnsureReferences())
            {
                return false;
            }

            if (!IsTargetValid(_player.TargetCharacter))
            {
                return false;
            }

            return true;
        }

        public bool TryAttackTarget()
        {
            if (_player != null && _player.ConditionState.Compare(CharacterConditions.Stunned))
            {
                Log.Progress(LogTags.Player, "기절 상태로 인해 공격할 수 없습니다.");
                return false;
            }

            if (!CanAttackTarget())
            {
                return false;
            }

            HitmarkNames hitmark = ResolveHitmark();
            if (hitmark == HitmarkNames.None)
            {
                Log.Warning(LogTags.Player, "플레이어 기본 공격 히트마크가 설정되지 않았습니다.");
                return false;
            }

            return ExecuteAttack(hitmark);
        }

        private bool ExecuteAttack(HitmarkNames hitmark)
        {
            Character targetCharacter = _player.TargetCharacter;
            if (targetCharacter == null || targetCharacter.MyVital == null)
            {
                return false;
            }

            _player.SetTarget(targetCharacter);
            AttackEntity attackEntity = _player.Attack.FindEntity(hitmark);
            if (attackEntity != null)
            {
                attackEntity.SetTarget(targetCharacter.MyVital);
            }

            _player.Attack.Activate(hitmark);

            Log.Info(LogTags.Player, "플레이어가 타겟을 공격했습니다. Target={0}", targetCharacter.Name.ToLogString());
            return true;
        }

        private HitmarkNames ResolveHitmark()
        {
            if (_hitmarkOverride != HitmarkNames.None)
            {
                return _hitmarkOverride;
            }

            if (_player != null && _player.Attack != null && _player.Attack.BasicAttackHitmark != HitmarkNames.None)
            {
                return _player.Attack.BasicAttackHitmark;
            }

            return HitmarkNames.None;
        }

        private bool EnsureReferences()
        {
            _player ??= Owner as PlayerCharacter ?? GetComponent<PlayerCharacter>();

            return _player != null;
        }

        private bool IsTargetValid(Character targetCharacter)
        {
            if (targetCharacter == null)
            {
                return false;
            }

            if (!targetCharacter.IsAlive)
            {
                return false;
            }

            if (targetCharacter.MyVital == null)
            {
                return false;
            }

            return true;
        }
    }
}

