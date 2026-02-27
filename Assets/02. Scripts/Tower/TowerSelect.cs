using UnityEngine;

public class TowerSelect : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private LayerMask towerLayer;          // Tower 레이어만 클릭되게
    [SerializeField] private UITowerActions uiTowerActions; // 선택 UI

    private void Awake()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 world = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos2D = new Vector2(world.x, world.y);

        RaycastHit2D hit = Physics2D.Raycast(pos2D, Vector2.zero, 0f, towerLayer);
        if (!hit) return;

        if (hit.collider.TryGetComponent(out TowerInstance tower))
        {
            uiTowerActions.SelectTower(tower);
        }
    }
}