using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildInput : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera mainCam;         // 메인 카메라
    [SerializeField] private Tilemap buildTilemap;   // BuildTilemap
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private LifeManager lifeManager;

    [Header("State")]
    public TowerData SelectedTower;                  // UIBuildPanel이 선택해주는 타워 데이터

    private void Awake()
    {
        // 인스펙터 연결 실수 대비(카메라 자동 지정)
        if (mainCam == null) mainCam = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;    // 좌클릭만 처리
        if (SelectedTower == null) return;           // 선택된 타워가 없으면 설치 안 함
        if (lifeManager != null && lifeManager.IsGameOver) return; 

        // 화면 좌표 → 월드 좌표 변환
        Vector3 world = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표 → 셀 좌표 변환
        Vector3Int cellPos = buildTilemap.WorldToCell(world);

        // 설치 시도
        buildManager.TryBuild(cellPos, SelectedTower);
    }
    
    public void ClearSelection()
    {
        SelectedTower = null;    // 선택 타워를 비워서 프리뷰 숨기기
    }
}