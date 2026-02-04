using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using TeamSuneat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 장비 아이템 팝업 - 강화 탭 페이지 (아이템 정보, 레벨 게이지, 강화/장착 버튼)
    public class UIEquipmentEnhancePage : UIPage
    {
        private const CurrencyNames ENHANCE_CURRENCY = CurrencyNames.Emerald;

        [FoldoutGroup("#UIEquipmentEnhancePage/Currency")]
        [SerializeField] private Image _currencyIconImage;

        [FoldoutGroup("#UIEquipmentEnhancePage/Currency")]
        [SerializeField] private TextMeshProUGUI _currencyValueText;

        [FoldoutGroup("#UIEquipmentEnhancePage")]
        [SerializeField] private UIEquipmentSlotItem _slotItem;

        [FoldoutGroup("#UIEquipmentEnhancePage/Button")]
        [SerializeField] private UIButton _enhanceButton;

        [FoldoutGroup("#UIEquipmentEnhancePage/Button")]
        [SerializeField] private UIButton _equipButton;

        private ItemNames _itemName = ItemNames.None;
        private ItemTypes _equipmentType = ItemTypes.None;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _currencyIconImage ??= this.FindComponent<Image>("Top Currency/Currency Icon Image");
            _currencyValueText ??= this.FindComponent<TextMeshProUGUI>("Top Currency/Currency Value Text");

            _slotItem ??= GetComponentInChildren<UIEquipmentSlotItem>(true);

            _enhanceButton ??= this.FindComponent<UIButton>("Bottom Buttons/Enhance Button");
            _equipButton ??= this.FindComponent<UIButton>("Bottom Buttons/Equip Button");
        }

        protected override void Awake()
        {
            base.Awake();
            AutoGetComponents();

            if (_enhanceButton != null)
            {
                _enhanceButton.RegisterClickSuccessEvent(OnClickEnhanceButton);
            }

            if (_equipButton != null)
            {
                _equipButton.RegisterClickSuccessEvent(OnClickEquipButton);
            }
        }

        private void OnEnable()
        {
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        private void OnDisable()
        {
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        protected override void OnShow()
        {
            base.OnShow();
            RefreshAll();
        }

        public void Setup(ItemNames itemName, ItemTypes equipmentType)
        {
            _itemName = itemName;
            _equipmentType = equipmentType;
            RefreshAll();
        }

        public void RefreshAll()
        {
            RefreshCurrency();
            if (_slotItem != null && _itemName != ItemNames.None)
            {
                _slotItem.Setup(_itemName);
            }
        }

        private void RefreshCurrency()
        {
            if (_currencyIconImage != null)
            {
                Sprite iconSprite = ENHANCE_CURRENCY.LoadSprite();
                _currencyIconImage.sprite = iconSprite;
                _currencyIconImage.enabled = iconSprite != null;
            }

            if (_currencyValueText != null)
            {
                VProfile profile = GameApp.GetSelectedProfile();
                int amount = profile?.Currency?.GetAmount(ENHANCE_CURRENCY) ?? 0;
                _currencyValueText.SetText(ValueStringEx.GetValueString(amount));
            }
        }

        private void OnCurrencyChanged(CurrencyNames currencyName, int addAmount)
        {
            if (currencyName != ENHANCE_CURRENCY)
            {
                return;
            }

            RefreshCurrency();
        }

        private void OnClickEnhanceButton()
        {
            // 강화/비용/성공률/스탯 상승 규칙은 2차로 연결합니다.
            Log.Warning(LogTags.UI_Popup, "장비 강화 기능은 아직 연결되지 않았습니다.");
        }

        private void OnClickEquipButton()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            if (_equipmentType == ItemTypes.Weapon)
            {
                profile.Weapon?.EquipWeapon(_itemName);
            }
            else if (_equipmentType == ItemTypes.Accessory)
            {
                profile.Accessory?.EquipAccessory(_itemName);
            }

            UIEquipmentItemPopup popup = GetComponentInParent<UIEquipmentItemPopup>(true);
            if (popup != null)
            {
                _ = CoroutineNextTimer(0.3f, popup.CloseWithSuccess);
            }
        }
    }
}
