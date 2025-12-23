using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator : MonoBehaviour, IAnimatorStateMachine
    {
        [SerializeField]
        protected Character _ownerCharacter;

        [SerializeField]
        private Animator _animator;

        public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Death"))
            {
                _ownerCharacter.Despawn();
            }
        }

        //

        [FoldoutGroup("#Buttons")]
        [Button(ButtonSizes.Medium)]
        public void AutoGetComponents()
        {
            _ownerCharacter = this.FindFirstParentComponent<Character>();
            _animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            _ownerCharacter = this.FindFirstParentComponent<Character>();
            _animator ??= GetComponent<Animator>();
        }

        //

        internal void Initialize()
        {
            InitializeAnimatorParameters();
        }

        internal void PlaySpawnAnimation()
        {
        }

        internal void PlayAttackAnimation()
        {
            _animator.UpdateAnimatorTrigger(ANIMATOR_IS_ATTACKING_PARAMETER_ID, AnimatorParameters);
        }

        internal void PlayAttackAnimationByHitmark(HitmarkNames hitmarkName)
        {
            _animator.Play(hitmarkName.ToString(), 0);
        }

        internal bool PlayDamageAnimation(HitmarkAssetData asset)
        {
            if (asset.NotPlayDamageAnimation)
            {
                return false;
            }

            return _animator.UpdateAnimatorTrigger(ANIMATOR_DAMAGE_PARAMETER_ID, AnimatorParameters);
        }

        public virtual void PlayDeathAnimation()
        {
            _animator.UpdateAnimatorTrigger(ANIMATOR_DEATH_PARAMETER_ID, AnimatorParameters);
        }

        //

        internal void StopAttacking()
        {
        }

        internal void UpdateAttackSpeed(float value)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_ATTACK_SPEED_PARAMETER_ID, value, AnimatorParameters);
        }

        internal void UpdateAnimatorBool(int parameterId, bool value)
        {
            _animator.UpdateAnimatorBool(parameterId, value, AnimatorParameters);
        }
    }
}