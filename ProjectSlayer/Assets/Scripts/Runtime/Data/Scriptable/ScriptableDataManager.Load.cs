using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        public bool CheckLoaded()
        {
            if (_logSetting == default) { return false; }
            else if (_gameDefine == default) { return false; }
            else if (!_buffAssets.IsValid()) { return false; }
            else if (!_stateEffectAssets.IsValid()) { return false; }
            else if (!_passiveAssets.IsValid()) { return false; }
            else if (!_hitmarkAssets.IsValid()) { return false; }
            else if (!_fontAssets.IsValid()) { return false; }
            else if (!_floatyAssets.IsValid()) { return false; }
            else if (!_flickerAssets.IsValid()) { return false; }
            else if (!_soundAssets.IsValid()) { return false; }
            else if (_enhancementDataAsset == null) { return false; }
            else if (_growthDataAsset == null) { return false; }
            else if (_experienceConfigAsset == null) { return false; }

            return true;
        }

        protected void OnLoadData()
        {
            // Core.cs의 OnLoadData() 메서드 호출
            _logSetting?.OnLoadData();

            // 강화 시스템 데이터 OnLoadData() 메서드 호출
            _enhancementDataAsset?.OnLoadData();

            // 성장 시스템 데이터 OnLoadData() 메서드 호출
            _growthDataAsset?.OnLoadData();

            // 경험치 설정 데이터 OnLoadData() 메서드 호출
            _experienceConfigAsset?.OnLoadData();

            // 사운드 OnLoadData() 메서드 호출
            foreach (KeyValuePair<int, SoundAsset> asset in _soundAssets)
            {
            }
        }

        public void LoadScriptableAssets()
        {
            int count = 0;
            string[] pathArray = PathManager.FindAllAssetPath();
            Clear();

            Log.Info("스크립터블 파일을 읽기 시작합니다.");

            for (int i = 0; i < pathArray.Length; i++)
            {
                string path = pathArray[i];

                if (LoadLogSettingSync(path))
                {
                    count += 1;
                }
                else if (LoadGameDefineSync(path))
                {
                    count += 1;
                }
                else if (LoadBuffSync(path))
                {
                    count += 1;
                }
                else if (LoadBuffStateEffectSync(path))
                {
                    count += 1;
                }
                else if (LoadPassiveSync(path))
                {
                    count += 1;
                }
                else if (LoadHitmarkSync(path))
                {
                    count += 1;
                }
                else if (LoadFontSync(path))
                {
                    count += 1;
                }
                else if (LoadFloatySync(path))
                {
                    count += 1;
                }
                else if (LoadFlickerSync(path))
                {
                    count += 1;
                }
                else if (LoadSoundSync(path))
                {
                    count += 1;
                }
                else if (LoadEnhancementDataSync(path))
                {
                    count += 1;
                }
                else if (LoadGrowthDataSync(path))
                {
                    count += 1;
                }
                else if (LoadExperienceConfigSync(path))
                {
                    count += 1;
                }
            }

            Log.Info("파일을 읽어왔습니다. Count: {0}", count.ToString());

            OnLoadData();
        }

        /// <summary>
        /// 강화 시스템 데이터 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadEnhancementDataSync(string filePath)
        {
            if (!filePath.Contains("EnhancementData"))
            {
                return false;
            }

            EnhancementDataAsset asset = ResourcesManager.LoadResource<EnhancementDataAsset>(filePath);
            if (asset != null)
            {
                if (_enhancementDataAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "강화 시스템 데이터 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _enhancementDataAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _enhancementDataAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        /// <summary>
        /// 성장 시스템 데이터 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadGrowthDataSync(string filePath)
        {
            if (!filePath.Contains("GrowthData"))
            {
                return false;
            }

            GrowthDataAsset asset = ResourcesManager.LoadResource<GrowthDataAsset>(filePath);
            if (asset != null)
            {
                if (_growthDataAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "성장 시스템 데이터 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _growthDataAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _growthDataAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        /// <summary>
        /// 경험치 설정 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadExperienceConfigSync(string filePath)
        {
            if (!filePath.Contains("ExperienceConfig"))
            {
                return false;
            }

            ExperienceConfigAsset asset = ResourcesManager.LoadResource<ExperienceConfigAsset>(filePath);
            if (asset != null)
            {
                if (_experienceConfigAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "경험치 설정 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _experienceConfigAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _experienceConfigAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        public async Task LoadScriptableAssetsAsync()
        {
            Clear();
            int count = 0;

            IList<ScriptableObject> assets = await ResourcesManager.LoadResourcesByLabelAsync<UnityEngine.ScriptableObject>(AddressableLabels.Scriptable);
            for (int i = 0; i < assets.Count; i++)
            {
                ScriptableObject asset = assets[i];
                if (asset == null)
                {
                    continue;
                }

                switch (asset)
                {
                    case LogSettingAsset logSetting:
                        _logSetting = logSetting;
                        count++;
                        break;

                    case GameDefineAsset gameDefine:
                        _gameDefine = gameDefine;
                        count++;
                        break;

                    case BuffAsset buff:
                        if (!_buffAssets.ContainsKey(buff.TID))
                        {
                            _buffAssets[buff.TID] = buff;
                            count++;
                        }
                        break;

                    case BuffStateEffectAsset buffStateEffect:
                        if (!_stateEffectAssets.ContainsKey(buffStateEffect.TID))
                        {
                            _stateEffectAssets[buffStateEffect.TID] = buffStateEffect;
                            count++;
                        }
                        break;

                    case PassiveAsset passive:
                        if (!_passiveAssets.ContainsKey(passive.TID))
                        {
                            _passiveAssets[passive.TID] = passive;
                            count++;
                        }
                        break;

                    case HitmarkAsset hitmark:
                        if (!_hitmarkAssets.ContainsKey(hitmark.TID))
                        {
                            _hitmarkAssets[hitmark.TID] = hitmark;
                            count++;
                        }
                        break;

                    case FontAsset font:
                        if (!_fontAssets.ContainsKey(font.TID))
                        {
                            _fontAssets[font.TID] = font;
                            count++;
                        }
                        break;

                    case FloatyAsset floaty:
                        if (!_floatyAssets.ContainsKey(floaty.TID))
                        {
                            _floatyAssets[floaty.TID] = floaty;
                            count++;
                        }
                        break;

                    case FlickerAsset flicker:
                        if (!_flickerAssets.ContainsKey(flicker.TID))
                        {
                            _flickerAssets[flicker.TID] = flicker;
                            count++;
                        }
                        break;

                    case SoundAsset sound:
                        if (!_soundAssets.ContainsKey(sound.TID))
                        {
                            _soundAssets[sound.TID] = sound;
                            count++;
                        }
                        break;

                    case EnhancementDataAsset enhancementData:
                        if (_enhancementDataAsset == null)
                        {
                            _enhancementDataAsset = enhancementData;
                            count++;
                        }
                        break;

                    case GrowthDataAsset growthData:
                        if (_growthDataAsset == null)
                        {
                            _growthDataAsset = growthData;
                            count++;
                        }
                        break;

                    case ExperienceConfigAsset experienceConfig:
                        if (_experienceConfigAsset == null)
                        {
                            _experienceConfigAsset = experienceConfig;
                            count++;
                        }
                        break;
                }
            }

            Log.Info("Addressable Scriptable 라벨로 파일을 읽어왔습니다. Count: {0}", count.ToString());

            OnLoadData();
        }
    }
}