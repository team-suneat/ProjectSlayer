using Sirenix.OdinInspector;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 스킬 슬롯 아이템 - 스킬 페이지 내 개별 스킬 카드 표시
    public class UISkillSlotItem : XBehaviour
    {
        [Title("#UISkillSlotItem")]
        [SerializeField] private Image _skillIconImage;
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillCountText;
        [SerializeField] private UIGauge _skillCountGauge;
        [SerializeField] private Image _grayOverlayImage;
        [SerializeField] private TextMeshProUGUI _equippedText;
        [SerializeField] private GameObject _unknownImage;

        private SkillNames _skillName = SkillNames.None;
        private SkillAsset _skillAsset;

        private enum SkillSlotState
        {
            Unknown,        // 배우지 않음
            Learned,        // 배우고 할당 안 함
            Equipped        // 배우고 할당함
        }

        public SkillNames SkillName => _skillName;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _skillIconImage ??= this.FindComponent<Image>("Skill Icon Image");
            _skillNameText ??= this.FindComponent<TextMeshProUGUI>("Skill Name Text");
            _skillCountText ??= this.FindComponent<TextMeshProUGUI>("Skill Count Text");
            _skillCountGauge ??= this.FindComponent<UIGauge>("Skill Count Gauge");
            _grayOverlayImage ??= this.FindComponent<Image>("Gray Overlay Image");
            _equippedText ??= this.FindComponent<TextMeshProUGUI>("Equipped Text");
            _unknownImage ??= this.FindGameObject("Unknown Image");
        }

        public override void AutoNaming()
        {
            if (_skillName != SkillNames.None)
            {
                SetGameObjectName($"UISkillSlotItem({_skillName})");
            }
            else
            {
                SetGameObjectName("UISkillSlotItem");
            }
        }

        public void Setup(SkillNames skillName)
        {
            _skillName = skillName;
            _skillAsset = ScriptableDataManager.Instance?.FindSkill(skillName);

            Refresh();
        }

        public void Refresh()
        {
            SkillSlotState state = GetSkillSlotState();
            RefreshSkillState(state);
            RefreshSkillIcon(state);
            RefreshSkillName(state);
            
            if (state != SkillSlotState.Unknown)
            {
                RefreshSkillCount();
            }
            else
            {
                ClearSkillCount();
            }
        }

        private SkillSlotState GetSkillSlotState()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return SkillSlotState.Unknown;
            }

            VCharacterSkill characterSkill = profile.Skill;
            if (characterSkill == null)
            {
                return SkillSlotState.Unknown;
            }

            bool hasSkill = characterSkill.HasSkill(_skillName);
            if (!hasSkill)
            {
                return SkillSlotState.Unknown;
            }

            bool isEquipped = characterSkill.SlotSkillNames.Contains(_skillName);
            return isEquipped ? SkillSlotState.Equipped : SkillSlotState.Learned;
        }

        private void RefreshSkillState(SkillSlotState state)
        {
            if (_grayOverlayImage != null)
            {
                _grayOverlayImage.gameObject.SetActive(state == SkillSlotState.Equipped);
            }

            if (_equippedText != null)
            {
                if (state == SkillSlotState.Equipped)
                {
                    _equippedText.text = "E";
                    _equippedText.gameObject.SetActive(true);
                }
                else
                {
                    _equippedText.gameObject.SetActive(false);
                }
            }

            if (_unknownImage != null)
            {
                _unknownImage.SetActive(state == SkillSlotState.Unknown);
            }
        }

        private void RefreshSkillIcon(SkillSlotState state)
        {
            if (_skillIconImage == null)
            {
                return;
            }

            if (state == SkillSlotState.Unknown)
            {
                _skillIconImage.enabled = false;
                return;
            }

            Sprite icon = _skillAsset?.Data?.Icon;
            if (icon != null)
            {
                _skillIconImage.sprite = icon;
                _skillIconImage.enabled = true;
            }
            else
            {
                _skillIconImage.enabled = false;
            }
        }

        private void RefreshSkillName(SkillSlotState state)
        {
            if (_skillNameText == null)
            {
                return;
            }

            if (state == SkillSlotState.Unknown)
            {
                _skillNameText.text = string.Empty;
                return;
            }

            if (_skillName != SkillNames.None)
            {
                _skillNameText.text = _skillName.ToString();
            }
            else
            {
                _skillNameText.text = string.Empty;
            }
        }

        private void RefreshSkillCount()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                ClearSkillCount();
                return;
            }

            VCharacterSkill characterSkill = profile.Skill;
            if (characterSkill == null)
            {
                ClearSkillCount();
                return;
            }

            int currentCount = GetCurrentSkillCardCount(characterSkill);
            int requiredCount = GetRequiredSkillCardCount();

            RefreshSkillCountText(currentCount, requiredCount);
            RefreshSkillCountGauge(currentCount, requiredCount);
        }

        private int GetCurrentSkillCardCount(VCharacterSkill characterSkill)
        {
            if (characterSkill.ObtainedCards == null)
            {
                return 0;
            }

            string skillNameString = _skillName.ToString();
            int count = 0;

            for (int i = 0; i < characterSkill.ObtainedCards.Count; i++)
            {
                if (characterSkill.ObtainedCards[i] == skillNameString)
                {
                    count++;
                }
            }

            return count;
        }

        private int GetRequiredSkillCardCount()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return 0;
            }

            VCharacterSkill characterSkill = profile.Skill;
            if (characterSkill == null)
            {
                return 0;
            }

            VSkill skill = characterSkill.FindSkill(_skillName);
            if (skill == null)
            {
                return 0;
            }

            int currentLevel = skill.Level;
            if (_skillAsset?.Data?.LevelUpCosts == null)
            {
                return 0;
            }

            if (currentLevel >= _skillAsset.Data.MaxLevel)
            {
                return 0;
            }

            int nextLevel = currentLevel + 1;
            if (nextLevel > 0 && nextLevel <= _skillAsset.Data.LevelUpCosts.Length)
            {
                return _skillAsset.Data.LevelUpCosts[nextLevel - 1];
            }

            return 0;
        }

        private void RefreshSkillCountText(int currentCount, int requiredCount)
        {
            if (_skillCountText == null)
            {
                return;
            }

            if (requiredCount <= 0)
            {
                _skillCountText.text = string.Empty;
                return;
            }

            _skillCountText.text = $"{currentCount} / {requiredCount}";
        }

        private void RefreshSkillCountGauge(int currentCount, int requiredCount)
        {
            if (_skillCountGauge == null)
            {
                return;
            }

            if (requiredCount <= 0)
            {
                _skillCountGauge.ResetFrontValue();
                return;
            }

            float fillAmount = Mathf.Clamp01((float)currentCount / requiredCount);
            _skillCountGauge.SetFrontValue(fillAmount);
        }

        private void ClearSkillCount()
        {
            if (_skillCountText != null)
            {
                _skillCountText.text = string.Empty;
            }

            if (_skillCountGauge != null)
            {
                _skillCountGauge.ResetFrontValue();
            }
        }
    }
}

