using System.Collections.Generic;

namespace TeamSuneat.UserInterface
{
    // 장비 악세사리 탭 페이지 - 악세사리 슬롯 목록 표시
    public class UIEquipmentAccessoryPage : UIPage
    {
        private const int ACCESSORY_ITEM_ID_MIN = 2101;
        private const int ACCESSORY_ITEM_ID_MAX = 2999;

        private UIEquipmentAccessorySlotItem[] _items;
        private readonly Dictionary<ItemNames, UIEquipmentAccessorySlotItem> _accessoryItemMap = new();

        protected override void Awake()
        {
            base.Awake();
            _items = GetComponentsInChildren<UIEquipmentAccessorySlotItem>(true);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (!_items.IsValid())
            {
                Log.Warning(LogTags.UI_Page, "UIEquipmentAccessorySlotItem을 찾을 수 없습니다.");
                return;
            }

            SetupAccessoryItems();
            RefreshAllItems();
        }

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        private void SetupAccessoryItems()
        {
            _accessoryItemMap.Clear();

            ItemNames[] itemNames = EnumEx.GetValues<ItemNames>(true);
            List<ItemNames> validAccessoryNames = new();

            for (int i = 0; i < itemNames.Length; i++)
            {
                ItemNames name = itemNames[i];
                if (name == ItemNames.None)
                {
                    continue;
                }

                int id = (int)name;
                if (id is >= ACCESSORY_ITEM_ID_MIN and <= ACCESSORY_ITEM_ID_MAX)
                {
                    validAccessoryNames.Add(name);
                }
            }

            int itemIndex = 0;
            for (int i = 0; i < validAccessoryNames.Count; i++)
            {
                ItemNames accessoryName = validAccessoryNames[i];
                if (itemIndex >= _items.Length)
                {
                    Log.Warning(LogTags.UI_Page, "악세사리 슬롯 아이템 개수가 부족합니다. 필요한 개수: {0}, 현재 개수: {1}", validAccessoryNames.Count, _items.Length);
                    break;
                }

                if (_items[itemIndex] != null)
                {
                    _items[itemIndex].Setup(accessoryName);
                    _accessoryItemMap.Add(accessoryName, _items[itemIndex]);
                    itemIndex++;
                }
            }

            Log.Info(LogTags.UI_Page, "악세사리 슬롯 아이템 {0}개 설정 완료", itemIndex);
        }

        public void Refresh()
        {
            RefreshAllItems();
        }

        private void RefreshAllItems()
        {
            if (!_items.IsValid())
            {
                return;
            }

            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] != null)
                {
                    _items[i].Refresh();
                }
            }
        }

        public UIEquipmentAccessorySlotItem FindItem(ItemNames accessoryName)
        {
            return _accessoryItemMap.TryGetValue(accessoryName, out UIEquipmentAccessorySlotItem item) ? item : null;
        }
    }
}