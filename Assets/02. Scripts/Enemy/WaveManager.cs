using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner;                // 스폰 담당
    [SerializeField] private Button nextWaveButton;          // NextWave 버튼
    [SerializeField] private TextMeshProUGUI waveText;       // 웨이브/남은 적 표시(선택)

    [SerializeField] private int startSpawnCount = 8;
    [SerializeField] private int addPerWave = 3;

    [SerializeField] private EnemyRegistry registry;

    private int waveIndex = 0;
    private int aliveEnemies = 0;     // 현재 살아있는 적 수
    private int plannedSpawn = 0;     // 이번 웨이브에서 스폰할 총 수

    private void OnEnable()
    {
        spawner.OnSpawned += HandleSpawned; // 스폰 알림 구독
        spawner.OnWaveSpawnFinished += CheckWaveEnd; 
        nextWaveButton.onClick.AddListener(StartNextWave);
    }

    private void OnDisable()
    {
        spawner.OnSpawned -= HandleSpawned;
        spawner.OnWaveSpawnFinished -= CheckWaveEnd;
        nextWaveButton.onClick.RemoveListener(StartNextWave);
    }

    public void StartNextWave()
    {
        // 웨이브 진행 중(스폰 중이거나 살아있는 적 있으면) 다음 웨이브 금지
        if (spawner.IsSpawning || aliveEnemies > 0) return;

        waveIndex++;
        plannedSpawn = startSpawnCount + (waveIndex - 1) * addPerWave;

        spawner.StartWave(plannedSpawn);

        // 웨이브 시작하면 버튼 잠금
        nextWaveButton.interactable = false;

        UpdateUI();
    }

    private void HandleSpawned(Enemy e)
    {
        aliveEnemies++; // 한 마리 살아있는 적 추가
        UpdateUI();

        // EnemyHealth에 죽음 이벤트 연결해서 죽으면 alive 감소
        if (e.TryGetComponent(out EnemyHealth hp))
        {
            hp.OnDied += () =>
            {
                aliveEnemies--;
                if (aliveEnemies < 0) aliveEnemies = 0;

                registry.Unregister(e);

                CheckWaveEnd();
                UpdateUI();
            };
        }

        e.OnReachedGoal += () =>
        {
            aliveEnemies--;
            if (aliveEnemies < 0) aliveEnemies = 0;

            registry.Unregister(e);

            CheckWaveEnd();
            UpdateUI();
        };
    }

    private void UpdateUI()
    {
        if (waveText != null)
            waveText.text = $"Wave {waveIndex} | Alive {aliveEnemies}";
    }

    private void CheckWaveEnd()
    {
        if (!spawner.IsSpawning && aliveEnemies == 0)
            nextWaveButton.interactable = true;

        UpdateUI();
    }
}