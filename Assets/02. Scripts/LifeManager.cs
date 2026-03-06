using System;
using TMPro;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private int startLife = 10;           // 시작 라이프
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private GameObject gameOverPanel;

    public int Life { get; private set; }
    public bool IsGameOver { get; private set; }

    public event Action<int> OnLifeChanged;  
    public event Action OnGameOver;  

    private void Awake()
    {
        Life = startLife;
        RefreshUI();
        OnLifeChanged?.Invoke(Life);
    }

    public void LoseLife(int amount)
    {
        if (IsGameOver) return;

        Life -= amount;
        if (Life < 0) Life = 0;

        RefreshUI();
        OnLifeChanged?.Invoke(Life);

        if (Life == 0)
        {
            IsGameOver = true;

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            OnGameOver?.Invoke();
        }
    }

    private void RefreshUI()
    {
        if (lifeText != null)
            lifeText.text = $"Life: {Life}";
    }
}