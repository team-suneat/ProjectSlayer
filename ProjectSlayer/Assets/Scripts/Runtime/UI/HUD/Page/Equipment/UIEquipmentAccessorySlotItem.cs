using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 악세사리 슬롯 아이템 - 악세사리 탭 페이지 내 개별 악세사리 슬롯 표시
    public class UIEquipmentAccessorySlotItem : XBehaviour
    {
        [FoldoutGroup("#UIEquipmentAccessorySlotItem-Text")]
        [SerializeField] private UILocalizedText _accessoryNameText;

        [FoldoutGroup("#UIEquipmentAccessorySlotItem-Text")]
        [SerializeField] private UILocalizedText _accessoryLevelText;

        [FoldoutGroup("#UIEquipmentAccessorySlotItem-Gauge")]
        [SerializeField] private UIGauge _levelGauge;

        [FoldoutGroup("#UIEquipmentAccessorySlotItem-Image")]
        [SerializeField] private Image _accessoryIconImage;

        [FoldoutGroup("#UIEquipmentAccessorySlotItem-Image")]
        [SerializeField] private Image _gradeBackgroundImage;

        private ItemNames _accessoryName = ItemNames.None;

        public ItemNames AccessoryName => _accessoryName;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _accessoryNameText ??= this.FindComponent<UILocalizedText>("Accessory Name Text");
            _accessoryLevelText ??= this.FindComponent<UILocalizedText>("Accessory Grade Text");
            _levelGauge ??= GetComponentInChildren<UIGauge>();
            _accessoryIconImage ??= this.FindComponent<Image>("Accessory Icon Image");
            _gradeBackgroundImage ??= this.FindComponent<Image>("Grade Background Image");
        }

        public void Setup(ItemNames accessoryName)
        {
            _accessoryName = accessoryName;

            SetAccessoryNameText(accessoryName);
            SetAccessoryLevelText(accessoryName);
            SetLevelGauge(accessoryName);
            SetAccessoryIconImage(accessoryName.LoadSprite());
            SetGradeBackgroundImage(accessoryName.LoadSprite());

            Refresh();
        }

        public void SetAccessoryNameText(ItemNames accessoryName)
        {
            if (_accessoryNameText != null)
            {
                string content = accessoryName.GetLocalizedString();
                _accessoryNameText.SetText(content);
            }
        }

        public void SetAccessoryLevelText(ItemNames accessoryName)
        {
            if (_accessoryLevelText != null)
            {
                VProfile profile = GameApp.GetSelectedProfile();
                if (profile != null)
                {
                    _accessoryLevelText.SetText(profile.Accessory.SummonLevel.GetGradeString());
                }
            }
        }

        public void SetLevelGauge(ItemNames accessoryName)
        {
            if (_levelGauge != null)
            {
                VProfile profile = GameApp.GetSelectedProfile();
                if (profile != null)
                {
                    SummonLevelConfigAsset asset = ScriptableDataManager.Instance.GetSummonLevelConfigAsset();
                    if (asset != null)
                    {
                        int requiredSummonCount = asset.GetRequiredSummonCountForLevel(profile.Accessory.SummonLevel);
                        int currentSummonCount = profile.Accessory.SummonLevel;
                        float fillAmount = currentSummonCount.SafeDivide01(requiredSummonCount);

                        _levelGauge.SetFrontValue(Mathf.Clamp01(fillAmount));
                        _levelGauge.SetValueText(currentSummonCount, requiredSummonCount);
                    }
                }
            }
        }

        public void SetAccessoryIconImage(Sprite sprite)
        {
            if (_accessoryIconImage != null)
            {
                _accessoryIconImage.sprite = sprite;
            }
        }

        public void SetGradeBackgroundImage(Sprite sprite)
        {
            if (_gradeBackgroundImage != null)
            {
                _gradeBackgroundImage.sprite = sprite;
            }
        }

        public void Refresh()
        {
        }
    }
}