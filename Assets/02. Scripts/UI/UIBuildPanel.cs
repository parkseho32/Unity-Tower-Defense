using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Economy economy;         // 골드 읽기용
    [SerializeField] private BuildInput buildInput;   // 선택 타워를 전달할 대상

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private TextMeshProUGUI towerCostText;

    [SerializeField] private Button towerButton1;
    [SerializeField] private Button towerButton2;

    [Header("Tower Datas")]
    [SerializeField] private TowerData tower1;        // TD_Basic
    [SerializeField] private TowerData tower2;        // TD_Slow

    private void OnEnable()
    {
        // 버튼 클릭 시 타워 선택
        towerButton1.onClick.AddListener(() => SelectTower(tower1));
        towerButton2.onClick.AddListener(() => SelectTower(tower2));

        // 골드 변하면 UI 갱신
        economy.OnGoldChanged += UpdateGoldUI;
    }

    private void OnDisable()
    {
        economy.OnGoldChanged -= UpdateGoldUI;

        towerButton1.onClick.RemoveAllListeners();
        towerButton2.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        UpdateGoldUI(economy.Gold);   // 시작 골드 표시
        SelectTower(tower1);          // 시작 시 기본 타워 선택(원하면 제거 가능)
    }

    private void SelectTower(TowerData data)
    {
        buildInput.SelectedTower = data; // 실제 설치 대상에 선택 전달

        towerNameText.text = data != null ? data.displayName : "-";
        towerCostText.text = data != null ? $"{data.cost} G" : "-";
    }

    private void UpdateGoldUI(int gold)
    {
        goldText.text = $"Gold: {gold}"; // 골드 표시 갱신
    }
}