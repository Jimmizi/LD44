using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PermanentUpgradeGUIManager : MonoBehaviour
{
    public GameObject damageUpgradeObject;
    public GameObject HPUpgradeObject;
    public GameObject cloningUpgradeObject;

    public Text cellsCounter;

    private int endOfRoundCellReward = 10;

    void Awake()
    {
        PermanentUpgradeManager.damageUpgrade.upgradeObject = damageUpgradeObject;
        PermanentUpgradeManager.HPUpgrade.upgradeObject = HPUpgradeObject;
        PermanentUpgradeManager.cloningUpgrade.upgradeObject = cloningUpgradeObject;
    }

    private void Start()
    {
        GameManager.InfectedCellsCount += endOfRoundCellReward;

        SetUpGUI(GameManager.InfectedCellsCount);
    }

    void SetUpGUI(int cells)
    {
        foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
        {
            // sets "stage x" text
            upgrade.upgradeObject.GetCompomentWithName<Text>("StageText").text = "stage\n" + upgrade.stage;

            // sets interactibility of upgrade button
            Button upgradeButton = upgrade.upgradeObject.GetComponentInChildren<Button>();
            upgradeButton.interactable = PermanentUpgradeManager.UpgradeCost(upgrade.stage) <= cells;
            upgradeButton.GetComponentInChildren<Text>().text = "Upgrade\n<" + PermanentUpgradeManager.UpgradeCost(upgrade.stage) + '>';
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