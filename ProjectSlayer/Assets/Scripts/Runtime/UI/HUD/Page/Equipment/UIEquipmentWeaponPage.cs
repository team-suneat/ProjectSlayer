using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace TeamSuneat.UserInterface
{
    // 장비 무기 탭 페이지 - 무기 슬롯 목록 표시
    public class UIEquipmentWeaponPage : UIPage
    {
        private const int WEAPON_ITEM_ID_MIN = 1101;
        private const int WEAPON_ITEM_ID_MAX = 1999;

        [Title("#UIEquipmentWeaponPage")]
        [ShowInInspector]
        [ReadOnly]
        private UIEquipmentWeaponSlotItem[] _items;

        private readonly Dictionary<ItemNames, UIEquipmentWeaponSlotItem> _weaponItemMap = new();

        protected override void Awake()
        {
            base.Awake();
            _items = GetComponentsInChildren<UIEquipmentWeaponSlotItem>(true);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_items == null || _items.Length == 0)
            {
                Log.Warning(LogTags.UI_Page, "UIEquipmentWeaponSlotItem을 찾을 수 없습니다.");
                return;
            }

            SetupWeaponItems();
            RefreshAllItems();
        }

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        private void SetupWeaponItems()
        {
            _weaponItemMap.Clear();

            ItemNames[] itemNames = EnumEx.GetValues<ItemNames>(true);
            List<ItemNames> validWeaponNames = new();

            for (int i = 0; i < itemNames.Length; i++)
            {
                ItemNames name = itemNames[i];
                if (name == ItemNames.None)
                {
                    continue;
                }

                int id = (int)name;
                if (id is >= WEAPON_ITEM_ID_MIN and <= WEAPON_ITEM_ID_MAX)
                {
                    validWeaponNames.Add(name);
                }
            }

            int itemIndex = 0;
            for (int i = 0; i < validWeaponNames.Count; i++)
            {
                ItemNames weaponName = validWeaponNames[i];
                if (itemIndex >= _items.Length)
                {
                    Log.Warning(LogTags.UI_Page, "무기 슬롯 아이템 개수가 부족합니다. 필요한 개수: {0}, 현재 개수: {1}", validWeaponNames.Count, _items.Length);
                    break;
                }

                if (_items[itemIndex] != null)
                {
                    _items[itemIndex].Setup(weaponName);
                    _weaponItemMap.Add(weaponName, _items[itemIndex]);
                    itemIndex++;
                }
            }

            Log.Info(LogTags.UI_Page, "무기 슬롯 아이템 {0}개 설정 완료", itemIndex);
        }

        public void Refresh()
        {
            RefreshAllItems();
        }

        private void RefreshAllItems()
        {
            if (_items == null)
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

        public UIEquipmentWeaponSlotItem FindItem(ItemNames weaponName)
        {
            return _weaponItemMap.TryGetValue(weaponName, out UIEquipmentWeaponSlotItem item) ? item : null;
        }
    }
}