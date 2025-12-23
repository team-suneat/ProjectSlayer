using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterCharacter : Character
    {
        [FoldoutGroup("#Character/Component/Monster")]
        [ChildGameObjectsOnly]
        [SerializeField] private DropObjectSpawner _dropObjectSpawner;

        public override Transform Target => null;

        public override LogTags LogTag => LogTags.Monster;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _dropObjectSpawner = GetComponentInChildren<DropObjectSpawner>();
        }

        public override void Initialize()
        {
            SetupLevel();

            base.Initialize();

            PlaySpawnAnimation();
            CharacterManager.Instance.Register(this);
        }

        //

        public override void ResetTarget()
        {
            base.ResetTarget();
        }

        public override void SetTarget(Vital targetVital)
        {
            if (targetVital == null)
            {
                return;
            }

            if (targetVital.Owner == null)
            {
                return;
            }

            TargetCharacter = targetVital.Owner;
        }

        public override void SetTarget(Character targetCharacter)
        {
            TargetCharacter = targetCharacter;
        }

        //
        public override void AddCharacterStats()
        {
            MonsterStatConfigAsset monsterStatConfigAsset = ScriptableDataManager.Instance.GetMonsterStatConfigAsset();
            if (monsterStatConfigAsset.IsValid())
            {
                bool isTreasureChest = Name == CharacterNames.TreasureChest;
                int health = monsterStatConfigAsset.GetHealth(Level, IsBoss, isTreasureChest);
                int attack = monsterStatConfigAsset.GetAttack(Level, IsBoss, isTreasureChest);
                Stat.AddWithSourceInfo(StatNames.Health, health, this, NameString, "CharacterBase");
                Stat.AddWithSourceInfo(StatNames.Attack, attack, this, NameString, "CharacterBase");
            }
            else
            {
                Log.Error("몬스터의 능력치를 불러올 수 없습니다.");
            }
        }

        //

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            _dropObjectSpawner?.SpawnDropEXP(position);
            CharacterAnimator?.PlayDeathAnimation();

            if (IsBoss)
            {
                GlobalEvent<Character>.Send(GlobalEventType.BOSS_CHARACTER_DEATH, this);
            }
            else
            {
                GlobalEvent<Character>.Send(GlobalEventType.MONSTER_CHARACTER_DEATH, this);
            }
        }
    }
}