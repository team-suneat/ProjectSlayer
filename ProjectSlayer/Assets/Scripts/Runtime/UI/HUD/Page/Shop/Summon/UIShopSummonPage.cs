using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 샵 소환 탭 페이지 - 장비/유물 내부 탭 전환 관리
    public class UIShopSummonPage : UIPage
    {
        private const int INNER_TAB_INDEX_EQUIPMENT = 0;
        private const int INNER_TAB_INDEX_RELIC = 1;

        [Title("#UIShopSummonPage")]
        [SerializeField] private UITogglePageController _togglePageController;

        private UIToggleGroup _toggleGroup;
        private int _currentInnerTabIndex = -1;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _togglePageController ??= GetComponentInChildren<UITogglePageController>();
            _toggleGroup ??= _togglePageController != null ? _togglePageController.GetComponentInChildren<UIToggleGroup>() : null;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_togglePageController != null && _currentInnerTabIndex < 0)
            {
                _togglePageController.OpenPage(INNER_TAB_INDEX_EQUIPMENT);
            }
        }

        protected override void OnShow()
        {
            base.OnShow();

            RegisterInnerTabEvents();
            int activeIndex = _togglePageController != null ? _togglePageController.GetCurrentPageIndex() : -1;
            if (activeIndex >= 0)
            {
                _currentInnerTabIndex = activeIndex;
            }
            else if (_currentInnerTabIndex < 0)
            {
                _togglePageController?.OpenPage(INNER_TAB_INDEX_EQUIPMENT);
            }

            Refresh();
        }

        protected override void OnHide()
        {
            base.OnHide();

            UnregisterInnerTabEvents();
        }

        private void RegisterInnerTabEvents()
        {
            if (_toggleGroup != null)
            {
                _toggleGroup.OnToggleChanged.AddListener(OnInnerTabChanged);
            }
        }

        private void UnregisterInnerTabEvents()
        {
            if (_toggleGroup != null)
            {
                _toggleGroup.OnToggleChanged.RemoveListener(OnInnerTabChanged);
            }
        }

        private void OnInnerTabChanged(int innerTabIndex)
        {
            _currentInnerTabIndex = innerTabIndex;
        }

        public void Refresh()
        {
            if (_togglePageController == null)
            {
                return;
            }

            int index = _currentInnerTabIndex >= 0 ? _currentInnerTabIndex : _togglePageController.GetCurrentPageIndex();
            UIPage currentPage = _togglePageController.GetPage(index);
            if (currentPage is UIShopSummonEquipmentPage equipmentPage)
            {
                equipmentPage.Refresh();
            }
            else if (currentPage is UIShopSummonRelicPage relicPage)
            {
                relicPage.Refresh();
            }
        }

        public void OpenEquipmentTab()
        {
            _togglePageController?.OpenPage(INNER_TAB_INDEX_EQUIPMENT);
        }

        public void OpenRelicTab()
        {
            _togglePageController?.OpenPage(INNER_TAB_INDEX_RELIC);
        }

        public int GetCurrentInnerTabIndex()
        {
            return _currentInnerTabIndex;
        }
    }
}