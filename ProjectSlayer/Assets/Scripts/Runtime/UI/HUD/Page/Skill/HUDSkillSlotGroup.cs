using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 인게임 스킬 슬롯 그룹 - 여러 스킬 슬롯을 관리
    public class HUDSkillSlotGroup : XBehaviour
    {
        [Title("#HUDSkillSlotGroup")]
        [SerializeField] private HUDSkillSlot[] _skillSlots;

        private readonly Dictionary<int, HUDSkillSlot> _slotMap = new();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            if (_skillSlots == null || _skillSlots.Length == 0)
            {
                _skillSlots = this.GetComponentsInChildren<HUDSkillSlot>(true);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeSlots();
        }

        private void InitializeSlots()
        {
            _slotMap.Clear();

            for (int i = 0; i < _skillSlots.Length; i++)
            {
                HUDSkillSlot slot = _skillSlots[i];
                if (slot == null)
                {
                    continue;
                }

                slot.Setup(i);
                _slotMap[i] = slot;
            }
        }

        public void RefreshFromCharacterSkill(VCharacterSkill characterSkill)
        {
            if (characterSkill == null)
            {
                ClearAllSlots();
                return;
            }

            for (int i = 0; i < _skillSlots.Length; i++)
            {
                HUDSkillSlot slot = _skillSlots[i];
                if (slot == null)
                {
                    continue;
                }

                slot.Refresh();
            }
        }

        public void UpdateCooldown(int slotIndex, float currentCooldown, float maxCooldown, bool isAvailable)
        {
            if (_slotMap.TryGetValue(slotIndex, out HUDSkillSlot slot))
            {
                slot.UpdateCooldown(currentCooldown, maxCooldown, isAvailable);
            }
        }

        public void UpdateDuration(int slotIndex, float currentDuration, float maxDuration)
        {
            if (_slotMap.TryGetValue(slotIndex, out HUDSkillSlot slot))
            {
                slot.UpdateDuration(currentDuration, maxDuration);
            }
        }

        public HUDSkillSlot GetSlot(int slotIndex)
        {
            return _slotMap.TryGetValue(slotIndex, out HUDSkillSlot slot) ? slot : null;
        }

        private void ClearAllSlots()
        {
            for (int i = 0; i < _skillSlots.Length; i++)
            {
                HUDSkillSlot slot = _skillSlots[i];
                if (slot != null)
                {
                    slot.Refresh();
                }
            }
        }
    }
}

