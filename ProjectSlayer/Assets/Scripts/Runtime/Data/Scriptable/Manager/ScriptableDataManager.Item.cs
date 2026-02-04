using System.Collections.Generic;

namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        #region Item Find Methods

        public ItemAsset FindItem(ItemNames key)
        {
            ItemAsset asset = FindItem(BitConvert.Enum32ToInt(key));
            if (asset == null)
            {
                Log.Warning("아이템 에셋 데이터를 찾을 수 없습니다: {0}", key.ToLogString());
            }
            return asset;
        }

        private ItemAsset FindItem(int tid)
        {
            if (_itemAssets.ContainsKey(tid))
            {
                return _itemAssets[tid];
            }

            return null;
        }

        #endregion Item Find Methods

        #region Item FindClone Methods

        public ItemAssetData FindItemClone(ItemNames itemName)
        {
            if (itemName != ItemNames.None)
            {
                ItemAssetData assetData = FindItemClone(BitConvert.Enum32ToInt(itemName));
                if (!assetData.IsValid())
                {
                    Log.Warning(LogTags.ScriptableData, "아이템 데이터를 찾을 수 없습니다. {0}({1})", itemName, itemName.ToLogString());
                }
                return assetData;
            }

            return new ItemAssetData();
        }

        public ItemAssetData FindItemClone(int itemTID)
        {
            if (_itemAssets.ContainsKey(itemTID))
            {
                return _itemAssets[itemTID].CreateDataClone();
            }

#if UNITY_EDITOR
            ItemNames itemName = itemTID.ToEnum<ItemNames>();
            Log.Warning(LogTags.ScriptableData, "아이템 데이터를 찾을 수 없습니다. {0}({1})", itemName, itemName.ToLogString());
#endif

            return new ItemAssetData();
        }

        #endregion Item FindClone Methods

        #region Item Refresh Methods

        public void RefreshAllItem()
        {
            foreach (KeyValuePair<int, ItemAsset> item in _itemAssets) { Refresh(item.Value); }
        }

        private void Refresh(ItemAsset itemAsset)
        {
            itemAsset?.Refresh();
        }

        #endregion Item Refresh Methods

        #region Item Validation Methods

        private void CheckValidItemsOnLoadAssets()
        {
#if UNITY_EDITOR
            ItemNames[] keys = EnumEx.GetValues<ItemNames>();
            int tid = 0;
            for (int i = 1; i < keys.Length; i++)
            {
                tid = BitConvert.Enum32ToInt(keys[i]);
                if (!_itemAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "아이템 에셋이 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        #endregion Item Validation Methods
    }
}