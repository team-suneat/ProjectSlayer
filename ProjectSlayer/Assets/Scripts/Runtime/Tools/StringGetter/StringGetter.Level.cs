using TeamSuneat.Data;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetGradeString(this int level)
        {
            string format = JsonDataManager.FindStringClone(StringDataLabels.FORMAT_GRADE);

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(format, level);
            }

            return level.ToString();
        }

        public static string GetLevelString(this int level)
        {
            string format = JsonDataManager.FindStringClone(StringDataLabels.FORMAT_LEVEL);

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(format, level);
            }

            return level.ToString();
        }

        public static string GetMaxLevelString(this int level)
        {
            string format = JsonDataManager.FindStringClone(StringDataLabels.FORMAT_MAX_LEVEL);

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(format, level);
            }

            return level.ToString();
        }
    }
}