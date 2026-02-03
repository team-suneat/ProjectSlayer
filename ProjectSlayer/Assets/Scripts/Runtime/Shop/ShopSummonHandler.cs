using TeamSuneat.Data;

namespace TeamSuneat
{
    public static class ShopSummonHandler
    {
        private static readonly float[] GradeResultValues = { 1f, 2f, 3f, 4f, 5f, 6f };

        public static GradeNames PickWeaponGrade(int summonLevel)
        {
            WeaponSummonConfigAsset config = ScriptableDataManager.Instance?.GetWeaponSummonConfigAsset();
            if (config == null)
            {
                return GradeNames.None;
            }
            float[] probabilities = config.GetProbabilities(summonLevel);
            return PickGradeByProbabilities(probabilities);
        }

        public static GradeNames PickAccessoryGrade(int summonLevel)
        {
            AccessorySummonConfigAsset config = ScriptableDataManager.Instance?.GetAccessorySummonConfigAsset();
            if (config == null)
            {
                return GradeNames.None;
            }
            float[] probabilities = config.GetProbabilities(summonLevel);
            return PickGradeByProbabilities(probabilities);
        }

        public static GradeNames PickSkillCardGrade()
        {
            SkillCardSummonConfigAsset config = ScriptableDataManager.Instance?.GetSkillCardSummonConfigAsset();
            if (config == null)
            {
                return GradeNames.None;
            }
            return config.PickGrade();
        }

        public static int PickWeaponLevel()
        {
            WeaponSummonConfigAsset config = ScriptableDataManager.Instance?.GetWeaponSummonConfigAsset();
            if (config == null)
            {
                return 0;
            }
            return config.PickLevel();
        }

        public static int PickAccessoryLevel()
        {
            AccessorySummonConfigAsset config = ScriptableDataManager.Instance?.GetAccessorySummonConfigAsset();
            if (config == null)
            {
                return 0;
            }
            return config.PickLevel();
        }

        private static GradeNames PickGradeByProbabilities(float[] probabilities)
        {
            if (probabilities == null || probabilities.Length == 0)
            {
                return GradeNames.None;
            }
            Gacha gacha = new Gacha();
            int count = probabilities.Length < 6 ? probabilities.Length : 6;
            float[] probs = new float[count];
            float[] resultValues = new float[count];
            for (int i = 0; i < count; i++)
            {
                probs[i] = probabilities[i];
                resultValues[i] = GradeResultValues[i];
            }
            gacha.Set(probs, resultValues);
            int index = gacha.Pick();
            if (index < 0)
            {
                return GradeNames.None;
            }
            return (GradeNames)(int)gacha.ResultValues[index];
        }
    }
}