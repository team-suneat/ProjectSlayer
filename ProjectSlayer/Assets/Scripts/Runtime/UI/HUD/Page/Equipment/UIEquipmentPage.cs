using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 장비 페이지 - 무기/악세사리/유물/정령 탭 전환 관리
    public class UIEquipmentPage : UIPage
    {
        private const int TAB_INDEX_WEAPON = 0;
        private const int TAB_INDEX_ACCESSORY = 1;
        private const int TAB_INDEX_RELIC = 2;
        private const int TAB_INDEX_SPIRIT = 3;

        [Title("#UIEquipmentPage")]
        [SerializeField] private UITogglePageController _togglePageController;

        private UIToggleGroup _toggleGroup;
        private int _currentTabIndex = -1;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _togglePageController ??= GetComponentInChildren<UITogglePageController>();
            _toggleGroup ??= _togglePageController != null ? _togglePageController.GetComponentInChildren<UIToggleGroup>() : null;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_togglePageController != null && _currentTabIndex < 0)
            {
                _togglePageController.OpenPage(TAB_INDEX_WEAPON);
            }
        }

        protected override void OnShow()
        {
            base.OnShow();

            RegisterEvents();
        }

        protected override void OnHide()
        {
            base.OnHide();

            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            if (_toggleGroup != null)
            {
                _toggleGroup.OnToggleChanged.AddListener(OnTabChanged);
            }
        }

        private void UnregisterEvents()
        {
            if (_toggleGroup != null)
            {
                _toggleGroup.OnToggleChanged.RemoveListener(OnTabChanged);
            }
        }

        private void OnTabChanged(int tabIndex)
        {
            _currentTabIndex = tabIndex;
            RefreshCurrentPage(tabIndex);
        }

        private void RefreshCurrentPage(int tabIndex)
        {
            if (_togglePageController == null)
            {
                return;
            }

            UIPage currentPage = _togglePageController.GetPage(tabIndex);
            if (currentPage == null)
            {
                return;
            }

            if (tabIndex == TAB_INDEX_WEAPON && currentPage is UIEquipmentWeaponPage weaponPage)
            {
                weaponPage.Refresh();
            }
            else if (tabIndex == TAB_INDEX_ACCESSORY && currentPage is UIEquipmentAccessoryPage accessoryPage)
            {
                accessoryPage.Refresh();
            }
            else if (tabIndex == TAB_INDEX_RELIC && currentPage is UIEquipmentRelicPage relicPage)
            {
                relicPage.Refresh();
            }
            else if (tabIndex == TAB_INDEX_SPIRIT && currentPage is UIEquipmentSpiritPage spiritPage)
            {
                spiritPage.Refresh();
            }
        }

        public void OpenWeaponTab()
        {
            _togglePageController?.OpenPage(TAB_INDEX_WEAPON);
        }

        public void OpenAccessoryTab()
        {
            _togglePageController?.OpenPage(TAB_INDEX_ACCESSORY);
        }

        public void OpenRelicTab()
        {
            _togglePageController?.OpenPage(TAB_INDEX_RELIC);
        }

        public void OpenSpiritTab()
        {
            _togglePageController?.OpenPage(TAB_INDEX_SPIRIT);
        }

        public int GetCurrentTabIndex()
        {
            return _currentTabIndex;
        }
    }
}
