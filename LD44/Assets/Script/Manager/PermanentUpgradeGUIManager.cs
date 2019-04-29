using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PermanentUpgradeGUIManager : MonoBehaviour
{
    public GameObject damageUpgradeObject;
    public GameObject HPUpgradeObject;
    public GameObject cloningUpgradeObject;

    public Text cellsCounter;

    private float endOfRoundCellModifier = 1.25f;

    void Awake()
    {
        PermanentUpgradeManager.damageUpgrade.upgradeObject = damageUpgradeObject;
        PermanentUpgradeManager.HPUpgrade.upgradeObject = HPUpgradeObject;
        PermanentUpgradeManager.cloningUpgrade.upgradeObject = cloningUpgradeObject;
    }

    private void Start()
    {
        GameManager.InfectedCellsCount = Mathf.FloorToInt((float)GameManager.InfectedCellsCount * endOfRoundCellModifier);

        GameManager.InfectedCellsCount = 50;

        SetUpGUI(GameManager.InfectedCellsCount);
    }

    void SetUpGUI(int cells)
    {
        foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
        {
            // sets stage and price texts
            upgrade.upgradeObject.GetCompomentWithName<Text>("LevelText").text = "^" + upgrade.stage;
            upgrade.upgradeObject.GetCompomentWithName<Text>("PriceText").text = PermanentUpgradeManager.UpgradeCost(upgrade.stage) + "x";

            // sets interactibility of upgrade button
            upgrade.upgradeObject.GetComponentInChildren<Button>().interactable = PermanentUpgradeManager.UpgradeCost(upgrade.stage) <= cells;
        }

        cellsCounter.text = cells.ToString(); // sets cell counter
    }

    public void UpgradePermanent(GameObject upgradeObject)
    {
        Upgrade upgrade = PermanentUpgradeManager.upgrades.First(x => x.upgradeObject == upgradeObject);

        if (GameManager.InfectedCellsCount < PermanentUpgradeManager.UpgradeCost(upgrade.temporaryStage))
            return;

        GameManager.InfectedCellsCount -= PermanentUpgradeManager.UpgradeCost(upgrade.stage);

        upgrade.stage++;

        SetUpGUI(GameManager.InfectedCellsCount);
    }
}