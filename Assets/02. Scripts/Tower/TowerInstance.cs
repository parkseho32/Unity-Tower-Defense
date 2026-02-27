using UnityEngine;

public class TowerInstance : MonoBehaviour
{
    [Header("Runtime State")]
    [SerializeField] private int level = 1; // 현재 레벨(인스펙터 디버그용)
    public int Level => level;              // 외부에서 읽기 전용

    [Header("Stats")]
    [SerializeField] private int damage = 2;   // 현재 데미지
    [SerializeField] private float range = 3.5f; // 현재 사거리
    public int Damage => damage;               // 읽기 전용
    public float Range => range;               // 읽기 전용

    [Header("Source Data")]
    [SerializeField] private TowerData data;   // 어떤 TowerData로 설치됐는지(저장)
    public TowerData Data => data;             // 읽기 전용

    // BuildManager가 설치 직후 호출해서 데이터/스탯 초기화
    public void Init(TowerData towerData, int baseDamage = 2, float baseRange = 3.5f)
    {
        data = towerData;   // 데이터 저장
        level = 1;          // 레벨 초기화
        damage = baseDamage; // 기본 스탯 세팅
        range = baseRange;
    }

    // 업그레이드 비용: 설치비의 50% * 현재 레벨 (간단 규칙)
    public int GetUpgradeCost()
    {
        if (data == null) return 0;
        return Mathf.Max(1, Mathf.RoundToInt(data.cost * 0.5f * level));
    }

    // 판매 가격: 설치비의 50% 고정
    public int GetSellValue()
    {
        if (data == null) return 0;
        return Mathf.RoundToInt(data.cost * 0.5f);
    }

    // 업그레이드 적용: 데미지 +1, 사거리 +0.5
    public void ApplyUpgrade()
    {
        level += 1;
        damage += 1;
        range += 0.5f;
    }
}