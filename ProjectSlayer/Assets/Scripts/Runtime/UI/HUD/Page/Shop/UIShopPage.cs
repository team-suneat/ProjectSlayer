using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 샵 페이지 - 소환 등 탭 전환 관리
    public class UIShopPage : UIPage
    {
        private const int TAB_INDEX_SUMMON = 0;

        [Title("#UIShopPage")]
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
                _togglePageController.OpenPage(TAB_INDEX_SUMMON);
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

            if (tabIndex == TAB_INDEX_SUMMON && currentPage is UIShopSummonPage summonPage)
            {
                summonPage.Refresh();
            }
        }

        public void OpenSummonTab()
        {
            _togglePageController?.OpenPage(TAB_INDEX_SUMMON);
        }

        public int GetCurrentTabIndex()
        {
            return _currentTabIndex;
        }
    }
}