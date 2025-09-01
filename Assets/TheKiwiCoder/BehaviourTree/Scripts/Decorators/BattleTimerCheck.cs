using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class BattleTimerCheck : DecoratorNode {
        [Header("타이머 설정")]
        [Tooltip("전투 제한 시간 (초)")]
        public float battleTimeLimit = 300f; // 5분
        
        [Tooltip("시간 체크 주기 (초)")]
        public float checkInterval = 1.0f;
        
        [Tooltip("시간이 다 되었을 때 인터럽트할지 여부")]
        public bool interruptOnTimeUp = true;
        
        [Header("시간 표시")]
        [Tooltip("콘솔에 남은 시간을 표시할지 여부")]
        public bool showTimeInConsole = true;
        
        private float battleStartTime;
        private float lastCheckTime;
        private bool isBattleStarted = false;
        
        protected override void OnStart() {
            // 전투 시작 시간 기록
            if (!isBattleStarted) {
                battleStartTime = Time.time;
                isBattleStarted = true;
                Debug.Log($"전투 타이머 시작: {battleTimeLimit}초 제한");
            }
            
            lastCheckTime = Time.time;
        }
        
        protected override void OnStop() {
            // 전투 종료 시 타이머 리셋
            if (isBattleStarted) {
                isBattleStarted = false;
                Debug.Log("전투 타이머 종료");
            }
        }
        
        protected override State OnUpdate() {
            // 체크 주기 확인
            if (Time.time - lastCheckTime < checkInterval) {
                return child.Update();
            }
            
            lastCheckTime = Time.time;
            
            // 경과 시간 계산
            float elapsedTime = Time.time - battleStartTime;
            float remainingTime = battleTimeLimit - elapsedTime;
            
            // 시간이 다 되었는지 확인
            if (remainingTime <= 0) {
                if (showTimeInConsole) {
                    Debug.LogWarning("전투 시간 초과! 인터럽트 발생");
                }
                
                // 시간 초과 시 인터럽트를 위해 Failure 반환
                if (interruptOnTimeUp) {
                    return State.Failure;
                }
            }
            
            // 남은 시간 표시 (콘솔)
            if (showTimeInConsole && remainingTime > 0) {
                if (remainingTime <= 60f) { // 1분 이하일 때는 매초 표시
                    Debug.Log($"전투 남은 시간: {remainingTime:F1}초");
                } else if (remainingTime <= 300f && (int)remainingTime % 30 == 0) { // 5분 이하일 때는 30초마다 표시
                    Debug.Log($"전투 남은 시간: {remainingTime:F0}초");
                } else if ((int)remainingTime % 60 == 0) { // 1분마다 표시
                    Debug.Log($"전투 남은 시간: {remainingTime / 60:F0}분");
                }
            }
            
            // 정상적인 경우 자식 노드 실행
            return child.Update();
        }
        
        // 외부에서 전투 시작 시간 리셋
        public void ResetBattleTimer() {
            battleStartTime = Time.time;
            isBattleStarted = true;
            Debug.Log("전투 타이머 리셋");
        }
        
        // 외부에서 남은 시간 확인
        public float GetRemainingTime() {
            if (!isBattleStarted) return battleTimeLimit;
            
            float elapsedTime = Time.time - battleStartTime;
            return Mathf.Max(0, battleTimeLimit - elapsedTime);
        }
        
        // 외부에서 경과 시간 확인
        public float GetElapsedTime() {
            if (!isBattleStarted) return 0f;
            
            return Time.time - battleStartTime;
        }
        
        // 외부에서 전투 진행률 확인 (0.0 ~ 1.0)
        public float GetBattleProgress() {
            if (!isBattleStarted) return 0f;
            
            float elapsedTime = Time.time - battleStartTime;
            return Mathf.Clamp01(elapsedTime / battleTimeLimit);
        }
        
        // 외부에서 전투 상태 확인
        public bool IsBattleInProgress() {
            return isBattleStarted;
        }
        
        // 외부에서 전투 시간 연장
        public void ExtendBattleTime(float additionalTime) {
            if (isBattleStarted) {
                battleTimeLimit += additionalTime;
                Debug.Log($"전투 시간 연장: +{additionalTime}초 (총 {battleTimeLimit}초)");
            }
        }
        
        // 외부에서 전투 시간 단축
        public void ReduceBattleTime(float reducedTime) {
            if (isBattleStarted) {
                battleTimeLimit = Mathf.Max(0, battleTimeLimit - reducedTime);
                Debug.Log($"전투 시간 단축: -{reducedTime}초 (총 {battleTimeLimit}초)");
            }
        }
    }
}
