using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 샵 소환 장비 탭 페이지 - 무기/악세사리/스킬 카드 소환 섹션 관리
    public class UIShopSummonEquipmentPage : UIPage
    {
        [Title("#UIShopSummonEquipmentPage")]
        [SerializeField] private UIShopSummonCategorySection[] _categorySections;

        private readonly Dictionary<ShopSummonCategory, UIShopSummonCategorySection> _categoryToSectionMap = new();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _categorySections = GetComponentsInChildren<UIShopSummonCategorySection>(true);
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        private void OnCurrencyChanged(CurrencyNames currencyName, int amount)
        {
            if (currencyName == CurrencyNames.Diamond)
            {
                RefreshAllSections();
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_categorySections == null || _categorySections.Length == 0)
            {
                Log.Warning(LogTags.UI_Page, "UIShopSummonCategorySection을 찾을 수 없습니다.");
                return;
            }

            SetupCategorySections();
            RefreshAllSections();
        }

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        private void SetupCategorySections()
        {
            _categoryToSectionMap.Clear();

            ShopSummonCategory[] categories = { ShopSummonCategory.Weapon, ShopSummonCategory.Accessory, ShopSummonCategory.SkillCard };
            int sectionCount = Mathf.Min(_categorySections.Length, categories.Length);

            for (int i = 0; i < sectionCount; i++)
            {
                if (_categorySections[i] != null)
                {
                    _categorySections[i].Setup(categories[i]);
                    _categoryToSectionMap.Add(categories[i], _categorySections[i]);
                }
            }
        }

        public void Refresh()
        {
            RefreshAllSections();
        }

        private void RefreshAllSections()
        {
            if (_categorySections == null)
            {
                return;
            }

            for (int i = 0; i < _categorySections.Length; i++)
            {
                _categorySections[i]?.Refresh();
            }
        }

        public UIShopSummonCategorySection FindSection(ShopSummonCategory category)
        {
            return _categoryToSectionMap.TryGetValue(category, out UIShopSummonCategorySection section) ? section : null;
        }
    }
}
