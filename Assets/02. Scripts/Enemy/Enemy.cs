using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action OnReachedGoal;    // 마지막 WP 도착 이벤트

    [Header("Move")]
    [SerializeField] private float moveSpeed = 2.5f;   // 적 이동 속도
    private Transform[] waypoints;                     // 이동할 경로(웨이포인트 배열)
    private int wpIndex = 0;                           // 현재 목표 웨이포인트 인덱스

    private bool isRemoved = false;   // 죽음 중복 방지
    private float baseSpeed;          // 기본 속도(슬로우 해제 시 복구)
    private Coroutine slowCo;         // 슬로우 코루틴 핸들

    [Header("VFX")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color slowColor = new Color(0.4f, 0.7f, 1f, 1f);

    public int WaypointIndex => wpIndex;
    public int WaypointCount => waypoints != null ? waypoints.Length : 0;

    private Color originalColor;

    //외부(Spawner)에서 경로를 주입해주는 초기화 함수
    public void Init(Transform[] pathWaypoints)
    {
        waypoints = pathWaypoints;    // 경로 저장
        wpIndex = 0;                  // 첫 웨이포인트부터 시작

        baseSpeed = moveSpeed;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        transform.position = waypoints[0].position;  // 시작 위치를 WP1로 맞춤
    }

    private void Update()
    {
        // 경로가 없으면 움직일 수 없으니 방어 코드
        if (waypoints == null || waypoints.Length == 0) return;

        // 현재 목표 웨이포인트로 이동
        Vector3 target = waypoints[wpIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        // 목표에 거의 도착하면 다음 웨이포인트로 전환
        float dist = Vector3.Distance(transform.position, target);

        if (dist < 0.05f)
        {
            wpIndex++;

            // 마지막을 지나면 "도착 처리" -> 제거
            if (wpIndex >= waypoints.Length)
            {
                ReachGoalAndRemove();
            }
        }
    }

    // 외부(총알)에서 호출: 일정 시간 동안 속도를 배율로 줄임
    public void ApplySlow(float multiplier, float duration)
    {
        if (isRemoved) return;

        if (slowCo != null)
            StopCoroutine(slowCo);

        slowCo = StartCoroutine(CoSlow(multiplier, duration));
    }

    private IEnumerator CoSlow(float multiplier, float duration)
    {
        moveSpeed = baseSpeed * multiplier;

        if (spriteRenderer != null)
            spriteRenderer.color = slowColor;

        yield return new WaitForSeconds(duration);

        moveSpeed = baseSpeed;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        slowCo = null;
    }

    private void ReachGoalAndRemove()
    {
        if (isRemoved) return;
        isRemoved = true;

        OnReachedGoal?.Invoke();
        Destroy(gameObject);
    }
}
