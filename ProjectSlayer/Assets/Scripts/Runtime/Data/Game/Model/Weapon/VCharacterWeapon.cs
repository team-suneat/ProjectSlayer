using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterWeapon
    {
        public Dictionary<string, VWeapon> Weapons = new();
        public List<string> UnlockedWeapons = new();
        public string EquippedWeaponNameString;
        public int SummonLevel;
        public int SummonExperience;

        [NonSerialized]
        private readonly Dictionary<ItemNames, VWeapon> _weaponMap = new();

        [NonSerialized]
        private ItemNames _equippedWeaponName = ItemNames.None;

        public ItemNames EquippedWeaponName => _equippedWeaponName;

        //

        public void OnLoadGameData()
        {
            _weaponMap.Clear();

            ItemNames itemName = ItemNames.None;
            foreach (KeyValuePair<string, VWeapon> kvp in Weapons)
            {
                VWeapon weapon = kvp.Value;
                weapon.OnLoadGameData();

                if (!EnumEx.ConvertTo(ref itemName, kvp.Key))
                {
                    Log.Error(LogTags.GameData_Weapon, "무기 키를 ItemNames로 변환하지 못했습니다: {0}", kvp.Key);
                    continue;
                }

                weapon.Name = itemName;
                _weaponMap[itemName] = weapon;
            }

            if (!string.IsNullOrEmpty(EquippedWeaponNameString))
            {
                EnumEx.ConvertTo(ref _equippedWeaponName, EquippedWeaponNameString);
            }

            Log.Info(LogTags.GameData_Weapon, "[Character] 무기 데이터를 불러옵니다. 총 {0}개, 장착: {1}",
                Weapons.Count, _equippedWeaponName.ToLogString());
        }

        //

        public bool CheckUnlocked(ItemNames weaponName)
        {
            return UnlockedWeapons.Contains(weaponName.ToString());
        }

        public void Unlock(ItemNames weaponName)
        {
            string key = weaponName.ToString();
            if (!UnlockedWeapons.Contains(key))
            {
                UnlockedWeapons.Add(key);
                Log.Info(LogTags.GameData_Weapon, "무기를 해금합니다: {0}", weaponName);
            }
        }

        //

        public bool HasWeapon(ItemNames weaponName)
        {
            return Weapons.ContainsKey(weaponName.ToString());
        }

        public VWeapon FindWeapon(ItemNames weaponName)
        {
            if (_weaponMap.TryGetValue(weaponName, out VWeapon weapon))
            {
                return weapon;
            }

            Log.Warning(LogTags.GameData_Weapon, "무기를 찾을 수 없습니다: {0}", weaponName.ToLogString());
            return null;
        }

        public VWeapon FindEquippedWeapon()
        {
            if (_equippedWeaponName == ItemNames.None)
            {
                return null;
            }

            return FindWeapon(_equippedWeaponName);
        }

        public void AddWeapon(ItemNames weaponName, int experience = 1)
        {
            string key = weaponName.ToString();
            if (!_weaponMap.ContainsKey(weaponName))
            {
                VWeapon newWeapon = new(weaponName);
                Weapons[key] = newWeapon;
                _weaponMap[weaponName] = newWeapon;
            }
            else
            {
                _weaponMap[weaponName].Level += 1;
            }

            Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록합니다: {0}(Lv.{1})", weaponName.ToLogString(), _weaponMap[weaponName].Level);
            AddSummonExperience(experience);
        }

        public void AddWeapon(ItemNames weaponName, GradeNames gradeName, StatNames statName)
        {
            AddWeapon(weaponName);

            if (gradeName != GradeNames.None && statName != StatNames.None && _weaponMap.TryGetValue(weaponName, out VWeapon weapon))
            {
                weapon.AddGrade(gradeName, statName);
            }
            else
            {
                Log.Error("인게임 무기 추가에 필요한 올바른 등급 또는 능력치 이름이 아닙니다: {0}, {1}", gradeName.ToLogString(), statName.ToLogString());
            }
        }

        public void RemoveWeapon(ItemNames weaponName)
        {
            string key = weaponName.ToString();
            if (_weaponMap.ContainsKey(weaponName))
            {
                _ = Weapons.Remove(key);
                _ = _weaponMap.Remove(weaponName);

                if (_equippedWeaponName == weaponName)
                {
                    _equippedWeaponName = ItemNames.None;
                    EquippedWeaponNameString = string.Empty;
                }

                Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록해제합니다: {0}", weaponName.ToLogString());
            }
        }

        public void EquipWeapon(ItemNames weaponName)
        {
            if (weaponName == ItemNames.None)
            {
                _equippedWeaponName = ItemNames.None;
                EquippedWeaponNameString = string.Empty;
                Log.Info(LogTags.GameData_Weapon, "무기 장착을 해제합니다.");
                return;
            }

            if (!_weaponMap.ContainsKey(weaponName))
            {
                Log.Warning(LogTags.GameData_Weapon, "장착할 무기가 없습니다: {0}", weaponName.ToLogString());
                return;
            }

            _equippedWeaponName = weaponName;
            EquippedWeaponNameString = weaponName.ToString();
            Log.Info(LogTags.GameData_Weapon, "무기를 장착합니다: {0}", weaponName.ToLogString());
        }

        //

        private void AddSummonExperience(int value)
        {
            if (value <= 0)
            {
                return;
            }

            SummonExperience += value;
            SummonLevelConfigAsset asset = ScriptableDataManager.Instance?.GetSummonLevelConfigAsset();
            if (asset == null)
            {
                return;
            }

            // 에셋 레벨은 2,3,4…(config) / 게임 SummonLevel은 0,1,2… → 누적량은 config 레벨 (SummonLevel+1) 기준
            while (true)
            {
                int currentTotal = SummonLevel == 0
                    ? 0
                    : asset.GetRequiredSummonCountForLevel(SummonLevel + 1);
                int nextTotal = asset.GetRequiredSummonCountForLevel(SummonLevel + 2);
                int requiredCount = nextTotal - currentTotal;

                if (requiredCount <= 0)
                {
                    break;
                }

                if (requiredCount <= SummonExperience)
                {
                    SummonLevel += 1;
                    SummonExperience -= requiredCount;
                }
                else
                {
                    break;
                }
            }
        }

        //

        public static VCharacterWeapon CreateDefault()
        {
            VCharacterWeapon defaultWeapons = new();

            defaultWeapons.AddWeapon(ItemNames.RustySword, 0);
            defaultWeapons.EquipWeapon(ItemNames.RustySword);
            defaultWeapons.SummonLevel = 1;

            return defaultWeapons;
        }
    }
}