using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry : MonoBehaviour
{
    // 현재 살아있는 적 목록(타워가 여기만 순회)
    private readonly List<Enemy> enemies = new();

    // 외부에서 읽기만 가능하게 노출(수정은 Register/Unregister로만)
    public IReadOnlyList<Enemy> Enemies => enemies;

    public void Register(Enemy enemy)
    {
        if (enemy == null) return;
        if (enemies.Contains(enemy)) return;
        enemies.Add(enemy);
    }

    public void Unregister(Enemy enemy)
    {
        if (enemy == null) return;
        enemies.Remove(enemy); // 없으면 그냥 무시
    }

    // 안전장치: 파괴된 참조가 남아있을 경우 정리
    public void CleanupNulls()
    {
        enemies.RemoveAll(e => e == null);
    }
}