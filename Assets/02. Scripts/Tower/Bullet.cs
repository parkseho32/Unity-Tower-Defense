using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // 총알 속도
    private Transform target;                   // 추적 대상
    private int damage;                         // 데미지 값

    // 타워가 총알 생성 후 목표/데미지를 주입
    public void Init(Transform targetTr, int dmg)
    {
        target = targetTr;
        damage = dmg;
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
        // 적을 맞췄는지 확인
        if (target != null && other.transform == target)
        {
            // EnemyHealth가 있으면 데미지 적용
            if (other.TryGetComponent(out EnemyHealth hp))
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject); // 맞으면 총알 제거
        }
    }
}