using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterSkill
    {
        public Dictionary<string, VSkill> Skills = new(); // 스킬 목록
        public List<VSkillSlot> Slots = new(); // 스킬 슬롯 목록
        public List<string> UnlockedCards = new(); // 해금된 스킬 카드 목록
        public List<string> ObtainedCards = new(); // 획득한 스킬 카드 목록

        [NonSerialized]
        private readonly Dictionary<SkillNames, VSkill> _skillMap = new();

        [NonSerialized]
        private readonly List<SkillNames> _slotSkillNames = new();

        public IReadOnlyList<SkillNames> SlotSkillNames => _slotSkillNames;

        public List<SkillNames> GetSkillNames()
        {
            return new List<SkillNames>(_slotSkillNames);
        }

        public List<VSkill> GetSkills()
        {
            List<VSkill> skills = new();
            foreach (SkillNames skillName in _slotSkillNames)
            {
                if (_skillMap.TryGetValue(skillName, out VSkill skill))
                {
                    skills.Add(skill);
                }
            }

            return skills;
        }

        public void OnLoadGameData()
        {
            _skillMap.Clear();

            SkillNames skillName = SkillNames.None;
            foreach (KeyValuePair<string, VSkill> kvp in Skills)
            {
                VSkill skill = kvp.Value;
                skill.OnLoadGameData();

                if (!EnumEx.ConvertTo(ref skillName, kvp.Key))
                {
                    Log.Error(LogTags.GameData_Skill, "스킬 키를 SkillNames로 변환하지 못했습니다: {0}", kvp.Key);
                    continue;
                }

                skill.Name = skillName;
                _skillMap[skillName] = skill;
            }

            _slotSkillNames.Clear();
            for (int i = 0; i < Slots.Count; i++)
            {
                VSkillSlot slot = Slots[i];
                if (slot.IsUnlocked && !string.IsNullOrEmpty(slot.SkillNameString))
                {
                    if (!EnumEx.ConvertTo(ref skillName, slot.SkillNameString))
                    {
                        Log.Error(LogTags.GameData_Skill, "스킬 슬롯 이름을 SkillNames로 변환하지 못했습니다: {0}", slot.SkillNameString);
                        continue;
                    }

                    if (_skillMap.ContainsKey(skillName))
                    {
                        _slotSkillNames.Add(skillName);
                    }
                }
            }
        }

        public void ClearIngameData()
        {
            // 스킬 데이터는 영구 데이터이므로 인게임 초기화가 필요 없습니다.
        }

        // 스킬 카드 관련 메서드

        public bool CheckUnlockedCard(SkillNames skillName)
        {
            return UnlockedCards.Contains(skillName.ToString());
        }

        public void UnlockCard(SkillNames skillName)
        {
            string key = skillName.ToString();
            if (!UnlockedCards.Contains(key))
            {
                UnlockedCards.Add(key);
                Log.Info(LogTags.GameData_Skill, "스킬 카드를 해금합니다: {0}", skillName);
            }
        }

        public bool CheckObtainedCard(SkillNames skillName)
        {
            return ObtainedCards.Contains(skillName.ToString());
        }

        public void ObtainCard(SkillNames skillName)
        {
            string key = skillName.ToString();
            if (!ObtainedCards.Contains(key))
            {
                ObtainedCards.Add(key);
                Log.Info(LogTags.GameData_Skill, "스킬 카드를 획득합니다: {0}", skillName);
            }
        }

        // 스킬 학습 관련 메서드

        public bool HasSkill(SkillNames skillName)
        {
            return Skills.ContainsKey(skillName.ToString());
        }

        public VSkill FindSkill(SkillNames skillName)
        {
            if (_skillMap.TryGetValue(skillName, out VSkill skill))
            {
                return skill;
            }

            Log.Warning(LogTags.GameData_Skill, "스킬을 찾을 수 없습니다: {0}", skillName.ToLogString());
            return null;
        }

        public void LearnSkill(SkillNames skillName)
        {
            string key = skillName.ToString();
            if (!_skillMap.ContainsKey(skillName))
            {
                VSkill newSkill = new VSkill(skillName);
                Skills[key] = newSkill;
                _skillMap[skillName] = newSkill;

                Log.Info(LogTags.GameData_Skill, "스킬을 학습합니다: {0}", skillName.ToLogString());
            }
        }

        // 스킬 슬롯 관련 메서드

        public void UnlockSlot(int slotIndex)
        {
            if (Slots.IsValid(slotIndex))
            {
                Slots[slotIndex].IsUnlocked = true;
                Log.Info(LogTags.GameData_Skill, "스킬 슬롯을 해금합니다: {0}", slotIndex);
            }
        }

        public bool CheckUnlockedSlot(int slotIndex)
        {
            if (Slots.IsValid(slotIndex))
            {
                return Slots[slotIndex].IsUnlocked;
            }
            return false;
        }

        public void EquipSkill(int slotIndex, SkillNames skillName)
        {
            if (!Slots.IsValid(slotIndex))
            {
                Log.Warning(LogTags.GameData_Skill, "유효하지 않은 스킬 슬롯 인덱스입니다: {0}", slotIndex);
                return;
            }

            if (!_skillMap.ContainsKey(skillName))
            {
                Log.Warning(LogTags.GameData_Skill, "학습하지 않은 스킬입니다: {0}", skillName.ToLogString());
                return;
            }

            VSkillSlot slot = Slots[slotIndex];
            if (!slot.IsUnlocked)
            {
                Log.Warning(LogTags.GameData_Skill, "해금되지 않은 스킬 슬롯입니다: {0}", slotIndex);
                return;
            }

            // 기존 슬롯에서 해당 스킬 제거
            for (int i = 0; i < Slots.Count; i++)
            {
                if (i != slotIndex && Slots[i].SkillNameString == skillName.ToString())
                {
                    Slots[i].SkillNameString = string.Empty;
                    _slotSkillNames.Remove(skillName);
                }
            }

            slot.SkillNameString = skillName.ToString();
            SyncSlotSkillNameStrings();
            Log.Info(LogTags.GameData_Skill, "스킬을 슬롯에 장착합니다: 슬롯 {0}, 스킬 {1}", slotIndex, skillName.ToLogString());
        }

        public void UnequipSkill(int slotIndex)
        {
            if (!Slots.IsValid(slotIndex))
            {
                Log.Warning(LogTags.GameData_Skill, "유효하지 않은 스킬 슬롯 인덱스입니다: {0}", slotIndex);
                return;
            }

            VSkillSlot slot = Slots[slotIndex];
            if (!string.IsNullOrEmpty(slot.SkillNameString))
            {
                SkillNames skillName = EnumEx.ConvertTo<SkillNames>(slot.SkillNameString);
                if (skillName != SkillNames.None)
                {
                    _slotSkillNames.Remove(skillName);
                }
                slot.SkillNameString = string.Empty;
                SyncSlotSkillNameStrings();
                Log.Info(LogTags.GameData_Skill, "스킬을 슬롯에서 해제합니다: 슬롯 {0}", slotIndex);
            }
        }

        public SkillNames GetEquippedSkill(int slotIndex)
        {
            if (!Slots.IsValid(slotIndex))
            {
                return SkillNames.None;
            }

            VSkillSlot slot = Slots[slotIndex];
            if (string.IsNullOrEmpty(slot.SkillNameString))
            {
                return SkillNames.None;
            }

            SkillNames skillName = EnumEx.ConvertTo<SkillNames>(slot.SkillNameString);
            if (skillName != SkillNames.None)
            {
                return skillName;
            }

            return SkillNames.None;
        }

        public static VCharacterSkill CreateDefault()
        {
            VCharacterSkill defaultSkill = new VCharacterSkill();

            // 초기 슬롯 10개 생성 (0번만 해금)
            for (int i = 0; i < 10; i++)
            {
                defaultSkill.Slots.Add(new VSkillSlot
                {
                    SlotID = i,
                    IsUnlocked = i == 0,
                    SkillNameString = string.Empty
                });
            }

            return defaultSkill;
        }

        private void SyncSlotSkillNameStrings()
        {
            // _slotSkillNames를 슬롯의 SkillNameString과 동기화
            _slotSkillNames.Clear();
            SkillNames skillName = SkillNames.None;
            for (int i = 0; i < Slots.Count; i++)
            {
                VSkillSlot slot = Slots[i];
                if (slot.IsUnlocked && !string.IsNullOrEmpty(slot.SkillNameString))
                {
                    if (EnumEx.ConvertTo(ref skillName, slot.SkillNameString))
                    {
                        if (!_slotSkillNames.Contains(skillName))
                        {
                            _slotSkillNames.Add(skillName);
                        }
                    }
                }
            }
        }
    }
}