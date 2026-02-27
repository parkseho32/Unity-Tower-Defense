using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Tilemap buildTilemap;
    [SerializeField] private Tilemap pathTilemap;
    [SerializeField] private Economy economy;

    private readonly Dictionary<Vector3Int, GameObject> placedTowers = new();

    public Economy Economy => economy; // UI에서 쓰기 편하게 공개

    public bool CanBuild(Vector3Int cellPos, TowerData data)
    {
        if (data == null || data.prefab == null) return false;
        if (!buildTilemap.HasTile(cellPos)) return false;
        if (pathTilemap != null && pathTilemap.HasTile(cellPos)) return false;
        if (placedTowers.ContainsKey(cellPos)) return false;
        if (!economy.CanSpend(data.cost)) return false;
        return true;
    }

    public bool TryBuild(Vector3Int cellPos, TowerData data)
    {
        if (!CanBuild(cellPos, data)) return false;

        economy.TrySpend(data.cost);

        Vector3 worldCenter = buildTilemap.GetCellCenterWorld(cellPos);
        GameObject towerGo = Instantiate(data.prefab, worldCenter, Quaternion.identity);

        // 설치된 타워에 TowerInstance가 있으면 초기화
        if (towerGo.TryGetComponent(out TowerInstance inst))
        {
            inst.Init(data);
        }

        placedTowers.Add(cellPos, towerGo);
        return true;
    }

    // 타워 오브젝트로부터 그 타워가 설치된 셀을 찾기(판매/점유 해제용)
    public bool TryGetCellByTower(GameObject towerGo, out Vector3Int cellPos)
    {
        foreach (var kv in placedTowers)
        {
            if (kv.Value == towerGo)
            {
                cellPos = kv.Key;
                return true;
            }
        }

        cellPos = default;
        return false;
    }

    // 점유 해제(판매/파괴 시 호출)
    public void UnregisterTower(GameObject towerGo)
    {
        if (TryGetCellByTower(towerGo, out Vector3Int cellPos))
        {
            placedTowers.Remove(cellPos);
        }
    }
}