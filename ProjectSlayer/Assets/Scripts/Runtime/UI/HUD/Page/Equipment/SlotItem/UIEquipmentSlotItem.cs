using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 장비 슬롯 아이템 - 무기/악세사리 탭 페이지 내 개별 슬롯 표시. Slot Kind로 타입 구분.
    public class UIEquipmentSlotItem : XBehaviour
    {
        [FoldoutGroup("#UIEquipmentSlotItem-Text")]
        [SerializeField] private UILocalizedText _nameText;

        [FoldoutGroup("#UIEquipmentSlotItem-Text")]
        [SerializeField] private UILocalizedText _gradeText;

        [FoldoutGroup("#UIEquipmentSlotItem-Text")]
        [SerializeField] private UILocalizedText _tierText;

        [FoldoutGroup("#UIEquipmentSlotItem-Gauge")]
        [SerializeField] private UIGauge _levelGauge;

        [FoldoutGroup("#UIEquipmentSlotItem-Section")]
        [SerializeField]
        private GameObject _equippedSection;

        [FoldoutGroup("#UIEquipmentSlotItem-Section")]
        [SerializeField]
        private GameObject _newSection;

        [FoldoutGroup("#UIEquipmentSlotItem-Image")]
        [SerializeField] private Image _iconImage;

        [FoldoutGroup("#UIEquipmentSlotItem-Image")]
        [SerializeField] private Image _frameImage;

        [FoldoutGroup("#UIEquipmentSlotItem-Image")]
        [SerializeField] private Image _gradeBackgroundImage;

        private ItemNames _itemName = ItemNames.None;
        private ItemAsset _itemAsset;

        public ItemNames ItemName => _itemName;
        public ItemTypes ItemType => _itemAsset.IsValid() ? _itemAsset.Data.Type : ItemTypes.None;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _nameText ??= this.FindComponent<UILocalizedText>("Equipment Name Text");
            _gradeText ??= this.FindComponent<UILocalizedText>("Equipment Grade Text");
            _tierText ??= this.FindComponent<UILocalizedText>("Equipment Tier Text");
            _levelGauge ??= GetComponentInChildren<UIGauge>();

            _equippedSection = this.FindGameObject("Equipped Section");
            _newSection = this.FindGameObject("New Section");

            _iconImage ??= this.FindComponent<Image>("Equipment Icon Image");
            _frameImage ??= this.FindComponent<Image>("Equipment Frame Image");
            _gradeBackgroundImage ??= this.FindComponent<Image>("Grade Background Image");
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            AutoGetComponents();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
        }

        public void Setup(ItemNames itemName)
        {
            _itemName = itemName;
            _itemAsset = ScriptableDataManager.Instance.FindItem(itemName);
            if (!_itemAsset.IsValid()) { return; }

            SetNameText();
            SetGradeText();
            SetTierText();
            SetSummonLevelGauge();
            SetIconImage();
            SetFrameImage();
            SetGradeBackgroundImage();

            Refresh();
        }

        public void SetNameText()
        {
            if (_nameText != null)
            {
                string content = _itemName.GetLocalizedString();
                _nameText.SetText(content);
            }
        }

        public void SetGradeText()
        {
            if (_gradeText != null)
            {
                string content = _itemAsset.Data.Grade.GetLocalizedString();
                _gradeText.SetText(content);

                Color gradeColor = _itemAsset.Data.Grade.GetGradeColor();
                _gradeText.SetTextColor(gradeColor);
            }
        }

        public void SetTierText()
        {
            if (_tierText != null)
            {
                VProfile profile = GameApp.GetSelectedProfile();
                if (profile != null)
                {
                    if (_itemAsset.Data.Type == ItemTypes.Weapon || _itemAsset.Data.Type == ItemTypes.Accessory)
                    {
                        string content = string.Format(StringDataLabels.FORMAT_GRADE, _itemAsset.Data.Tier);
                        _tierText.SetText(content);
                    }
                    else
                    {
                        _tierText.ResetText();
                    }
                }
            }
        }

        public void SetSummonLevelGauge()
        {
            if (_levelGauge == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            SummonLevelConfigAsset asset = ScriptableDataManager.Instance.GetSummonLevelConfigAsset();
            if (asset == null)
            {
                return;
            }

            int summonLevel = _itemAsset.Data.Type == ItemTypes.Weapon ? profile.Weapon.SummonLevel : profile.Accessory.SummonLevel;
            int requiredSummonCount = asset.GetRequiredSummonCountForLevel(summonLevel);
            int currentSummonCount = summonLevel;
            float fillAmount = currentSummonCount.SafeDivide01(requiredSummonCount);

            _levelGauge.SetFrontValue(Mathf.Clamp01(fillAmount));
            _levelGauge.SetValueText(currentSummonCount, requiredSummonCount);
        }

        public void SetIconImage()
        {
            if (_iconImage != null)
            {
                _iconImage.sprite = _itemName.LoadSprite();
            }
        }

        public void SetFrameImage()
        {
            if (_frameImage != null)
            {
                Color gradeColor = _itemAsset.Data.Grade.GetGradeColor(_frameImage.color.a);
                _frameImage.color = gradeColor;
            }
        }

        public void SetGradeBackgroundImage()
        {
            if (_gradeBackgroundImage != null)
            {
                Color gradeColor = _itemAsset.Data.Grade.GetGradeColor(_gradeBackgroundImage.color.a);
                _gradeBackgroundImage.color = gradeColor;
            }
        }

        public void Refresh()
        {
            RefreshEquippedSection();
        }

        private void RefreshEquippedSection()
        {
            if (_equippedSection == null)
            {
                return;
            }

            if (!_itemAsset.IsValid())
            {
                return;
            }

            if (_itemAsset.Data.Type == ItemTypes.None)
            {
                _equippedSection.SetActive(false);
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                _equippedSection.SetActive(false);
                return;
            }

            bool isEquipped = profile.Weapon.EquippedWeaponName == _itemName || profile.Accessory.EquippedAccessoryName == _itemName;
            _equippedSection.SetActive(isEquipped);
        }
    }
}