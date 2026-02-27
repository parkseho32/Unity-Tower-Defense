using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    [SerializeField] private Tower towerPrefab; // 설치할 타워 프리팹
    private Tower builtTower;                   // 설치된 타워 저장

    private void OnMouseDown()
    {
        // 이미 설치되어 있으면 무시
        if (builtTower != null) return;

        // 클릭 시 타워 설치
        builtTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
    }
}