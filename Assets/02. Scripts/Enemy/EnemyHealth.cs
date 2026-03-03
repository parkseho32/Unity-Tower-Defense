using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;       // 최대 체력
    [SerializeField] private int rewardGold = 5;   // 처치 보상 골드

    public event Action OnDied;  // 죽을 때 알림

    private int hp;
    private bool isDead = false;
    private Economy economy;                       // 골드 관리자 참조

    private void Awake()
    {
        hp = maxHp;
        economy = FindObjectOfType<Economy>();     // 씬에 Economy 1개라는 전제(현재 구조에 맞음)
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;

        if (hp <= 0)
        {
            isDead = true;

            OnDied?.Invoke();

            if (economy != null) economy.Add(rewardGold);           // 죽을 때 골드 지급

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Destroy(gameObject);
        }
    }
}