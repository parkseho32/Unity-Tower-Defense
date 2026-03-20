using UnityEngine;

public enum TargetMode
{
    Closest,
    MostProgressed
}

public class Tower : MonoBehaviour
{
    [SerializeField] private TargetMode targetMode = TargetMode.MostProgressed;

    [Header("Refs")]
    [SerializeField] private EnemyRegistry registry;

    [Header("Attack")]
    [SerializeField] private float range = 3.5f;     // 사거리
    [SerializeField] private float fireRate = 0.8f;  // 발사 간격(초)
    [SerializeField] private int damage = 2;         // 한 발 데미지

    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;    // 총알 프리팹
    [SerializeField] private Transform firePoint;    // 발사 위치

    private float fireTimer; // 발사 쿨타임 타이머
    private TowerInstance inst;   // 업그레이드 스텟 제공자

    private void Awake()
    {
        inst = GetComponent<TowerInstance>();   // 같은 오브젝트의 TowerInstance 찾기
        if (inst == null) inst = GetComponentInParent<TowerInstance>();
        if (inst == null) inst = GetComponentInChildren<TowerInstance>();
    }

    private void Update()
    {
        fireTimer -= Time.deltaTime;
        // 쿨타임이 남아있으면 발사 안 함
        if (fireTimer > 0f) return;

        float currentrange = (inst != null) ? inst.Range : range;     // 업그레이드 반영 사거리
        int currentdamage = (inst != null) ? inst.Damage : damage;    // 업그레이드 반영 데미지
        float currentFireRate = (inst != null) ? inst.FireRate : fireRate; // 업그레이드 반영 발사 간격

        Transform target = FindClosestEnemyInRange(currentrange);

        // 사거리 안에 적이 있으면 발사
        if (target != null)
        {
            Fire(target, currentdamage);
            fireTimer = currentFireRate; // 쿨타임 리셋
        }
    }

    // 사거리 내 가장 가까운 적을 찾는 함수 (간단/확실)
    private Transform FindClosestEnemyInRange(float currentrange)
    {
        if (registry == null) return null; // 연결 안 됐으면 동작 안 함(디버그 포인트)

        Enemy bestEnemy = null;
        float bestDist = float.MaxValue;

        int bestWpIndex = -1;

        // FindObjectsOfType 제거 → registry 리스트만 순회
        var list = registry.Enemies;
        for (int i = 0; i < list.Count; i++)
        {
            Enemy e = list[i];
            if (e == null) continue;

            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist > currentrange) continue;   // 사거리 밖이면 제외

            if (targetMode == TargetMode.Closest)
            {
                if (dist <= currentrange && dist < bestDist)
                {
                    // 기존 방식: 가장 가까운 적
                    bestDist = dist;
                    bestEnemy = e;
                }
            }
            else
            {
                // 1) 웨이포인트 인덱스가 큰 적 우선
                int wp = e.WaypointIndex;

                if (wp > bestWpIndex)
                {
                    bestWpIndex = wp;
                    bestDist = dist;
                    bestEnemy = e;
                }
                else if(wp == bestWpIndex && dist < bestDist)
                {
                    bestDist = dist;
                    bestEnemy = e;
                }
            }
        }


        return bestEnemy != null ? bestEnemy.transform : null;
    }

    private void Fire(Transform target, int currentdamage)
    {
        // 총알 생성 후 타겟/데미지 주입
        Bullet b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        if(inst != null && inst.Data != null)
        {
            b.Init(target, currentdamage,
                inst.Data.applySlow,
                inst.Data.slowMultiplier,
                inst.Data.slowDuration);
        }
        else
        {
            b.Init(target, currentdamage);
        }
    }

    public void SetRegistry(EnemyRegistry reg)
    {
        registry = reg;
    }
}