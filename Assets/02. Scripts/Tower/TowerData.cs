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
}