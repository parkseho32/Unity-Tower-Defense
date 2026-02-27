using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 10; // 적 최대 체력
    private int hp;                          // 현재 체력

    private void Awake()
    {
        hp = maxHp; // 시작 체력 세팅
    }

    // 총알이 호출할 데미지 함수
    public void TakeDamage(int damage)
    {
        hp -= damage; // 체력 감소

        if (hp <= 0)
        {
            Destroy(gameObject); // 2일차는 죽으면 제거로 처리
        }
    }
}