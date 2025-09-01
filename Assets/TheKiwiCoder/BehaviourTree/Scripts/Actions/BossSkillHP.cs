using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TheKiwiCoder;

public class BossSkillHP : ActionNode
{
    [Header("스킬 설정")]
    public float skillDuration = 2.0f;
    
    [Header("스킬 정보")]
    [SerializeField] private string skillName = "HP 스킬";
    
    [Header("스킬 상태")]
    [SerializeField] private bool isUsed = false;
    [SerializeField] private bool isCurrentlyCasting = false;
    
    [Header("스킬 이벤트")]
    [SerializeField] public UnityEvent onSkillStart;
    [SerializeField] public UnityEvent onSkillEnd;
    
    private float skillStartTime;
    
    protected override void OnStart()
    {
        if (isUsed)
        {
            Debug.LogWarning($"BossSkillHP: {skillName}은 이미 사용되었습니다.");
            return;
        }
        
        isCurrentlyCasting = true;
        skillStartTime = Time.time;
        
        if (onSkillStart != null)
        {
            onSkillStart.Invoke();
        }
        
        Debug.Log($"BossSkillHP: {skillName} 시작");
    }
    
    protected override void OnStop()
    {
        isCurrentlyCasting = false;
        
        if (onSkillEnd != null)
        {
            onSkillEnd.Invoke();
        }
        
        Debug.Log($"BossSkillHP: {skillName} 종료");
    }
    
    protected override State OnUpdate()
    {
        if (isUsed || !isCurrentlyCasting)
        {
            return State.Failure;
        }
        
        float elapsedTime = Time.time - skillStartTime;
        
        if (elapsedTime >= skillDuration)
        {
            isUsed = true;
            return State.Success;
        }
        
        // 스킬 실행 중
        return State.Running;
    }
    
    public void ResetForNewBattle()
    {
        isUsed = false;
        isCurrentlyCasting = false;
    }
    
    public bool CanUse()
    {
        return !isUsed && !isCurrentlyCasting;
    }
}
