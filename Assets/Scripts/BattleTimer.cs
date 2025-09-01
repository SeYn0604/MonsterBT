using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTimer : MonoBehaviour
{
    [Header("전투 시간 설정")]
    [SerializeField] private float battleTimeLimit = 300f; // 5분
    
    [Header("전투 상태")]
    [SerializeField] private bool isBattleStarted = false;
    [SerializeField] private float battleStartTime;
    [SerializeField] private float currentBattleTime;
    
    [Header("디버그 정보")]
    [SerializeField] private bool showTimeInConsole = true;
    
    private void Start()
    {
        // 전투 시작
        StartBattle();
    }
    
    private void Update()
    {
        if (isBattleStarted)
        {
            currentBattleTime = Time.time - battleStartTime;
            
            if (showTimeInConsole)
            {
                float remainingTime = battleTimeLimit - currentBattleTime;
                if (remainingTime > 0)
                {
                    Debug.Log($"전투 시간: {currentBattleTime:F1}초 / 남은 시간: {remainingTime:F1}초");
                }
                else
                {
                    Debug.Log("전투 시간 초과!");
                }
            }
        }
    }
    
    public void StartBattle()
    {
        if (!isBattleStarted)
        {
            battleStartTime = Time.time;
            isBattleStarted = true;
            currentBattleTime = 0f;
            Debug.Log("전투 시작!");
        }
    }
    
    public void EndBattle()
    {
        isBattleStarted = false;
        Debug.Log($"전투 종료! 총 전투 시간: {currentBattleTime:F1}초");
    }
    
    public void ResetBattleTimer()
    {
        isBattleStarted = false;
        currentBattleTime = 0f;
        Debug.Log("전투 타이머 리셋!");
    }
    
    // 현재 전투 시간 가져오기
    public float GetCurrentBattleTime()
    {
        if (isBattleStarted)
        {
            return Time.time - battleStartTime;
        }
        return 0f;
    }
    
    // 남은 시간 가져오기
    public float GetRemainingTime()
    {
        if (isBattleStarted)
        {
            return Mathf.Max(0f, battleTimeLimit - GetCurrentBattleTime());
        }
        return battleTimeLimit;
    }
    
    // 전투 시간 초과 여부 확인
    public bool IsTimeUp()
    {
        return GetCurrentBattleTime() >= battleTimeLimit;
    }
    
    // 전투 진행 중인지 확인
    public bool IsBattleActive()
    {
        return isBattleStarted;
    }
}
