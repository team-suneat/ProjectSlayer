using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary> 프로젝트에서 생성한 Scriptable Object를 관리합니다. </summary>
    public partial class ScriptableDataManager : Singleton<ScriptableDataManager>
    {
        private GameDefineAsset _gameDefine;
        private LogSettingAsset _logSetting;

        private ExperienceConfigAsset _experienceConfigAsset; // 캐릭터 경험치
        private MonsterStatConfigAsset _monsterStatConfigAsset; // 몬스터 스탯
        private EnhancementDataAsset _enhancementDataAsset; // 캐릭터 강화
        private GrowthDataAsset _growthDataAsset; // 캐릭터 성장

        private readonly Dictionary<int, HitmarkAsset> _hitmarkAssets = new();
        private readonly Dictionary<int, BuffAsset> _buffAssets = new();
        private readonly Dictionary<int, BuffStateEffectAsset> _stateEffectAssets = new();
        private readonly Dictionary<int, PassiveAsset> _passiveAssets = new();
        private readonly Dictionary<int, FontAsset> _fontAssets = new();
        private readonly Dictionary<int, FloatyAsset> _floatyAssets = new();
        private readonly Dictionary<int, FlickerAsset> _flickerAssets = new();
        private readonly Dictionary<int, SoundAsset> _soundAssets = new();
        private readonly Dictionary<int, StageAsset> _stageAssets = new();
        private readonly Dictionary<int, AreaAsset> _areaAssets = new();

        public void Clear()
        {
            _logSetting = null;
            _gameDefine = null;

            _soundAssets.Clear();
            _hitmarkAssets.Clear();
            _buffAssets.Clear();
            _stateEffectAssets.Clear();
            _passiveAssets.Clear();
            _fontAssets.Clear();
            _floatyAssets.Clear();
            _flickerAssets.Clear();
            _stageAssets.Clear();
            _areaAssets.Clear();
            _enhancementDataAsset = null;
            _growthDataAsset = null;
            _experienceConfigAsset = null;
            _monsterStatConfigAsset = null;
        }

        public void RefreshAll()
        {
            RefreshAllBuff();
            RefreshAllPassive();
            RefreshAllHitmark();
            RefreshAllFonts();
            RefreshAllFlicker();
            RefreshAllFloaty();
            RefreshAllSounds();
            RefreshAllStage();
            RefreshAllArea();
            RefreshAllEnhancement();
            RefreshAllGrowth();
            RefreshAllExperienceConfig();
            RefreshAllMonsterStatConfig();
        }
    }
}