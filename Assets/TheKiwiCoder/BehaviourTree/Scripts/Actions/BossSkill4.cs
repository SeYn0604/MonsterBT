using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TheKiwiCoder;

public class BossSkill4 : ActionNode
{
    [Header("스킬 설정")]
    public float skillDuration = 2.0f;
    
    [Header("스킬 정보")]
    [SerializeField] private string skillName = "스킬 4";
    
    [Header("스킬 상태")]
    [SerializeField] private bool isCurrentlyCasting = false;
    
    [Header("스킬 이벤트")]
    [SerializeField] public UnityEvent onSkillStart;
    [SerializeField] public UnityEvent onSkillEnd;
    
    private float skillStartTime;
    
    protected override void OnStart()
    {
        isCurrentlyCasting = true;
        skillStartTime = Time.time;
        
        if (onSkillStart != null)
        {
            onSkillStart.Invoke();
        }
        
        Debug.Log($"BossSkill4: {skillName} 시작");
    }
    
    protected override void OnStop()
    {
        isCurrentlyCasting = false;
        
        if (onSkillEnd != null)
        {
            onSkillEnd.Invoke();
        }
        
        Debug.Log($"BossSkill4: {skillName} 종료");
    }
    
    protected override State OnUpdate()
    {
        if (!isCurrentlyCasting)
        {
            return State.Failure;
        }
        
        float elapsedTime = Time.time - skillStartTime;
        
        if (elapsedTime >= skillDuration)
        {
            return State.Success;
        }
        
        // 스킬 실행 중
        return State.Running;
    }
    
    public bool CanUse()
    {
        return !isCurrentlyCasting;
    }
    
    public void ResetSkill()
    {
        isCurrentlyCasting = false;
    }
}
