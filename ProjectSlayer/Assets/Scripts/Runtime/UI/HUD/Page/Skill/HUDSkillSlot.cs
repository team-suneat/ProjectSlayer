using Sirenix.OdinInspector;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 인게임 스킬 슬롯 UI - 스킬 아이콘, 쿨타임, 사용 가능 여부 표시
    public class HUDSkillSlot : XBehaviour
    {
        [Title("#HUDSkillSlot")]
        [SerializeField] private int _slotIndex;

        [SerializeField] private Image _skillIconImage;
        [SerializeField] private Image _cooldownOverlayImage;
        [SerializeField] private Image _disabledOverlayImage;
        [SerializeField] private UIGauge _skillGauge;
        [SerializeField] private TextMeshProUGUI _cooldownText;
        [SerializeField] private Toggle _autoUseToggle;
        [SerializeField] private Button _skillButton;
        [SerializeField] private GameObject _passiveIndicator;

        [Title("#Gauge Colors")]
        [SerializeField] private Color _timeBasedColor = new Color(0.2f, 0.6f, 1f, 1f); // 파란색
        [SerializeField] private Color _attackCountBasedColor = new Color(1f, 0.5f, 0f, 1f); // 주황색

        private SkillNames _currentSkillName = SkillNames.None;
        private VSkill _currentSkill;
        private SkillAsset _skillAsset;

        public int SlotIndex => _slotIndex;
        public SkillNames CurrentSkillName => _currentSkillName;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _skillIconImage ??= this.FindComponent<Image>("Skill Icon Image");
            _cooldownOverlayImage ??= this.FindComponent<Image>("Cooldown Overlay Image");
            _disabledOverlayImage ??= this.FindComponent<Image>("Disabled Overlay Image");
            _skillGauge ??= this.FindComponent<UIGauge>("Skill Gauge");
            _cooldownText ??= this.FindComponent<TextMeshProUGUI>("Cooldown Text");
            _autoUseToggle ??= this.FindComponent<Toggle>("Auto Use Toggle");
            _skillButton ??= this.FindComponent<Button>("Skill Button");
            _passiveIndicator ??= this.FindGameObject("Passive Indicator");
        }

        public override void AutoNaming()
        {
            SetGameObjectName($"HUDSkillSlot({_slotIndex})");
        }

        private void Awake()
        {
            if (_skillButton != null)
            {
                _skillButton.onClick.AddListener(OnSkillButtonClicked);
            }

            if (_autoUseToggle != null)
            {
                _autoUseToggle.onValueChanged.AddListener(OnAutoUseToggleChanged);
            }
        }

        public void Setup(int slotIndex)
        {
            _slotIndex = slotIndex;
            Refresh();
        }

        public void Refresh()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                ClearSlot();
                return;
            }

            VCharacterSkill characterSkill = profile.Skill;
            if (!TryGetValidSlot(characterSkill, out VSkillSlot slot))
            {
                ClearSlot();
                return;
            }

            if (!TryParseSkillName(slot.SkillNameString, out SkillNames skillName))
            {
                ClearSlot();
                return;
            }

            LoadSkillData(characterSkill, skillName);
            RefreshSkillIcon();
            RefreshAutoUseToggle();
            RefreshPassiveIndicator();
        }

        private bool TryGetValidSlot(VCharacterSkill characterSkill, out VSkillSlot slot)
        {
            slot = null;

            if (characterSkill == null || !characterSkill.Slots.IsValid(_slotIndex))
            {
                return false;
            }

            slot = characterSkill.Slots[_slotIndex];
            return slot.IsUnlocked && !string.IsNullOrEmpty(slot.SkillNameString);
        }

        private bool TryParseSkillName(string skillNameString, out SkillNames skillName)
        {
            skillName = SkillNames.None;

            if (!EnumEx.ConvertTo(ref skillName, skillNameString))
            {
                Log.Warning(LogTags.UI_Page, "스킬 슬롯 이름을 SkillNames로 변환하지 못했습니다: {0}", skillNameString);
                return false;
            }

            return true;
        }

        private void LoadSkillData(VCharacterSkill characterSkill, SkillNames skillName)
        {
            _currentSkillName = skillName;
            _currentSkill = characterSkill.FindSkill(skillName);
            _skillAsset = ScriptableDataManager.Instance?.FindSkill(skillName);
        }

        private void RefreshSkillIcon()
        {
            if (_skillIconImage == null)
            {
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

        private void RefreshAutoUseToggle()
        {
            if (_autoUseToggle == null)
            {
                return;
            }

            if (_currentSkill != null)
            {
                _autoUseToggle.isOn = _currentSkill.IsAutoUse;
                _autoUseToggle.gameObject.SetActive(true);
            }
            else
            {
                _autoUseToggle.gameObject.SetActive(false);
            }
        }

        private void RefreshPassiveIndicator()
        {
            if (_passiveIndicator == null)
            {
                return;
            }

            bool isPassive = _skillAsset?.Data?.Type == SkillTypes.Passive;
            _passiveIndicator.SetActive(isPassive);
        }

        public void UpdateCooldown(float currentCooldown, float maxCooldown, bool isAvailable)
        {
            if (!IsSkillValid())
            {
                HideCooldown();
                return;
            }

            bool isPassive = IsPassiveSkill();
            if (isPassive)
            {
                HideCooldown();
                UpdateAvailability(isAvailable);
                return;
            }

            UpdateCooldownOverlay(currentCooldown, maxCooldown);
            UpdateCooldownGauge(currentCooldown, maxCooldown);
            UpdateCooldownText(currentCooldown);
            UpdateAvailability(isAvailable);
        }

        public void UpdateDuration(float currentDuration, float maxDuration)
        {
            if (!IsSkillValid() || _skillGauge == null)
            {
                if (_skillGauge != null)
                {
                    _skillGauge.ResetFrontValue();
                }
                return;
            }

            if (maxDuration > 0f && currentDuration > 0f)
            {
                float fillAmount = currentDuration / maxDuration;
                _skillGauge.SetFrontValue(fillAmount);
            }
            else
            {
                _skillGauge.ResetFrontValue();
            }
        }

        private bool IsSkillValid()
        {
            return _currentSkillName != SkillNames.None && _skillAsset != null;
        }

        private bool IsPassiveSkill()
        {
            return _skillAsset?.Data?.Type == SkillTypes.Passive;
        }

        private void UpdateCooldownOverlay(float currentCooldown, float maxCooldown)
        {
            if (_cooldownOverlayImage == null)
            {
                return;
            }

            if (maxCooldown > 0f && currentCooldown > 0f)
            {
                float fillAmount = currentCooldown / maxCooldown;
                _cooldownOverlayImage.fillAmount = fillAmount;
                _cooldownOverlayImage.gameObject.SetActive(true);
            }
            else
            {
                _cooldownOverlayImage.gameObject.SetActive(false);
            }
        }

        private void UpdateCooldownGauge(float currentCooldown, float maxCooldown)
        {
            if (_skillGauge == null || _skillAsset?.Data == null)
            {
                return;
            }

            if (maxCooldown <= 0f || currentCooldown <= 0f)
            {
                _skillGauge.ResetFrontValue();
                return;
            }

            CooldownTypes cooldownType = _skillAsset.Data.CooldownType;
            Color gaugeColor = cooldownType == CooldownTypes.TimeBased ? _timeBasedColor : _attackCountBasedColor;

            _skillGauge.SetFrontColor(gaugeColor);
            float fillAmount = 1f - (currentCooldown / maxCooldown);
            _skillGauge.SetFrontValue(fillAmount);
        }

        private void UpdateCooldownText(float currentCooldown)
        {
            if (_cooldownText == null || _skillAsset?.Data == null)
            {
                return;
            }

            if (currentCooldown <= 0f)
            {
                _cooldownText.gameObject.SetActive(false);
                return;
            }

            CooldownTypes cooldownType = _skillAsset.Data.CooldownType;
            if (cooldownType == CooldownTypes.TimeBased)
            {
                _cooldownText.text = "0.0";
                _cooldownText.gameObject.SetActive(true);
            }
            else if (cooldownType == CooldownTypes.AttackCountBased)
            {
                _cooldownText.text = Mathf.CeilToInt(currentCooldown).ToString();
                _cooldownText.gameObject.SetActive(true);
            }
            else
            {
                _cooldownText.gameObject.SetActive(false);
            }
        }

        private void UpdateAvailability(bool isAvailable)
        {
            if (_disabledOverlayImage != null)
            {
                _disabledOverlayImage.gameObject.SetActive(!isAvailable);
            }

            if (_skillButton != null)
            {
                _skillButton.interactable = isAvailable;
            }
        }

        private void HideCooldown()
        {
            if (_cooldownOverlayImage != null)
            {
                _cooldownOverlayImage.gameObject.SetActive(false);
            }

            if (_skillGauge != null)
            {
                _skillGauge.ResetFrontValue();
            }

            if (_cooldownText != null)
            {
                _cooldownText.gameObject.SetActive(false);
            }

            if (_disabledOverlayImage != null)
            {
                _disabledOverlayImage.gameObject.SetActive(false);
            }
        }

        private void ClearSlot()
        {
            _currentSkillName = SkillNames.None;
            _currentSkill = null;
            _skillAsset = null;

            if (_skillIconImage != null)
            {
                _skillIconImage.enabled = false;
            }

            if (_autoUseToggle != null)
            {
                _autoUseToggle.gameObject.SetActive(false);
            }

            if (_passiveIndicator != null)
            {
                _passiveIndicator.SetActive(false);
            }

            if (_skillGauge != null)
            {
                _skillGauge.ResetFrontValue();
            }

            HideCooldown();
        }

        private void OnSkillButtonClicked()
        {
            if (_currentSkillName == SkillNames.None)
            {
                return;
            }

            // TODO: 스킬 사용 로직 구현
            Log.Info(LogTags.UI_Page, "스킬 사용 버튼 클릭: {0}", _currentSkillName);
        }

        private void OnAutoUseToggleChanged(bool isOn)
        {
            if (_currentSkill == null)
            {
                return;
            }

            _currentSkill.IsAutoUse = isOn;

            // TODO: 세이브 데이터 저장 로직 호출
            Log.Info(LogTags.UI_Page, "스킬 자동 사용 설정 변경: {0}, {1}", _currentSkillName, isOn);
        }
    }
}