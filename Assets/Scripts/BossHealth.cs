using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour {
    [Header("보스 HP 설정")]
    [SerializeField] private float maxHP = 1000f;
    [SerializeField] private float currentHP;
    
    [Header("페이즈 설정")]
    [SerializeField] private BossPhase[] phases;
    [SerializeField] private int currentPhase = 0;
    
    [Header("이벤트")]
    [SerializeField] private UnityEvent<float> onHPChanged;
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent onDamaged;
    [SerializeField] private UnityEvent<int> onPhaseChanged;
    
    [Header("UI 표시")]
    [SerializeField] private bool showHealthBar = true;
    [SerializeField] private GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    
    private bool isDead = false;
    
    [System.Serializable]
    public class BossPhase {
        public string phaseName = "Phase 1";
        public float hpThreshold = 0.8f; // 이 HP 비율 이하일 때 다음 페이즈
        public UnityEvent onPhaseStart;
        public UnityEvent onPhaseEnd;
    }
    
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public float HPPercentage => maxHP > 0 ? currentHP / maxHP : 0f;
    public bool IsDead => isDead;
    public int CurrentPhase => currentPhase;
    
    private void Awake() {
        currentHP = maxHP;
        
        // 페이즈가 설정되지 않은 경우 기본값 생성
        if (phases == null || phases.Length == 0) {
            CreateDefaultPhases();
        }
    }
    
    private void Start() {
        if (showHealthBar && healthBarPrefab != null) {
            CreateHealthBar();
        }
        
        // 초기 HP 이벤트 발생
        onHPChanged?.Invoke(HPPercentage);
        
        // 첫 번째 페이즈 시작
        if (phases.Length > 0) {
            phases[0].onPhaseStart?.Invoke();
        }
    }
    
    public void TakeDamage(float damage) {
        if (isDead) return;
        
        currentHP = Mathf.Max(0, currentHP - damage);
        
        // 데미지 이벤트 발생
        onDamaged?.Invoke();
        onHPChanged?.Invoke(HPPercentage);
        
        // 페이즈 체크
        CheckPhaseTransition();
        
        // HP가 0 이하가 되면 사망
        if (currentHP <= 0 && !isDead) {
            Die();
        }
        
        // HP 바 업데이트
        UpdateHealthBar();
    }
    
    public void Heal(float healAmount) {
        if (isDead) return;
        
        float oldHP = currentHP;
        currentHP = Mathf.Min(maxHP, currentHP + healAmount);
        
        // 힐링이 실제로 이루어진 경우에만 이벤트 발생
        if (currentHP > oldHP) {
            onHPChanged?.Invoke(HPPercentage);
            UpdateHealthBar();
        }
    }
    
    public void SetMaxHP(float newMaxHP) {
        if (newMaxHP <= 0) return;
        
        float ratio = currentHP / maxHP;
        maxHP = newMaxHP;
        currentHP = maxHP * ratio;
        
        onHPChanged?.Invoke(HPPercentage);
        UpdateHealthBar();
    }
    
    public void RestoreFullHP() {
        currentHP = maxHP;
        onHPChanged?.Invoke(HPPercentage);
        UpdateHealthBar();
    }
    
    public float GetCurrentHP() {
        return currentHP;
    }
    
    private void CheckPhaseTransition() {
        if (currentPhase >= phases.Length - 1) return;
        
        float hpPercentage = HPPercentage;
        float nextPhaseThreshold = phases[currentPhase + 1].hpThreshold;
        
        if (hpPercentage <= nextPhaseThreshold) {
            // 현재 페이즈 종료
            phases[currentPhase].onPhaseEnd?.Invoke();
            
            // 다음 페이즈로 이동
            currentPhase++;
            
            // 페이즈 변경 이벤트 발생
            onPhaseChanged?.Invoke(currentPhase);
            
            // 새 페이즈 시작
            phases[currentPhase].onPhaseStart?.Invoke();
            
            Debug.Log($"보스 페이즈 변경: {phases[currentPhase].phaseName}");
        }
    }
    
    private void CreateDefaultPhases() {
        phases = new BossPhase[3];
        
        // 페이즈 1 (100% ~ 80%)
        phases[0] = new BossPhase {
            phaseName = "Phase 1",
            hpThreshold = 0.8f
        };
        
        // 페이즈 2 (80% ~ 30%)
        phases[1] = new BossPhase {
            phaseName = "Phase 2",
            hpThreshold = 0.3f
        };
        
        // 페이즈 3 (30% ~ 0%)
        phases[2] = new BossPhase {
            phaseName = "Phase 3",
            hpThreshold = 0.0f
        };
    }
    
    private void Die() {
        isDead = true;
        onDeath?.Invoke();
        
        // HP 바 숨기기
        if (healthBarInstance != null) {
            healthBarInstance.SetActive(false);
        }
        
        // 보스 사망 처리 (필요에 따라 수정)
        // gameObject.SetActive(false);
    }
    
    private void CreateHealthBar() {
        if (healthBarPrefab != null) {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + Vector3.up * 3f, Quaternion.identity);
            healthBarInstance.transform.SetParent(transform);
        }
    }
    
    private void UpdateHealthBar() {
        if (healthBarInstance != null) {
            // HP 바의 fill amount를 업데이트하는 로직
            var healthBarFill = healthBarInstance.GetComponentInChildren<UnityEngine.UI.Image>();
            if (healthBarFill != null) {
                healthBarFill.fillAmount = HPPercentage;
            }
        }
    }
    
    // 디버그용 HP 표시
    private void OnGUI() {
        if (showHealthBar && healthBarInstance == null) {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 3f);
            if (screenPos.z > 0) {
                GUI.Label(new Rect(screenPos.x - 75, Screen.height - screenPos.y, 150, 20), 
                         $"보스 HP: {currentHP:F0}/{maxHP:F0} ({HPPercentage:P0})");
                GUI.Label(new Rect(screenPos.x - 75, Screen.height - screenPos.y + 20, 150, 20), 
                         $"페이즈: {phases[currentPhase].phaseName}");
            }
        }
    }
    
    // 에디터에서 값 변경 시 자동 업데이트
    private void OnValidate() {
        if (Application.isPlaying) {
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            onHPChanged?.Invoke(HPPercentage);
        }
    }
}
