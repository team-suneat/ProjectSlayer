using UnityEngine;

namespace TeamSuneat
{
    public class CharacterAttackAnimationEvent : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        private void StartAttackAnimationEvent(string hitmarkNameString)
        {
            if (_character != null && _character.Attack != null)
            {
                _character.Attack.Activate(hitmarkNameString);
            }
        }
    }
}