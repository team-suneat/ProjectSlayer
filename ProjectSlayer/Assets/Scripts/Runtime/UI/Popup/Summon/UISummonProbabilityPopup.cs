using Sirenix.OdinInspector;
using System.Text;
using TeamSuneat.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UISummonProbabilityPopup : UIPopup
    {
        private static readonly GradeNames[] GRADE_ORDER =
        {
            GradeNames.Common,
            GradeNames.Uncommon,
            GradeNames.Rare,
            GradeNames.Epic,
            GradeNames.Legendary,
            GradeNames.Mythic
        };

        private const int MIN_LEVEL = 1;
        private const int MAX_LEVEL = 10;

        [FoldoutGroup("#UISummonProbabilityPopup")]
        [SerializeField] private UILocalizedText _descriptionText;

        [FoldoutGroup("#UISummonProbabilityPopup")]
        [SerializeField] private GameObject _levelSection;

        [FoldoutGroup("#UISummonProbabilityPopup")]
        [SerializeField] private UILocalizedText _summonLevelText;

        [FoldoutGroup("#UISummonProbabilityPopup/Level")]
        [SerializeField] private Button _levelPrevButton;

        [FoldoutGroup("#UISummonProbabilityPopup/Level")]
        [SerializeField] private Button _levelNextButton;

        [FoldoutGroup("#UISummonProbabilityPopup/Grade")]
        [SerializeField] private UISummonProbabilityEntry[] _gradeEntries;

        [FoldoutGroup("#UISummonProbabilityPopup/Rank")]
        [SerializeField] private TextMeshProUGUI _rankContentText;

        private ShopSummonCategory _category;
        private int _currentLevel = MIN_LEVEL;

        public override UIPopupNames Name => UIPopupNames.SummonProbability;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _descriptionText = this.FindComponent<UILocalizedText>("Rect/Description Text");

            _levelSection = this.FindGameObject("Rect/Level Section");
            _summonLevelText = this.FindComponent<UILocalizedText>("Rect/Level Section/Summon Level Text");
            _levelPrevButton = this.FindComponent<Button>("Rect/Level Section/Prev Level Button");
            _levelNextButton = this.FindComponent<Button>("Rect/Level Section/Next Level Button");

            _gradeEntries = GetComponentsInChildren<UISummonProbabilityEntry>(true);

            _rankContentText = this.FindComponent<TextMeshProUGUI>("Rect/Rank Probability Text");
        }

        protected override void Awake()
        {
            base.Awake();
            AutoGetComponents();

            if (_levelPrevButton != null)
            {
                _levelPrevButton.onClick.AddListener(OnClickLevelPrev);
            }

            if (_levelNextButton != null)
            {
                _levelNextButton.onClick.AddListener(OnClickLevelNext);
            }
        }

        public void Setup(ShopSummonCategory category)
        {
            _category = category;
            _currentLevel = MIN_LEVEL;

            bool isEquipmentCategory = category is ShopSummonCategory.Weapon or ShopSummonCategory.Accessory;
            _descriptionText?.SetActive(isEquipmentCategory);
            _levelPrevButton?.SetActive(isEquipmentCategory);
            _levelNextButton?.SetActive(isEquipmentCategory);

            RefreshAll();
        }

        public override void Open()
        {
            base.Open();
            RefreshTitleText();
            RefreshAll();
        }

        public void RefreshAll()
        {
            RefreshLevelSection();
            RefreshGradeRows();
            RefreshRankSection();
        }

        private void RefreshLevelSection()
        {
            if (_category is not ShopSummonCategory.Weapon and not ShopSummonCategory.Accessory)
            {
                return;
            }

            if (_summonLevelText != null)
            {
                string format = JsonDataManager.FindStringClone(StringDataLabels.FORMAT_SUMMON_LEVEL);
                _summonLevelText.SetText(string.Format(format, _currentLevel.ToString()));
            }

            if (_levelPrevButton != null)
            {
                _levelPrevButton.SetActive(_currentLevel > MIN_LEVEL);
            }

            if (_levelNextButton != null)
            {
                _levelNextButton.SetActive(_currentLevel < MAX_LEVEL);
            }
        }

        private void RefreshGradeRows()
        {
            float[] probabilities = GetCurrentGradeProbabilities();
            if (probabilities == null || probabilities.Length == 0 || _gradeEntries == null)
            {
                return;
            }

            for (int i = 0; i < _gradeEntries.Length && i < GRADE_ORDER.Length; i++)
            {
                UISummonProbabilityEntry entry = _gradeEntries[i];
                if (entry == null)
                {
                    continue;
                }

                GradeNames grade = GRADE_ORDER[i];
                float p = i < probabilities.Length ? probabilities[i] : 0f;
                entry.Setup(grade, p);
            }
        }

        private void RefreshRankSection()
        {
            if (_rankContentText != null)
            {
                string content = BuildRankSectionContent();
                _rankContentText.text = content;
            }
        }

        private string BuildRankSectionContent()
        {
            if (_category == ShopSummonCategory.SkillCard)
            {
                return string.Empty;
            }

            float[] probabilities = null;
            float[] values = null;
            if (_category == ShopSummonCategory.Weapon)
            {
                WeaponSummonConfigAsset weaponConfig = ScriptableDataManager.Instance?.GetWeaponSummonConfigAsset();
                probabilities = weaponConfig?.GetLevelGachaProbabilities();
                values = weaponConfig?.GetLevelGachaResultValues();
            }
            else if (_category == ShopSummonCategory.Accessory)
            {
                AccessorySummonConfigAsset accessoryConfig = ScriptableDataManager.Instance?.GetAccessorySummonConfigAsset();
                probabilities = accessoryConfig?.GetLevelGachaProbabilities();
                values = accessoryConfig?.GetLevelGachaResultValues();
            }

            if (probabilities == null || values == null || probabilities.Length != values.Length || probabilities.Length == 0)
            {
                return string.Empty;
            }

            string format = JsonDataManager.FindStringClone(StringDataLabels.FORMAT_GRADE);
            StringBuilder sb = new();
            for (int i = probabilities.Length - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    _ = sb.Append(" / ");
                }
                int level = (int)values[i];
                _ = sb.Append(string.Format(format, (level + 1).ToString()));
                _ = sb.Append(": ");
                _ = sb.Append(ValueStringEx.GetPercentString(probabilities[i]));
            }
            return sb.ToString();
        }

        private float[] GetCurrentGradeProbabilities()
        {
            return _category switch
            {
                ShopSummonCategory.Weapon => ScriptableDataManager.Instance?.GetWeaponSummonConfigAsset()?.GetProbabilities(_currentLevel),
                ShopSummonCategory.Accessory => ScriptableDataManager.Instance?.GetAccessorySummonConfigAsset()?.GetProbabilities(_currentLevel),
                ShopSummonCategory.SkillCard => ScriptableDataManager.Instance?.GetSkillCardSummonConfigAsset()?.GetGradeProbabilities(),
                _ => null
            };
        }

        private void OnClickLevelPrev()
        {
            if (_currentLevel > MIN_LEVEL)
            {
                _currentLevel--;
                RefreshAll();
            }
        }

        private void OnClickLevelNext()
        {
            if (_currentLevel < MAX_LEVEL)
            {
                _currentLevel++;
                RefreshAll();
            }
        }

        private static string GetProbabilityDescriptionStringKey(ShopSummonCategory category)
        {
            return category switch
            {
                ShopSummonCategory.Weapon => "Summon_Probability_Description_Weapon",
                ShopSummonCategory.Accessory => "Summon_Probability_Description_Weapon",
                _ => string.Empty,
            };
        }
    }
}