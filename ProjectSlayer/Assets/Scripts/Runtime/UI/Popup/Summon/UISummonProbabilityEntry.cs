using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UISummonProbabilityEntry : XBehaviour
    {
        [Title("#UISummonProbabilityEntry")]
        [SerializeField] private UILocalizedText _nameText;
        [SerializeField] private UILocalizedText _probabilityText;

        private GradeNames _grade;

        public GradeNames Grade => _grade;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _nameText ??= this.FindComponent<UILocalizedText>("Grade Name Text");
            _probabilityText ??= this.FindComponent<UILocalizedText>("Grade Probability Text");
        }

        public void Setup(GradeNames grade, float probability)
        {
            SetGrade(grade);
            SetProbability(probability);
        }

        private void SetGrade(GradeNames grade)
        {
            _grade = grade;

            if (_nameText != null)
            {
                string stringKey = grade.GetStringKey();
                _nameText.SetStringKey(stringKey);
            }
        }

        public void SetProbability(float probability)
        {
            if (_probabilityText != null)
            {
                string content = ValueStringEx.GetPercentString(probability);
                _probabilityText.SetText(content);
            }
        }

        public void Clear()
        {
            _grade = GradeNames.None;
            _nameText?.ResetText();
            _probabilityText?.ResetText();
        }
    }
}