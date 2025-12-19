using System.Collections;
using TeamSuneat.Assets.Scripts.Runtime.Stage;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public class GameManager : MonoBehaviour
    {
        public bool IsBattleActive { get; internal set; }
        public StageSystem CurrentStageSystem { get; set; }

        private void Awake()
        {
            CharacterManager.Instance.Reset();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => { return GameApp.Instance != null; });
            InitializeGameplayTimeTracking();
        }

        private void InitializeGameplayTimeTracking()
        {
            // 게임플레이 시간 추적 초기화
            GameTimeManager.Instance.InitializeGameplayTracking(false);
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
            GameTimeManager.Instance.UpdateTimeTracking();
            CharacterManager.Instance.LogicUpdate();

            // UIManager.Instance?.LogicUpdate();
        }

        private void LateUpdate()
        {
            // UIManager.Instance?.LateLogicUpdate();
            // PassiveManager.Instance.LateLogicUpdate();
        }

        private void FixedUpdate()
        {
            CharacterManager.Instance.PhysicsUpdate();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) { GameTimeManager.Instance.Pause(); }
            else { GameTimeManager.Instance.Resume(); }
        }

        internal void ResetStage()
        {
        }
    }
}