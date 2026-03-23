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
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private TMP_Dropdown targetModeDropdown;
    [SerializeField] private Button setDefaultButton;

    private TowerInstance selected;

    private void Start()
    {
        panelRoot.SetActive(false);

        upgradeButton.onClick.AddListener(OnClickUpgrade);
        sellButton.onClick.AddListener(OnClickSell);

        if (closeButton != null)
            closeButton.onClick.AddListener(Deselect);

        if (targetModeDropdown != null)
            targetModeDropdown.onValueChanged.AddListener(OnChangedTargetMode);

        if (setDefaultButton != null)
            setDefaultButton.onClick.AddListener(SetAsDefaultMode);
    }

    public void SelectTower(TowerInstance tower)
    {
        selected = tower;
        panelRoot.SetActive(true);

        SyncDropdownWithTower();

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
        if (lifeManager != null && lifeManager.IsGameOver) return;

        int upCost = selected.GetUpgradeCost();
        if (!buildManager.Economy.TrySpend(upCost)) return;

        selected.ApplyUpgrade(); // 데미지+사거리 증가
        Refresh();
    }

    private void OnClickSell()
    {
        if (selected == null) return;
        if (lifeManager != null && lifeManager.IsGameOver) return;

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

    private Tower GetTowerFromSelected()
    {
        if (selected == null) return null;

        Tower t = selected.GetComponent<Tower>();
        if (t == null) t = selected.GetComponentInChildren<Tower>();
        if (t == null) t = selected.GetComponentInParent<Tower>();

        return t;
    }

    private void SyncDropdownWithTower()
    {
        if (targetModeDropdown == null) return;

        Tower t = GetTowerFromSelected();
        if (t == null) return;

        int idx = (int)t.GetTargetMode();
        targetModeDropdown.SetValueWithoutNotify(idx);
        targetModeDropdown.RefreshShownValue();
    }

    private void OnChangedTargetMode(int value)
    {
        Tower t = GetTowerFromSelected();
        if (t == null) return;

        t.SetTargetMode((TargetMode)value);
    }

    private void SetAsDefaultMode()
    {
        if (targetModeDropdown == null) return;
        if (buildManager == null) return;

        buildManager.SetDefaultTargetMode((TargetMode)targetModeDropdown.value);
    }
}