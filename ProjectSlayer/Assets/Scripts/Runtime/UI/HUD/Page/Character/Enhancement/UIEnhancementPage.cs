using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    // 강화 페이지 - 강화 아이템 스크롤 뷰 관리
    public class UIEnhancementPage : UIPage
    {
        [Title("#UIEnhancementPage")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Transform _contentParent;
        [SerializeField] private UIEnhancementItem _itemPrefab;

        private readonly List<UIEnhancementItem> _items = new();
        private bool _isInitialized;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _scrollRect ??= GetComponentInChildren<ScrollRect>();
            if (_scrollRect != null)
            {
                _contentParent ??= _scrollRect.content;
            }
        }

        protected override void OnShow()
        {
            base.OnShow();

            if (!_isInitialized)
            {
                Initialize();
            }

            RefreshAllItems();
        }

        private void Initialize()
        {
            AutoGetComponents();

            if (_itemPrefab == null)
            {
                Log.Warning(LogTags.UI_Page, "UIEnhancementItem 프리팹이 설정되지 않았습니다.");
                return;
            }

            if (_contentParent == null)
            {
                Log.Warning(LogTags.UI_Page, "Content Parent가 설정되지 않았습니다.");
                return;
            }

            CreateEnhancementItems();
            _isInitialized = true;
        }

        private void CreateEnhancementItems()
        {
            // 기존 아이템 제거
            ClearItems();

            // 강화 데이터 에셋 가져오기
            EnhancementDataAsset asset = ScriptableDataManager.Instance?.GetEnhancementDataAsset();
            if (asset == null || asset.DataArray == null)
            {
                Log.Warning(LogTags.UI_Page, "강화 데이터 에셋을 찾을 수 없습니다.");
                return;
            }

            // 각 강화 타입에 대해 아이템 생성
            for (int i = 0; i < asset.DataArray.Length; i++)
            {
                EnhancementData data = asset.DataArray[i];
                if (data == null || data.StatName == StatNames.None)
                {
                    continue;
                }

                UIEnhancementItem item = CreateItem(data);
                if (item != null)
                {
                    _items.Add(item);
                }
            }

            Log.Info(LogTags.UI_Page, "강화 아이템 {0}개 생성 완료", _items.Count);
        }

        private UIEnhancementItem CreateItem(EnhancementData data)
        {
            if (_itemPrefab == null || _contentParent == null)
            {
                return null;
            }

            UIEnhancementItem item = Instantiate(_itemPrefab, _contentParent);
            item.gameObject.SetActive(true);
            item.Setup(data);
            item.OnLevelUpSuccess.AddListener(OnItemLevelUpSuccess);

            return item;
        }

        private void ClearItems()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null)
                {
                    _items[i].OnLevelUpSuccess.RemoveListener(OnItemLevelUpSuccess);
                    Destroy(_items[i].gameObject);
                }
            }
            _items.Clear();
        }

        public void RefreshAllItems()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null)
                {
                    _items[i].Refresh();
                }
            }
        }

        private void OnItemLevelUpSuccess()
        {
            // 다른 아이템의 버튼 상태도 갱신 (요구 능력치 조건이 변경될 수 있음)
            RefreshAllItems();
        }

        public UIEnhancementItem FindItem(StatNames statName)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null && _items[i].GetStatName() == statName)
                {
                    return _items[i];
                }
            }
            return null;
        }

        private void OnDestroy()
        {
            ClearItems();
        }
    }
}