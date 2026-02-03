using Sirenix.OdinInspector;
using UnityEngine;
using TeamSuneat;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class ItemAssetData : ScriptableData<int>
    {
        [SuffixLabel("개별 에셋 변경 모드")]
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("아이템 이름")]
        [GUIColor("GetItemNameColor")]
        public ItemNames Name;

        [SuffixLabel("등급 이름")]
        [GUIColor("GetGradeColor")]
        public GradeNames Grade;

        [FoldoutGroup("#String")]
        public string GradeNameString;

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public void Validate()
        {
            if (!IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Grade, GradeNameString))
                {
                    Log.Error("Item 에셋 데이터의 GradeNameString 변수를 변환할 수 없습니다. {0} ({1}), {2}", Name, Name.ToLogString(), GradeNameString);
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            GradeNameString = Grade.ToString();
            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();
        }

        public ItemAssetData Clone()
        {
            return new ItemAssetData
            {
                Name = Name,
                Grade = Grade,
                GradeNameString = GradeNameString,
                IsChangingAsset = IsChangingAsset
            };
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;
            UpdateIfChanged(ref GradeNameString, Grade);
            return _hasChangedWhiteRefreshAll;
        }

        private bool _hasChangedWhiteRefreshAll = false;

        private void UpdateIfChanged<TEnum>(ref string target, TEnum newValue) where TEnum : System.Enum
        {
            string newString = newValue?.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        protected Color GetGradeColor(GradeNames key)
        {
            return GetFieldColor(key);
        }

#endif
    }
}
