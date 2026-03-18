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

    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private int lifeLossPerEscape = 1;

    [SerializeField] private Economy economy;
    [SerializeField] private int baseWaveBonus = 20;
    [SerializeField] private int bonusPerWave = 5;

    [SerializeField] private TextMeshProUGUI waveBannerText;
    [SerializeField] private float bannerDuration = 1.2f;

    private int waveIndex = 0;
    private int aliveEnemies = 0;     // 현재 살아있는 적 수
    private int plannedSpawn = 0;     // 이번 웨이브에서 스폰할 총 수

    private bool waveClearedRewarded = false;

    private Coroutine bannerCo;

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
        if (lifeManager != null && lifeManager.IsGameOver) return;   // 게임오버면 시작 불가
        // 웨이브 진행 중(스폰 중이거나 살아있는 적 있으면) 다음 웨이브 금지
        if (spawner.IsSpawning || aliveEnemies > 0) return;

        waveClearedRewarded = false;
        waveIndex++;
        plannedSpawn = startSpawnCount + (waveIndex - 1) * addPerWave;

        spawner.StartWave(plannedSpawn,waveIndex);

        // 웨이브 시작하면 버튼 잠금
        nextWaveButton.interactable = false;

        ShowBanner($"WAVE {waveIndex} START");
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
            if (lifeManager != null)
                lifeManager.LoseLife(lifeLossPerEscape);

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
        {
            nextWaveButton.interactable = true;

            if(!waveClearedRewarded && economy != null)
            {
                int bonus = baseWaveBonus + (waveIndex * bonusPerWave);
                economy.Add(bonus);
                waveClearedRewarded = true;

                ShowBanner("WAVE CLEAR!");
            }
        }

        UpdateUI();
    }

    private void ShowBanner(string msg)
    {
        if (waveBannerText == null) return;

        if (bannerCo != null) StopCoroutine(bannerCo);
        bannerCo = StartCoroutine(CoBanner(msg));
    }

    private System.Collections.IEnumerator CoBanner(string msg)
    {
        waveBannerText.gameObject.SetActive(true);
        waveBannerText.text = msg;

        yield return new WaitForSeconds(bannerDuration);

        waveBannerText.gameObject.SetActive(false);
        bannerCo = null;
    }
}