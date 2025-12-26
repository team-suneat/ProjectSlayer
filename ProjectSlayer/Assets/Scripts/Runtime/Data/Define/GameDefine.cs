namespace TeamSuneat
{
    public class GameDefine
    {
        #region 기본 설정

        public static bool IS_EDITOR
        {
            get
            {
#if UNITY_EDITOR
                return true;
#endif

                return false;
            }
        }

        public static bool IS_DEVELOPMENT_BUILD
        {
            get
            {
#if DEVELOPMENT_BUILD
                return true;
#endif

                return false;
            }
        }

        public static bool IS_EDITOR_OR_DEVELOPMENT_BUILD
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return true;
#endif

                return false;
            }
        }

        public const bool USE_ES3 = false;
        public const bool USE_AES_EDITOR = false;

        public const bool USE_DEBUG_LOG_ERROR = true;
        public const bool USE_DEBUG_LOG_WARNING = true;

        /// <summary> 기본 화면 너비 </summary>
        public const float DEFAULT_SCREEN_WIDTH = 1080;

        /// <summary> 기본 화면 높이 </summary>
        public const float DEFAULT_SCREEN_HEIGHT = 1920;

        /// <summary> 기본 화면 비율 </summary>
        public const float DEFAULT_SCREEN_RATE = 9f / 16f;

        #endregion 기본 설정

        #region 게임 설정

        public const int CHARACTER_MAX_LEVEL = 1000;

        /// <summary> 보스 모드 시간 제한 (초) </summary>
        public const float BOSS_MODE_TIME_LIMIT = 30f;

        #endregion 게임 설정

        #region 개발자 설정

        public static bool DEV_SCRIPTABLE_OBJECT_FORCE_REFRESH_ALL = false;

        #endregion 개발자 설정
    }
}