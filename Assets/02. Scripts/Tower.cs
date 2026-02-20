using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float range = 3.5f;     // 사거리
    [SerializeField] private float fireRate = 0.8f;  // 발사 간격(초)
    [SerializeField] private int damage = 2;         // 한 발 데미지

    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;    // 총알 프리팹
    [SerializeField] private Transform firePoint;    // 발사 위치

    private float fireTimer; // 발사 쿨타임 타이머

    private void Update()
    {
        fireTimer -= Time.deltaTime;

        // 쿨타임이 남아있으면 발사 안 함
        if (fireTimer > 0f) return;

        Transform target = FindClosestEnemyInRange();

        // 사거리 안에 적이 있으면 발사
        if (target != null)
        {
            Fire(target);
            fireTimer = fireRate; // 쿨타임 리셋
        }
    }

    // 사거리 내 가장 가까운 적을 찾는 함수 (간단/확실)
    private Transform FindClosestEnemyInRange()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>(); // 1일차 Enemy 이동 스크립트 기준
        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (var e in enemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= range && dist < bestDist)
            {
                bestDist = dist;
                best = e.transform;
            }
        }

        return best;
    }

    private void Fire(Transform target)
    {
        // 총알 생성 후 타겟/데미지 주입
        Bullet b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        b.Init(target, damage);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 에디터에서 사거리 확인용
        Gizmos.DrawWireSphere(transform.position, range);
    }
#endif
}