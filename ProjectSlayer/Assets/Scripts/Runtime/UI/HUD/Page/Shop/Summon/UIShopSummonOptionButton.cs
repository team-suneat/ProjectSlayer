using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    [System.Serializable]
    public class ShopSummonOptionUnityEvent : UnityEvent<ShopSummonCategory, ShopSummonOptionType>
    { }

    // 소환 옵션 버튼 - 1회/11회 보석 또는 광고 11회 중 하나
    public class UIShopSummonOptionButton : XBehaviour
    {
        private const CurrencyNames SUMMON_CURRENCY = CurrencyNames.Diamond;
        private const int GEM_SINGLE_COST = 50;
        private const int GEM_MULTI_COST = 500;
        private const int GEM_SINGLE_COST_SKILL = 300;
        private const int GEM_MULTI_COST_SKILL = 3000;
        private const int SUMMON_COUNT_SINGLE = 1;
        private const int SUMMON_COUNT_MULTI = 11;

        [SerializeField] private UIPurchaseButton _purchaseButton;
        [SerializeField] private UILocalizedText _summonCountText;

        [FoldoutGroup("#Events")]
        [SerializeField] private ShopSummonOptionUnityEvent _onOptionClicked;

        private ShopSummonCategory _category;
        private ShopSummonOptionType _optionType;
        private int _summonCount = SUMMON_COUNT_SINGLE;

        public ShopSummonCategory Category => _category;
        public ShopSummonOptionType OptionType => _optionType;
        public int SummonCount => _summonCount;
        public ShopSummonOptionUnityEvent OnOptionClicked => _onOptionClicked;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _purchaseButton ??= GetComponentInChildren<UIPurchaseButton>();
            _summonCountText ??= this.FindComponent<UILocalizedText>("Summon Count Text");
        }

        public void Setup(ShopSummonCategory category, ShopSummonOptionType optionType)
        {
            _category = category;
            _optionType = optionType;
            _summonCount = optionType == ShopSummonOptionType.GemSingle ? SUMMON_COUNT_SINGLE : SUMMON_COUNT_MULTI;

            if (_optionType == ShopSummonOptionType.AdMulti)
            {
                _purchaseButton?.RegisterClickSuccessEvent(InvokeOptionClicked);
            }
            else
            {
                int cost = GetGemCost();
                _purchaseButton?.Setup(SUMMON_CURRENCY, cost);
                _purchaseButton?.RegisterClickSuccessEvent(InvokeOptionClicked);
            }

            Refresh();
        }

        public void SetSummonCount(int count)
        {
            _summonCount = count;
            RefreshSummonCountText();
        }

        public void Refresh()
        {
            _purchaseButton?.Refresh();
            RefreshSummonCountText();
        }

        private int GetGemCost()
        {
            if (_category == ShopSummonCategory.SkillCard)
            {
                return _optionType == ShopSummonOptionType.GemSingle ? GEM_SINGLE_COST_SKILL : GEM_MULTI_COST_SKILL;
            }

            return _optionType == ShopSummonOptionType.GemSingle ? GEM_SINGLE_COST : GEM_MULTI_COST;
        }

        private void RefreshSummonCountText()
        {
            if (_summonCountText == null)
            {
                return;
            }

            _summonCountText.SetText($"{_summonCount}회 소환");
        }

        private void InvokeOptionClicked()
        {
            switch (_optionType)
            {
                case ShopSummonOptionType.GemSingle:
                    OnGemSingleClicked();
                    break;

                case ShopSummonOptionType.GemMulti:
                    OnGemMultiClicked();
                    break;

                case ShopSummonOptionType.AdMulti:
                    OnAdMultiClicked();
                    break;
            }

            _onOptionClicked?.Invoke(_category, _optionType);
        }

        private void OnGemSingleClicked()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (_category == ShopSummonCategory.Weapon)
            {
                GradeNames grade = ShopSummonHandler.PickWeaponGrade(profile.Weapon.SummonLevel);
                int level = ShopSummonHandler.PickWeaponLevel();
                ItemNames itemName = ItemNameHelper.ConvertToWeaponName(grade, level);

                profile?.Weapon.AddWeapon(itemName);
            }
            else if (_category == ShopSummonCategory.Accessory)
            {
                GradeNames grade = ShopSummonHandler.PickAccessoryGrade(profile.Accessory.SummonLevel);
                int level = ShopSummonHandler.PickAccessoryLevel();
                ItemNames itemName = ItemNameHelper.ConvertToAccessoryName(grade, level);

                profile?.Accessory.AddAccessory(itemName);
            }
            else if (_category == ShopSummonCategory.SkillCard)
            {
                GradeNames grade = ShopSummonHandler.PickSkillCardGrade();
                SkillNames skillName = SkillNameHelper.PickSkillName(grade);

                profile?.Skill.AddSkillCard(skillName);
            }
        }

        private void OnGemMultiClicked()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (_category == ShopSummonCategory.Weapon)
            {
                for (int i = 0; i < _summonCount; i++)
                {
                    GradeNames grade = ShopSummonHandler.PickWeaponGrade(profile.Weapon.SummonLevel);
                    int level = ShopSummonHandler.PickWeaponLevel();
                    ItemNames itemName = ItemNameHelper.ConvertToWeaponName(grade, level);

                    profile?.Weapon.AddWeapon(itemName);
                }
            }
            else if (_category == ShopSummonCategory.Accessory)
            {
                for (int i = 0; i < _summonCount; i++)
                {
                    GradeNames grade = ShopSummonHandler.PickAccessoryGrade(profile.Accessory.SummonLevel);
                    int level = ShopSummonHandler.PickAccessoryLevel();
                    ItemNames itemName = ItemNameHelper.ConvertToAccessoryName(grade, level);

                    profile?.Accessory.AddAccessory(itemName);
                }
            }
            else if (_category == ShopSummonCategory.SkillCard)
            {
                for (int i = 0; i < _summonCount; i++)
                {
                    GradeNames grade = ShopSummonHandler.PickSkillCardGrade();
                    SkillNames skillName = SkillNameHelper.PickSkillName(grade);

                    profile?.Skill.AddSkillCard(skillName);
                }
            }
        }

        private void OnAdMultiClicked()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (_category == ShopSummonCategory.Weapon)
            {
                for (int i = 0; i < _summonCount; i++)
                {
                    GradeNames grade = ShopSummonHandler.PickWeaponGrade(profile.Weapon.SummonLevel);
                    int level = ShopSummonHandler.PickWeaponLevel();
                    ItemNames itemName = ItemNameHelper.ConvertToWeaponName(grade, level);

                    profile?.Weapon.AddWeapon(itemName);
                }
            }
            else if (_category == ShopSummonCategory.Accessory)
            {
                for (int i = 0; i < _summonCount; i++)
                {
                    GradeNames grade = ShopSummonHandler.PickAccessoryGrade(profile.Accessory.SummonLevel);
                    int level = ShopSummonHandler.PickAccessoryLevel();
                    ItemNames itemName = ItemNameHelper.ConvertToAccessoryName(grade, level);

                    profile?.Accessory.AddAccessory(itemName);
                }
            }
            else if (_category == ShopSummonCategory.SkillCard)
            {
                for (int i = 0; i < _summonCount; i++)
                {
                    GradeNames grade = ShopSummonHandler.PickSkillCardGrade();
                    SkillNames skillName = SkillNameHelper.PickSkillName(grade);

                    profile?.Skill.AddSkillCard(skillName);
                }
            }
        }
    }
}