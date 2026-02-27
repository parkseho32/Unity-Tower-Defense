using System;
using UnityEngine;

public class Economy : MonoBehaviour
{
    [SerializeField] private int startGold = 100; // 시작 골드
    public int Gold { get; private set; }         // 현재 골드 (읽기 전용)

    public event Action<int> OnGoldChanged;       // 골드가 바뀔 때 UI에 알려주는 이벤트

    private void Awake()
    {
        Gold = startGold;                 // 시작 골드 적용
        OnGoldChanged?.Invoke(Gold);      // 시작 시 UI도 갱신되도록 1회 호출
    }

    public bool CanSpend(int amount)
    {
        return Gold >= amount;            // 골드가 충분한지 확인
    }

    public bool TrySpend(int amount)
    {
        if (!CanSpend(amount)) return false; // 부족하면 실패

        Gold -= amount;                      // 차감
        OnGoldChanged?.Invoke(Gold);         // UI 갱신 알림
        return true;
    }

    public void Add(int amount)
    {
        Gold += amount;                   // 획득
        OnGoldChanged?.Invoke(Gold);      // UI 갱신 알림
    }
}