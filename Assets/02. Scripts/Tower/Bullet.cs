using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // 총알 속도
    private Transform target;                   // 추적 대상
    private int damage;                         // 데미지 값

    // 슬로우 옵션
    private bool applySlow;
    private float slowMultiplier;
    private float slowDuration;

    private bool hasHit = false;

    // 타워가 총알 생성 후 목표/데미지를 주입
    public void Init(Transform targetTr, int dmg)
    {
        Init(targetTr, dmg, false, 1f, 0f);
    }

    public void Init(Transform targetTr, int dmg, bool slow, float mult, float dur)
    {
        target = targetTr;
        damage = dmg;

        applySlow = slow;
        slowMultiplier = mult;
        slowDuration = dur;
    }

    private void Update()
    {
        // 타겟이 죽어서 사라지면 총알도 제거
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 타겟 방향으로 이동
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // 적을 맞췄는지 확인
        if (target != null && other.transform == target)
        {
            hasHit = true;

            // EnemyHealth가 있으면 데미지 적용
            if (other.TryGetComponent(out EnemyHealth hp))
            {
                hp.TakeDamage(damage);
            }

            // 슬로우
            if(applySlow && other.TryGetComponent(out Enemy enemy))
            {
                enemy.ApplySlow(slowMultiplier, slowDuration);
            }

            Destroy(gameObject); // 맞으면 총알 제거
        }
    }
}