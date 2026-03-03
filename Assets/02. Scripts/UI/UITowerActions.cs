using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITowerActions : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BuildManager buildManager;

    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button closeButton;

    private TowerInstance selected;

    private void Start()
    {
        panelRoot.SetActive(false);

        upgradeButton.onClick.AddListener(OnClickUpgrade);
        sellButton.onClick.AddListener(OnClickSell);

        if (closeButton != null)
            closeButton.onClick.AddListener(Deselect);
    }

    public void SelectTower(TowerInstance tower)
    {
        selected = tower;
        panelRoot.SetActive(true);
        Refresh();
    }

    private void Refresh()
    {
        if (selected == null)
        {
            panelRoot.SetActive(false);
            return;
        }

        int upCost = selected.GetUpgradeCost();
        int sellValue = selected.GetSellValue();

        titleText.text = selected.Data != null ? selected.Data.displayName : "Tower";
        levelText.text = $"Lv. {selected.Level}  DMG {selected.Damage}  RNG {selected.Range:0.0}";
        costText.text = $"Upgrade: {upCost}G   Sell: {sellValue}G";

        // 돈 부족하면 업그레이드 버튼 비활성
        upgradeButton.interactable = buildManager.Economy.CanSpend(upCost);
    }

    private void OnClickUpgrade()
    {
        if (selected == null) return;

        int upCost = selected.GetUpgradeCost();
        if (!buildManager.Economy.TrySpend(upCost)) return;

        selected.ApplyUpgrade(); // 데미지+사거리 증가
        Refresh();
    }

    private void OnClickSell()
    {
        if (selected == null) return;

        int sellValue = selected.GetSellValue();
        buildManager.Economy.Add(sellValue);

        GameObject towerGo = selected.gameObject;

        // 점유 해제 후 파괴
        buildManager.UnregisterTower(towerGo);
        Destroy(towerGo);

        selected = null;
        panelRoot.SetActive(false);
    }

    public void ClosePanel()
    {
        Deselect();
    }

    public void Deselect()
    {
        selected = null;          // 선택 해제
        panelRoot.SetActive(false); // 패널 숨김
    }
}