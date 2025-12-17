using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    // HUD 재화 표시 컴포넌트 - 재화 아이콘 및 소지량 표시
    public class HUDCurrency : XBehaviour
    {
        [Title("#HUDCurrency")]
        [SerializeField] private CurrencyNames _currencyName;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _valueText;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _iconImage = GetComponentInChildren<Image>();
            _valueText = GetComponentInChildren<TextMeshProUGUI>();
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        private void OnPlayerCharacterBattleReady()
        {
            RefreshIcon();
            RefreshValue();
        }

        private void OnCurrencyChanged(CurrencyNames currencyName, int amount)
        {
            if (_currencyName != currencyName)
            {
                return;
            }

            SetValue(amount);
        }

        private void RefreshIcon()
        {
            if (_iconImage == null)
            {
                return;
            }

            if (_currencyName == CurrencyNames.None)
            {
                _iconImage.enabled = false;
                return;
            }

            Sprite iconSprite = SpriteEx.LoadSprite(_currencyName);
            if (iconSprite != null)
            {
                _iconImage.sprite = iconSprite;
                _iconImage.enabled = true;
            }
            else
            {
                _iconImage.enabled = false;
            }
        }

        private void RefreshValue()
        {
            if (_currencyName == CurrencyNames.None)
            {
                SetValue(0);
                return;
            }

            var profile = GameApp.GetSelectedProfile();
            if (profile == null || profile.Currency == null)
            {
                SetValue(0);
                return;
            }

            int amount = profile.Currency.GetAmount(_currencyName);
            SetValue(amount);
        }

        private void SetValue(int amount)
        {
            if (_valueText == null)
            {
                return;
            }

            _valueText.text = amount.ToString("N0");
        }
    }
}