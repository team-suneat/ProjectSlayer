using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 무기 슬롯 아이템 - 무기 탭 페이지 내 개별 무기 슬롯 표시
    public class UIEquipmentWeaponSlotItem : XBehaviour
    {
        [FoldoutGroup("#UIEquipmentWeaponSlotItem-Text")]
        [SerializeField] private UILocalizedText _weaponNameText;

        [FoldoutGroup("#UIEquipmentWeaponSlotItem-Text")]
        [SerializeField] private UILocalizedText _weaponLevelText;

        [FoldoutGroup("#UIEquipmentWeaponSlotItem-Gauge")]
        [SerializeField] private UIGauge _levelGauge;

        [FoldoutGroup("#UIEquipmentWeaponSlotItem-Image")]
        [SerializeField] private Image _weaponIconImage;

        [FoldoutGroup("#UIEquipmentWeaponSlotItem-Image")]
        [SerializeField] private Image _gradeBackgroundImage;

        private ItemNames _weaponName = ItemNames.None;

        public ItemNames WeaponName => _weaponName;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _weaponNameText ??= this.FindComponent<UILocalizedText>("Weapon Name Text");
            _weaponLevelText ??= this.FindComponent<UILocalizedText>("Weapon Level Text");
            _levelGauge ??= this.FindComponent<UIGauge>("Level Gauge");
            _weaponIconImage ??= this.FindComponent<Image>("Weapon Icon Image");
            _gradeBackgroundImage ??= this.FindComponent<Image>("Grade Background Image");
        }

        public void Setup(ItemNames weaponName)
        {
            _weaponName = weaponName;

            SetWeaponNameText(weaponName);
            SetWeaponLevelText(weaponName);
            SetLevelGauge(weaponName);
            SetWeaponIconImage(weaponName.LoadSprite());
            SetGradeBackgroundImage(weaponName.LoadSprite());

            Refresh();
        }

        public void SetWeaponNameText(ItemNames weaponName)
        {
            if (_weaponNameText != null)
            {
                string content = weaponName.GetLocalizedString();
                _weaponNameText.SetText(content);
            }
        }

        public void SetWeaponLevelText(ItemNames weaponName)
        {
            if (_weaponLevelText != null)
            {
                VProfile profile = GameApp.GetSelectedProfile();
                if (profile != null)
                {
                    _weaponLevelText.SetText(profile.Weapon.SummonLevel.GetGradeString());
                }
            }
        }

        public void SetLevelGauge(ItemNames weaponName)
        {
            if (_levelGauge != null)
            {
                VProfile profile = GameApp.GetSelectedProfile();
                if (profile != null)
                {
                    SummonLevelConfigAsset asset = ScriptableDataManager.Instance.GetSummonLevelConfigAsset();
                    if (asset != null)
                    {
                        int requiredSummonCount = asset.GetRequiredSummonCountForLevel(profile.Weapon.SummonLevel);
                        int currentSummonCount = profile.Weapon.SummonLevel;
                        float fillAmount = currentSummonCount.SafeDivide01(requiredSummonCount);

                        _levelGauge.SetFrontValue(Mathf.Clamp01(fillAmount));
                        _levelGauge.SetValueText(currentSummonCount, requiredSummonCount);
                    }
                }
            }
        }

        public void SetWeaponIconImage(Sprite sprite)
        {
            if (_weaponIconImage != null)
            {
                _weaponIconImage.sprite = sprite;
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