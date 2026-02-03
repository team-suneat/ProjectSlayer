using System;
using System.Linq;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.Development
{
    public enum DevelopmentToolTab
    {
        GameTime,
        LogTag,
        GamePlay,
        Cheat,
        Stat,
        GameData,
    }

    public class DevelopmentTools : MonoBehaviour
    {
        private bool _isWindowOpen = false;
        private bool _isFirstOpen = true;
        private Rect _windowRect;
        private DevelopmentToolsGUI _gui;
        private DevelopmentToolTab _selectedTab = DevelopmentToolTab.GameTime;

        private const KeyCode TOGGLE_KEY = KeyCode.F1;

        private void Awake()
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                gameObject.SetActive(false);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _gui = new DevelopmentToolsGUI();
            InitializeWindowRect();
        }

        private void InitializeWindowRect()
        {
            float width = Screen.width * 0.5f;
            float height = Screen.height * 0.5f;
            float x = 0f;
            float y = 0f;
            _windowRect = new Rect(x, y, width, height);
        }

        private void Update()
        {
            if (Input.GetKeyDown(TOGGLE_KEY))
            {
                bool wasOpen = _isWindowOpen;
                _isWindowOpen = !_isWindowOpen;

                if (_isWindowOpen && !wasOpen)
                {
                    if (_isFirstOpen)
                    {
                        InitializeWindowRect();
                        _isFirstOpen = false;
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                return;
            }

            if (!_isWindowOpen)
            {
                return;
            }

            // OnGUI 내부에서는 GUI.skin이 유효하므로 스타일 초기화 확인
            if (_gui.WindowStyle == null)
            {
                _gui.RefreshStyle(isEditor: false);
                // OnGUI 내부이므로 GUI.skin을 기반으로 스타일 업데이트
                if (GUI.skin != null)
                {
                    _gui.RefreshStyleFromSkin();
                }
            }

            float width = Screen.width * 0.5f;
            float height = Screen.height * 0.5f;
            _gui.RefreshSize(width, height);
            _windowRect.width = width;
            _windowRect.height = height;

            if (_isFirstOpen)
            {
                _windowRect.x = 0f;
                _windowRect.y = 0f;
            }

            _windowRect = GUILayout.Window(0, _windowRect, DrawWindow, "개발 도구 (F1 토글)", _gui.WindowStyle);
        }

        private void DrawWindow(int windowID)
        {
            _gui.ScrollPosition = GUILayout.BeginScrollView(_gui.ScrollPosition);

            _gui.DrawTitleLabel("[인게임 개발 도구]");
            GUILayout.Space(10);

            DrawTabButtons();
            GUILayout.Space(10);

            DrawSelectedTabContent();

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        private void DrawTabButtons()
        {
            string[] tabNames = new string[]
            {
                "게임 타임",
                "로그 태그",
                "게임 플레이",
                "치트",
                "능력치",
                "게임 데이터"
            };

            int newSelectedTab = _gui.DrawSelectionGrid((int)_selectedTab, tabNames, 4);

            if (newSelectedTab != (int)_selectedTab)
            {
                _selectedTab = (DevelopmentToolTab)newSelectedTab;
            }
        }

        private void DrawSelectedTabContent()
        {
            switch (_selectedTab)
            {
                case DevelopmentToolTab.GameTime:
                    DrawGameTimeSection();
                    break;

                case DevelopmentToolTab.LogTag:
                    DrawLogTagSection();
                    break;

                case DevelopmentToolTab.GamePlay:
                    DrawGamePlaySection();
                    break;

                case DevelopmentToolTab.Cheat:
                    DrawCheatSection();
                    break;

                case DevelopmentToolTab.Stat:
                    DrawStatSection();
                    break;

                case DevelopmentToolTab.GameData:
                    DrawGameDataSection();
                    break;
            }
        }

        private void DrawGameTimeSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("게임 타임 스케일");

            GUILayout.BeginHorizontal();
            _gui.DrawButton("0.1x", () => GameTimeManager.Instance?.SetFactor(0.1f));
            _gui.DrawButton("0.5x", () => GameTimeManager.Instance?.SetFactor(0.5f));
            _gui.DrawButton("1.0x", () => GameTimeManager.Instance?.SetFactor(1.0f));
            _gui.DrawButton("2.0x", () => GameTimeManager.Instance?.SetFactor(2.0f));
            _gui.DrawButton("3.0x", () => GameTimeManager.Instance?.SetFactor(3.0f));
            GUILayout.EndHorizontal();

            _gui.DrawContentLabel($"현재 타임 스케일: {Time.timeScale:F1}x");

            GUILayout.EndVertical();
        }

        private LogTags? _selectedLogTag = null;

        private void DrawLogTagSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("Log Tags");

            LogSettingAsset logSetting = ScriptableDataManager.Instance?.GetLogSetting();
            if (logSetting == null)
            {
                _gui.DrawContentLabel("LogSettingAsset을 불러올 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            (string title, LogTags[] tags)[] groups = GetLogTagGroups();

            for (int g = 0; g < groups.Length; g++)
            {
                string groupTitle = groups[g].title;
                LogTags[] groupTags = groups[g].tags;

                if (groupTags.Length == 0)
                {
                    continue;
                }

                _gui.DrawTitleLabel(groupTitle, useWidth: false, useHeight: true);
                GUILayout.Space(3);

                string[] tagDisplayNames = new string[groupTags.Length];
                for (int i = 0; i < groupTags.Length; i++)
                {
                    LogTags tag = groupTags[i];
                    string displayName = GetLogTagDisplayName(tag);
                    bool isEnabled = logSetting.Find(tag);

                    if (isEnabled)
                    {
                        tagDisplayNames[i] = displayName.ToSelectString();
                    }
                    else
                    {
                        tagDisplayNames[i] = displayName.ToDisableString();
                    }
                }

                int selectedIndexInGroup = -1;
                if (_selectedLogTag.HasValue)
                {
                    for (int i = 0; i < groupTags.Length; i++)
                    {
                        if (groupTags[i] == _selectedLogTag.Value)
                        {
                            selectedIndexInGroup = i;
                            break;
                        }
                    }
                }

                int newSelectedIndex = _gui.DrawSelectionGrid(selectedIndexInGroup, tagDisplayNames, 4, useWidth: true, useHeight: true);

                if (newSelectedIndex >= 0 && newSelectedIndex < groupTags.Length && newSelectedIndex != selectedIndexInGroup)
                {
                    LogTags selectedTag = groupTags[newSelectedIndex];
                    bool isEnabled = logSetting.Find(selectedTag);

                    if (isEnabled)
                    {
                        logSetting.SwitchOff(selectedTag);
                    }
                    else
                    {
                        logSetting.SwitchOn(selectedTag);
                    }
                    logSetting.Refresh();
                    _selectedLogTag = null;
                }
                else if (newSelectedIndex >= 0 && newSelectedIndex < groupTags.Length)
                {
                    _selectedLogTag = groupTags[newSelectedIndex];
                }

                GUILayout.Space(5);
            }

            GUILayout.BeginHorizontal();
            _gui.DrawButton("All On", () =>
            {
                logSetting.ExternSwitchOnAll();
                logSetting.Refresh();
            }, useWidth: true, useHeight: false);

            _gui.DrawButton("All Off", () =>
            {
                logSetting.ExternSwitchOffAll();
                logSetting.Refresh();
            }, useWidth: true, useHeight: false);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private (string title, LogTags[] tags)[] GetLogTagGroups()
        {
            return new (string title, LogTags[] tags)[]
            {
                ("Character", new LogTags[] { LogTags.Character, LogTags.Player, LogTags.Monster, LogTags.CharacterSpawn }),
                ("Character-Renderer", new LogTags[] { LogTags.Animation }),
                ("Character-Battle", new LogTags[] { LogTags.Attack, LogTags.BattleResource, LogTags.Damage, LogTags.Effect, LogTags.Stat, LogTags.Vital, LogTags.Buff }),
                ("Skill", new LogTags[] { LogTags.Skill }),
                ("Item", new LogTags[] { LogTags.Currency }),
                ("Game-Data", new LogTags[] { LogTags.GameData, LogTags.GameData_Stage, LogTags.GameData_Weapon, LogTags.GameData_Accessory }),
                ("Data", new LogTags[] { LogTags.GamePref, LogTags.JsonData, LogTags.Resource, LogTags.ScriptableData, LogTags.Path }),
                ("Setting", new LogTags[] { LogTags.Setting, LogTags.Audio, LogTags.Camera, LogTags.Global }),
                ("Input", new LogTags[] { LogTags.Input}),
                ("Stage", new LogTags[] { LogTags.Stage, LogTags.Scene }),
                ("Time", new LogTags[] { LogTags.Time }),
                ("MapObject", new LogTags[] { LogTags.PositionGroup }),
                ("String", new LogTags[] { LogTags.String, LogTags.Font }),
                ("UI", new LogTags[] { LogTags.UI, LogTags.UI_Button, LogTags.UI_Gauge, LogTags.UI_Toggle, LogTags.UI_Page, LogTags.UI_Notice, LogTags.UI_Popup, LogTags.UI_Details, LogTags.UI_Skill }),
                ("Timeline", new LogTags[] { LogTags.Timeline })
            };
        }

        private string GetLogTagDisplayName(LogTags tag)
        {
            return tag switch
            {
                LogTags.CharacterSpawn => "Spawn",
                LogTags.BattleResource => "Resource",
                LogTags.GameData_Stage => "Stage",
                LogTags.GameData_Weapon => "Weapon",
                LogTags.GameData_Accessory => "Accessory",
                LogTags.UI_Button => "UI_Btn",
                LogTags.UI_Gauge => "UI_Gauge",
                LogTags.UI_Toggle => "UI_Toggle",
                LogTags.UI_Page => "UI_Page",
                LogTags.UI_Notice => "UI_Notice",
                LogTags.UI_Popup => "UI_Popup",
                LogTags.UI_Details => "UI_Details",
                LogTags.UI_Skill => "UI_Skill",
                LogTags.GamePref => "GamePref",
                LogTags.JsonData => "JsonData",
                LogTags.ScriptableData => "Scriptable",
                LogTags.GameData => "GameData",
                _ => tag.ToString()
            };
        }

        private void DrawGamePlaySection()
        {
            GUILayout.BeginVertical("box");
            string title = JsonDataManager.FindStringClone("Option_GameSetting");
            if (string.IsNullOrEmpty(title))
            {
                title = "게임 플레이 설정";
            }
            _gui.DrawTitleLabel(title, useWidth: true, useHeight: true);

            if (GameSetting.Instance == null)
            {
                _gui.DrawContentLabel("GameSetting을 불러올 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            GamePlay play = GameSetting.Instance.Play;

            // 카메라 쉐이크
            string cameraShakeLabel = JsonDataManager.FindStringClone("Option_CameraShake");
            if (string.IsNullOrEmpty(cameraShakeLabel))
            {
                cameraShakeLabel = "카메라 쉐이크";
            }
            play.CameraShake = _gui.DrawContentToggleButton(cameraShakeLabel, play.CameraShake, useWidth: true, useHeight: true);

            // 피해량 텍스트
            string damageTextLabel = JsonDataManager.FindStringClone("Option_DamageText");
            if (string.IsNullOrEmpty(damageTextLabel))
            {
                damageTextLabel = "피해량 텍스트";
            }
            play.UseDamageText = _gui.DrawContentToggleButton(damageTextLabel, play.UseDamageText, useWidth: true, useHeight: true);

            GUILayout.EndVertical();
        }

        private void DrawCheatSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("치트 설정", useWidth: true, useHeight: true);

            if (GameSetting.Instance == null)
            {
                _gui.DrawContentLabel("GameSetting을 불러올 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            GameCheat cheat = GameSetting.Instance.Cheat;

            GUILayout.EndVertical();
        }

        private void DrawStatSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("플레이어 능력치", useWidth: true, useHeight: true);

            PlayerCharacter player = CharacterManager.Instance?.Player;
            if (player == null)
            {
                _gui.DrawContentLabel("플레이어 캐릭터를 찾을 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            StatSystem statSystem = player.Stat;
            if (statSystem == null)
            {
                _gui.DrawContentLabel("능력치 시스템을 찾을 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            // 모든 능력치 이름 가져오기
            StatNames[] allStatNames = Enum.GetValues(typeof(StatNames))
                .Cast<StatNames>()
                .Where(stat => stat != StatNames.None)
                .ToArray();

            if (allStatNames.Length == 0)
            {
                _gui.DrawContentLabel("표시할 능력치가 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            // 능력치별로 표시
            foreach (StatNames statName in allStatNames)
            {
                CharacterStat characterStat = statSystem.GetCharacterStat(statName);

                GUILayout.BeginVertical("box");

                if (characterStat != null)
                {
                    // 능력치 이름과 최종 값
                    float baseValue = characterStat.BaseValue;
                    int modifierCount = characterStat.ModifierCount;

                    string statDisplayName = GetStatDisplayName(statName);
                    string valueString = characterStat.ValueString;

                    GUILayout.BeginHorizontal();
                    _gui.DrawContentLabel($"{statDisplayName}: {valueString}");

                    if (modifierCount > 0)
                    {
                        _gui.DrawContentLabel($"(기본: {baseValue:F2}, Modifier: {modifierCount}개)");
                    }
                    else
                    {
                        _gui.DrawContentLabel($"(기본: {baseValue:F2})");
                    }
                    GUILayout.EndHorizontal();

                    // Modifier 상세 정보 표시
                    if (modifierCount > 0)
                    {
                        GUILayout.Space(3);
                        GUILayout.BeginVertical("box");
                        _gui.DrawContentLabel("Modifier:");

                        foreach (var modifier in characterStat.StatModifiers)
                        {
                            string modifierValueString = modifier.GetValueString();
                            string sourceString = modifier.GetSourceString();

                            if (string.IsNullOrEmpty(sourceString))
                            {
                                sourceString = "알 수 없음";
                            }
                            else
                            {
                                // 마지막 쉼표 제거
                                sourceString = sourceString.TrimEnd(',', ' ');
                            }

                            string modifierTypeString = GetModifierTypeString(modifier.Type);
                            _gui.DrawContentLabel($"  • {modifierTypeString}: {modifierValueString} ({sourceString})");
                        }

                        GUILayout.EndVertical();
                    }
                }
                else
                {
                    // 능력치가 등록되지 않은 경우 기본값 표시
                    float defaultValue = statSystem.FindValueOrDefault(statName);
                    string statDisplayName = GetStatDisplayName(statName);
                    _gui.DrawContentLabel($"{statDisplayName}: {defaultValue:F2} (기본값)");
                }

                GUILayout.EndVertical();
                GUILayout.Space(3);
            }

            GUILayout.EndVertical();
        }

        private string GetStatDisplayName(StatNames statName)
        {
            return statName switch
            {
                StatNames.Attack => "공격력",
                StatNames.Health => "최대 체력",
                StatNames.HealthRegen => "체력 회복량",
                StatNames.AttackSpeed => "공격 속도(%)",
                StatNames.CriticalChance => "치명타 확률(%)",
                StatNames.CriticalDamage => "치명타 피해(%)",
                StatNames.Mana => "마나",
                StatNames.ManaRegen => "마나 회복량",
                StatNames.GoldGain => "추가 골드 획득량(%)",
                StatNames.XPGain => "추가 경험치(%)",
                StatNames.DamageReduction => "피해 감소(%)",
                StatNames.DevastatingStrike => "회심의 일격(%)",
                StatNames.DevastatingStrikeChance => "회심의 일격 확률(%)",
                StatNames.Accuracy => "명중",
                StatNames.Dodge => "회피",
                StatNames.Shield => "보호막",
                StatNames.ShieldMulti => "보호막 배율",
                StatNames.None => "없음",
                _ => statName.ToString()
            };
        }

        private string GetModifierTypeString(StatModType modType)
        {
            return modType switch
            {
                StatModType.Flat => "고정값",
                StatModType.PercentAdd => "퍼센트 추가",
                StatModType.PercentMulti => "퍼센트 배율",
                StatModType.Use => "사용",
                _ => modType.ToString()
            };
        }

        private void DrawGameDataSection()
        {
            GameDataManager dataManager = GetGameDataManager();

            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("저장/로드", useWidth: true, useHeight: true);

            if (dataManager == null)
            {
                _gui.DrawContentLabel("GameDataManager 인스턴스를 찾을 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginHorizontal();
            _gui.DrawButton("게임 데이터 저장", () =>
            {
                dataManager.Save();
                Debug.Log("게임 데이터를 저장했습니다.");
            }, useWidth: true, useHeight: false);
            _gui.DrawButton("게임 데이터 로드", () =>
            {
                dataManager.LoadGameDataWithRecovery();
                Debug.Log("게임 데이터를 로드했습니다.");
            }, useWidth: false);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("백업 복구", useWidth: true, useHeight: true);

            _gui.DrawButton("백업 파일에서 복구", () =>
            {
                bool success = dataManager.TryLoadFromBackup();
                if (!success)
                {
                    Debug.LogWarning("백업 파일에서 복구에 실패했습니다.");
                }
            }, useWidth: false);

            _gui.DrawButton("모든 파일에서 복구 시도", () =>
            {
                bool success = dataManager.TryLoadFromAnyAvailableFile();
                if (!success)
                {
                    Debug.LogWarning("모든 파일에서 복구에 실패했습니다.");
                }
            }, useWidth: false);

            _gui.DrawButton("가장 최근 백업으로 복구", () =>
            {
                bool success = dataManager.RestoreFromBackup();
                if (!success)
                {
                    Debug.LogWarning("백업 복구에 실패했습니다.");
                }
            }, useWidth: false);

            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("진단/분석", useWidth: true, useHeight: true);

            _gui.DrawButton("세이브 파일 상태 확인", () =>
            {
                dataManager.LogAllSaveFileStatus();
            }, useWidth: false);

            _gui.DrawButton("백업 파일 정보 출력", () =>
            {
                dataManager.LogBackupFileInfo();
            }, useWidth: false);

            _gui.DrawButton("모든 세이브 파일 진단", () =>
            {
                dataManager.DiagnoseAllSaveFiles();
            }, useWidth: false);

            _gui.DrawButton("마이그레이션 상태 점검", () =>
            {
                dataManager.CheckAllSaveFilesMigrationStatus();
            }, useWidth: false);

            _gui.DrawButton("세이브 파일 통계 출력", () =>
            {
                dataManager.PrintSaveFileStatistics();
            }, useWidth: false);

            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("파일 관리", useWidth: true, useHeight: true);

            _gui.DrawButton("에디터용 세이브 파일 삭제", () =>
            {
                GameDataManager.DeleteSaveFileForEditor();
                Debug.Log("에디터용 세이브 파일을 삭제했습니다.");
            }, useWidth: false);

            GUILayout.EndVertical();
            GUILayout.Space(5);

            DrawOwnedWeaponAccessorySkillSection();
        }

        private void DrawOwnedWeaponAccessorySkillSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("보유 무기 / 악세사리 / 스킬", useWidth: true, useHeight: true);

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                _gui.DrawContentLabel("프로필을 불러올 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            // 무기
            _gui.DrawTitleLabel("무기", useWidth: false, useHeight: true);
            if (profile.Weapon?.Weapons != null && profile.Weapon.Weapons.Count > 0)
            {
                string equippedKey = profile.Weapon.EquippedWeaponName.ToString();
                foreach (var kvp in profile.Weapon.Weapons)
                {
                    string name = kvp.Key;
                    VWeapon w = kvp.Value;
                    bool equipped = name == equippedKey;
                    string suffix = equipped ? " (장착)" : "";
                    _gui.DrawContentLabel($"  • {name} Lv.{w.Level}{suffix}");
                }
            }
            else
            {
                _gui.DrawContentLabel("  보유 무기가 없습니다.");
            }

            GUILayout.Space(3);

            // 악세사리
            _gui.DrawTitleLabel("악세사리", useWidth: false, useHeight: true);
            if (profile.Accessory?.Accessories != null && profile.Accessory.Accessories.Count > 0)
            {
                string equippedKey = profile.Accessory.EquippedAccessoryName.ToString();
                foreach (var kvp in profile.Accessory.Accessories)
                {
                    string name = kvp.Key;
                    VAccessory a = kvp.Value;
                    bool equipped = name == equippedKey;
                    string suffix = equipped ? " (장착)" : "";
                    _gui.DrawContentLabel($"  • {name} Lv.{a.Level}{suffix}");
                }
            }
            else
            {
                _gui.DrawContentLabel("  보유 악세사리가 없습니다.");
            }

            GUILayout.Space(3);

            // 스킬
            _gui.DrawTitleLabel("스킬", useWidth: false, useHeight: true);
            if (profile.Skill?.Skills != null && profile.Skill.Skills.Count > 0)
            {
                foreach (var kvp in profile.Skill.Skills)
                {
                    string name = kvp.Key;
                    VSkill s = kvp.Value;
                    _gui.DrawContentLabel($"  • {name} Lv.{s.Level}");
                }
            }
            else
            {
                _gui.DrawContentLabel("  보유 스킬이 없습니다.");
            }

            GUILayout.EndVertical();
        }

        private GameDataManager GetGameDataManager()
        {
            var gameApp = GameApp.Instance;
            if (gameApp != null && gameApp.dataManager != null)
            {
                return gameApp.dataManager;
            }
            return null;
        }
    }
}