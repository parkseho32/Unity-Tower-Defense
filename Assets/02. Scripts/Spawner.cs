using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy enemyPrefab;   // 스폰할 적 프리팹
    [SerializeField] private Transform pathRoot;  // WP들이 들어있는 부모 오브젝트(Path)

    [Header("Wave")]
    [SerializeField] private int spawnCount = 10; // 웨이브 당 스폰 수
    [SerializeField] private float spawnInterval = 0.05f;  // 스폰 간격(초)

    private Transform[] waypoints;   // pathRoot 자식들을 배열로 저장

    private void Awake()
    {
        // Path 아래 자식들을 waypoints로 구성 
        // (주의) 자식 순서가 경로 순서이므로 Hierarchy에서 WP0~WPn 순서를 맞춰야함
        int childCount = pathRoot.childCount;
        waypoints = new Transform[childCount];

        for(int i =0;i<childCount;i++)
        {
            waypoints[i] = pathRoot.GetChild(i);
        }
    }

    // GameManager가 웨이브 시작할 때 호출하는 함수
    public void StartWave()
    {
        StartCoroutine(CoSpawnWave());
    }

    private IEnumerator CoSpawnWave()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // 적 생성
            Enemy e = Instantiate(enemyPrefab);
            e.Init(waypoints);   // 경로 주입

            // 다음 스폰까지 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
