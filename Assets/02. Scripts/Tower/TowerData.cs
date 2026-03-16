using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("UI")]
    public string displayName;    // UI에 표시할 타워 이름
    public Sprite icon;           // UI 아이콘(없어도 됨)

    [Header("Build")]
    public int cost;              // 설치 비용
    public GameObject prefab;     // 설치할 타워 프리팹

    [Header("OnHit Effect")]
    public bool applySlow;        // 맞추면 슬로우 적용 여부
    [Range(0.1f, 1f)]
    public float slowMultiplier = 0.5f;   // 속도 배율
    public float slowDuration = 2f;       // 지속시간

    [Header("Base Stats")]
    public int baseDamage = 2;
    public float baseRange = 3.5f;
    public float baseFireRate = 0.8f;
}