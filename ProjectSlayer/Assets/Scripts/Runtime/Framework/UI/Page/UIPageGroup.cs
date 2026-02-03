using UnityEngine;

namespace TeamSuneat
{
    public class UIPageGroup : XBehaviour
    {
        private UIPage[] _pages;
        public int PageCount => _pages != null ? _pages.Length : 0;

        private void Awake()
        {
            _pages = this.GetComponentsInDirectChildren<UIPage>();
            CloseAllPages();
        }

        public void ShowPage(int index)
        {
            if (!_pages.IsValid(index))
            {
                Debug.LogWarning($"[UIPageGroup] 유효하지 않은 인덱스입니다: {index}");
                return;
            }

            if (_pages[index] != null)
            {
                _pages[index].Show();
            }
        }

        public void HidePage(int index)
        {
            if (!_pages.IsValid(index))
            {
                Debug.LogWarning($"[UIPageGroup] 유효하지 않은 인덱스입니다: {index}");
                return;
            }

            if (_pages[index] != null)
            {
                _pages[index].Hide();
            }
        }

        public void CloseAllPages()
        {
            for (int i = 0; i < _pages.Length; i++)
            {
                UIPage page = _pages[i];
                if (page != null)
                {
                    page.Hide();
                }
            }
        }

        public UIPage GetPage(int index)
        {
            if (!_pages.IsValid(index))
            {
                return null;
            }

            return _pages[index];
        }
    }
}