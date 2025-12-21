using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 스킬 페이지 - 모든 스킬 카드 목록 표시
    public class UISkillPage : UIPage
    {
        [Title("#UISkillPage")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Transform _contentParent;
        [SerializeField] private UISkillSlotItem _itemPrefab;

        private readonly List<UISkillSlotItem> _items = new();
        private readonly Dictionary<SkillNames, UISkillSlotItem> _skillItemMap = new();

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
                Log.Warning(LogTags.UI_Page, "UISkillSlotItem 프리팹이 설정되지 않았습니다.");
                return;
            }

            if (_contentParent == null)
            {
                Log.Warning(LogTags.UI_Page, "Content Parent가 설정되지 않았습니다.");
                return;
            }

            CreateSkillItems();
            RefreshAllItems();
        }

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        private void CreateSkillItems()
        {
            ClearItems();

            SkillNames[] skillNames = EnumEx.GetValues<SkillNames>();
            for (int i = 0; i < skillNames.Length; i++)
            {
                SkillNames skillName = skillNames[i];
                if (skillName == SkillNames.None)
                {
                    continue;
                }

                UISkillSlotItem item = CreateItem(skillName);
                if (item != null)
                {
                    _items.Add(item);
                    _skillItemMap[skillName] = item;
                }
            }

            Log.Info(LogTags.UI_Page, "스킬 아이템 {0}개 생성 완료", _items.Count);
        }

        private UISkillSlotItem CreateItem(SkillNames skillName)
        {
            if (_itemPrefab == null || _contentParent == null)
            {
                return null;
            }

            UISkillSlotItem item = Instantiate(_itemPrefab, _contentParent);
            item.gameObject.SetActive(true);
            item.Setup(skillName);

            return item;
        }

        private void ClearItems()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null)
                {
                    Destroy(_items[i].gameObject);
                }
            }
            _items.Clear();
            _skillItemMap.Clear();
        }

        public void Refresh()
        {
            RefreshAllItems();
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

        public UISkillSlotItem FindItem(SkillNames skillName)
        {
            return _skillItemMap.TryGetValue(skillName, out UISkillSlotItem item) ? item : null;
        }

        private void OnDestroy()
        {
            ClearItems();
        }
    }
}

