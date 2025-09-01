using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class BossHPCheck : DecoratorNode {
        [Tooltip("보스의 최대 HP")]
        public float maxHP = 1000f;
        
        [Tooltip("HP 체크 임계값 (이 값 이하일 때 인터럽트)")]
        public float hpThreshold = 0.3f;
        
        [Tooltip("HP 체크 주기 (초)")]
        public float checkInterval = 0.5f;
        
        private float lastCheckTime;
        private BossHealth bossHealth;

        protected override void OnStart() {
            lastCheckTime = Time.time;
            
            // 보스의 HP 컴포넌트를 찾습니다
            if (bossHealth == null) {
                bossHealth = context.gameObject.GetComponent<BossHealth>();
                if (bossHealth == null) {
                    Debug.LogWarning("BossHPCheck: BossHealth 컴포넌트를 찾을 수 없습니다!");
                }
            }
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            // 체크 주기 확인
            if (Time.time - lastCheckTime < checkInterval) {
                return child.Update();
            }
            
            lastCheckTime = Time.time;
            
            // 보스 HP 체크
            if (bossHealth != null) {
                float currentHP = bossHealth.GetCurrentHP();
                float hpPercentage = currentHP / maxHP;
                
                // HP가 임계값 이하일 때 인터럽트
                if (hpPercentage <= hpThreshold) {
                    return State.Failure; // 인터럽트를 위해 Failure 반환
                }
            }
            
            // 정상적인 경우 자식 노드 실행
            return child.Update();
        }
    }
}
