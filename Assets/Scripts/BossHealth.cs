using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour {
    [Header("보스 HP 설정")]
    [SerializeField] private float maxHP = 1000f;
    [SerializeField] private float currentHP;
    
    private bool isDead = false;
    
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public float HPPercentage => maxHP > 0 ? currentHP / maxHP : 0f;
    public bool IsDead => isDead;
    
    private void Awake() {
        currentHP = maxHP;
    }
    
    public void TakeDamage(float damage) {
        if (isDead) return;
        
        currentHP = Mathf.Max(0, currentHP - damage);
        
        // HP가 0 이하가 되면 사망
        if (currentHP <= 0 && !isDead) {
            Die();
        }
    }
    
    public void Heal(float healAmount) {
        if (isDead) return;
        
        currentHP = Mathf.Min(maxHP, currentHP + healAmount);
    }
    
    public void SetMaxHP(float newMaxHP) {
        if (newMaxHP <= 0) return;
        
        float ratio = currentHP / maxHP;
        maxHP = newMaxHP;
        currentHP = maxHP * ratio;
    }
    
    public void RestoreFullHP() {
        currentHP = maxHP;
    }
    
    public float GetCurrentHP() {
        return currentHP;
    }
    
    private void Die() {
        isDead = true;
        Debug.Log("보스가 사망했습니다!");
    }
    
    // 에디터에서 값 변경 시 자동 업데이트
    private void OnValidate() {
        if (Application.isPlaying) {
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        }
    }
}
