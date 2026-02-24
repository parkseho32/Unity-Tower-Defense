using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Tilemap buildTilemap; // 설치 가능 영역 타일맵(BuildTilemap)
    [SerializeField] private Economy economy;      // 골드 관리자

    // 어떤 셀에 타워가 설치되었는지 저장 (중복 설치 방지)
    private readonly Dictionary<Vector3Int, GameObject> placedTowers = new();

    // BuildInput이 호출: 특정 셀에 특정 타워를 설치 시도
    public bool TryBuild(Vector3Int cellPos, TowerData data)
    {
        if (data == null) return false;                  // 선택 타워가 없으면 실패
        if (data.prefab == null) return false;           // 프리팹이 없으면 실패

        // 1) 그 셀이 BuildTilemap 위인지 확인 (타일이 없으면 설치 불가)
        if (!buildTilemap.HasTile(cellPos)) return false;

        // 2) 이미 타워가 설치된 셀이면 설치 불가
        if (placedTowers.ContainsKey(cellPos)) return false;

        // 3) 돈 충분한지 확인 후 차감
        if (!economy.TrySpend(data.cost)) return false;

        // 4) 셀 중앙 월드 좌표에 타워 생성
        Vector3 worldCenter = buildTilemap.GetCellCenterWorld(cellPos);
        GameObject tower = Instantiate(data.prefab, worldCenter, Quaternion.identity);

        // 5) 점유 등록
        placedTowers.Add(cellPos, tower);

        return true;
    }
}