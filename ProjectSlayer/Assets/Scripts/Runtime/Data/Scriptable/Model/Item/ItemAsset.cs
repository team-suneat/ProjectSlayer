using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using TeamSuneat;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Item", menuName = "TeamSuneat/Scriptable/Item")]
    public class ItemAsset : XScriptableObject
    {
        [Title("#ItemAsset")]
        public ItemAssetData Data;

        public ItemNames Name
        {
            get => Data.Name;
            set => Data.Name = value;
        }

        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public override void OnLoadData()
        {
            base.OnLoadData();

            if (Data.IsChangingAsset)
            {
                Log.Error("Asset의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }

            Data.OnLoadData();
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Data.Name, NameString))
                {
                    Log.Error($"아이템 에셋의 이름 갱신에 실패했습니다. {name}({NameString})");
                }

                Data.Validate();
            }
        }

        public override void Rename()
        {
            Rename("Item");
        }

        public override void Refresh()
        {
            NameString = Data.Name.ToString();
            Data.Refresh();

            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameString, Name);
            if (Data.RefreshWithoutSave())
            {
                _hasChangedWhiteRefreshAll = true;
            }

            base.RefreshWithoutSave();
            return _hasChangedWhiteRefreshAll;
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
#endif

            ItemNames[] itemNames = EnumEx.GetValues<ItemNames>();
            int itemCount = 0;

            Log.Info("모든 아이템 에셋의 갱신을 시작합니다: {0}", itemNames.Length);

            base.RefreshAll();

            for (int i = 1; i < itemNames.Length; i++)
            {
                if (itemNames[i] != ItemNames.None)
                {
                    ItemAsset asset = ScriptableDataManager.Instance.FindItem(itemNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            itemCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(itemNames.Length);
                EditorUtility.DisplayProgressBar("모든 아이템 에셋의 갱신", itemNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 아이템 에셋의 갱신을 종료합니다: {0}/{1}", itemCount.ToSelectString(0), itemNames.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            ItemNames[] itemNames = EnumEx.GetValues<ItemNames>();
            for (int i = 1; i < itemNames.Length; i++)
            {
                if (itemNames[i] == ItemNames.None)
                {
                    continue;
                }

                ItemAsset asset = ScriptableDataManager.Instance.FindItem(itemNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<ItemAsset>("Item", itemNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new ItemAssetData
                        {
                            Name = itemNames[i]
                        };
                        asset.NameString = itemNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif

        public ItemAssetData CreateDataClone()
        {
            return Data.Clone();
        }
    }
}
