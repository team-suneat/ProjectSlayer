using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 성장 페이지 - 능력치 포인트로 구매하는 성장 능력치 관리
    public class UIGrowthPage : UIPage
    {
        [Title("#UIGrowthPage")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Transform _contentParent;
        [SerializeField] private UIGrowthItem _itemPrefab;
        [SerializeField] private UILocalizedText _statPointText;

        private readonly List<UIGrowthItem> _items = new();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _scrollRect ??= GetComponentInChildren<ScrollRect>();
            if (_scrollRect != null)
            {
                _contentParent ??= _scrollRect.content;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_itemPrefab == null)
            {
                Log.Warning(LogTags.UI_Page, "UIGrowthItem 프리팹이 설정되지 않았습니다.");
                return;
            }

            if (_contentParent == null)
            {
                Log.Warning(LogTags.UI_Page, "Content Parent가 설정되지 않았습니다.");
                return;
            }

            CreateGrowthItems();
            RefreshAllItems();
        }

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        private void CreateGrowthItems()
        {
            // 기존 아이템 제거
            ClearItems();

            // 성장 데이터 에셋 가져오기
            GrowthDataAsset asset = ScriptableDataManager.Instance?.GetGrowthDataAsset();
            if (asset == null || asset.DataArray == null)
            {
                Log.Warning(LogTags.UI_Page, "성장 데이터 에셋을 찾을 수 없습니다.");
                return;
            }

            // 각 성장 타입에 대해 아이템 생성
            for (int i = 0; i < asset.DataArray.Length; i++)
            {
                GrowthData data = asset.DataArray[i];
                if (data == null || data.StatName == StatNames.None)
                {
                    continue;
                }

                UIGrowthItem item = CreateItem(data);
                if (item != null)
                {
                    _items.Add(item);
                }
            }

            Log.Info(LogTags.UI_Page, "성장 아이템 {0}개 생성 완료", _items.Count);
        }

        private UIGrowthItem CreateItem(GrowthData data)
        {
            if (_itemPrefab == null || _contentParent == null)
            {
                return null;
            }

            UIGrowthItem item = Instantiate(_itemPrefab, _contentParent);
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

        public void Refresh()
        {
            RefreshStatPoint();
            RefreshAllItems();
        }

        private void RefreshStatPoint()
        {
            if (_statPointText == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            StringData stringData = JsonDataManager.FindStringData("Format_StatPoint");
            int statPoint = profile.Growth.StatPoint;
            string content = StringGetter.Format(stringData, statPoint.ToString());
            _statPointText.SetText(content);
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
            // 능력치 포인트 표시 갱신
            RefreshStatPoint();
            // 다른 아이템의 버튼 상태도 갱신
            RefreshAllItems();
        }

        public UIGrowthItem FindItem(StatNames statName)
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