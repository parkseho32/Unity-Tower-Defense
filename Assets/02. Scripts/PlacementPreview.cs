using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementPreview : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Tilemap buildTilemap;
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private BuildInput buildInput;

    [Header("Preview")]
    [SerializeField] private SpriteRenderer sr;

    private TowerData lastTower; // 마지막으로 적용한 타워(변경 감지용)

    private void Awake()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (sr == null) sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // ✅ 우클릭: 선택 취소(프리뷰 숨김)
        if (Input.GetMouseButtonDown(1))
        {
            buildInput.ClearSelection();
        }

        TowerData selected = buildInput.SelectedTower;
        if (selected == null)
        {
            if (sr.enabled) sr.enabled = false;
            lastTower = null;
            return;
        }

        if (!sr.enabled) sr.enabled = true;

        // ✅ 선택 타워가 바뀌면 프리뷰 스프라이트도 변경
        if (selected != lastTower)
        {
            ApplyPreviewSpriteFromPrefab(selected);
            lastTower = selected;
        }

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = buildTilemap.WorldToCell(mouseWorld);
        transform.position = buildTilemap.GetCellCenterWorld(cellPos);

        bool canBuild = buildManager.CanBuild(cellPos, selected);
        sr.color = canBuild ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f);
    }

    private void ApplyPreviewSpriteFromPrefab(TowerData data)
    {
        // 타워 프리팹에 SpriteRenderer가 있으면 그 스프라이트를 프리뷰에 적용
        SpriteRenderer towerSr = data.prefab.GetComponentInChildren<SpriteRenderer>();
        if (towerSr != null)
        {
            sr.sprite = towerSr.sprite; // 프리뷰 스프라이트 변경
        }
        // 없으면 기존 sr.sprite 유지(임시 Square 그대로)
    }
}