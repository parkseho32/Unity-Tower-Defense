using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action OnReachedGoal;    // 마지막 WP 도착 이벤트

    [Header("Move")]
    [SerializeField] private float moveSpeed = 2.5f;   // 적 이동 속도
    private Transform[] waypoints;                     // 이동할 경로(웨이포인트 배열)
    private int wpIndex = 0;                           // 현재 목표 웨이포인트 인덱스

    private bool isRemoved = false;   // 죽음 중복 방지

    //외부(Spawner)에서 경로를 주입해주는 초기화 함수
    public void Init(Transform[] pathWaypoints)
    {
        waypoints = pathWaypoints;    // 경로 저장
        wpIndex = 0;                  // 첫 웨이포인트부터 시작
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

    private void ReachGoalAndRemove()
    {
        if (isRemoved) return;
        isRemoved = true;

        OnReachedGoal?.Invoke();
        Destroy(gameObject);
    }
}
