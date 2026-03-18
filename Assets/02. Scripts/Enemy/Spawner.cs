using System;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy enemyPrefab;   // 스폰할 적 프리팹
    [SerializeField] private Transform pathRoot;  // WP들이 들어있는 부모 오브젝트(Path)

    [Header("Wave")]
    [SerializeField] private int spawnCount = 10; // 웨이브 당 스폰 수
    [SerializeField] private float spawnInterval = 0.05f;  // 스폰 간격(초)

    [SerializeField] private EnemyRegistry registry;

    private Transform[] waypoints;   // pathRoot 자식들을 배열로 저장

    public bool IsSpawning { get; private set; }
    public event Action<Enemy> OnSpawned;

    public event Action OnWaveSpawnFinished;

    private void Awake()
    {
        int childCount = pathRoot.childCount;
        waypoints = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            waypoints[i] = pathRoot.GetChild(i);
        }
    }

    public void StartWave()
    {
        StartWave(spawnCount, 1);
    }

    public void StartWave(int count)
    {
        StartWave(count, 1);
    }

    public void StartWave(int count, int waveIndex)  // 웨이브마다 스폰 수 받기
    {
        if (IsSpawning) return;  // 이미 스폰 중이면 중복 시작 방지
        StartCoroutine(CoSpawnWave(count,waveIndex));
    }

    private IEnumerator CoSpawnWave(int count, int waveIndex)
    {
        IsSpawning = true;

        for (int i = 0; i < count; i++)
        {
            Enemy e = Instantiate(enemyPrefab);
            e.Init(waypoints);

            if (e.TryGetComponent(out EnemyHealth hp))
                hp.ApplyWaveScaling(waveIndex, 5);

            registry.Register(e);

            OnSpawned?.Invoke(e);    // WaveManager가 한 마리 스폰됨 카운트 가능

            yield return new WaitForSeconds(spawnInterval);
        }

        IsSpawning = false;
        OnWaveSpawnFinished?.Invoke();
    }
}
