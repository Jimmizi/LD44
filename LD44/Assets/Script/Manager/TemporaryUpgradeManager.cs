using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class TemporaryUpgradeManager : MonoBehaviour
{
    public GameObject damageUpgradeObject;
    public GameObject HPUpgradeObject;
    public GameObject cloningUpgradeObject;

    public Text cellsCounter;

    public static TemporaryUpgradeManager singleton;

    void Start()
    {
        singleton = this;

        PermanentUpgradeManager.damageUpgrade.temporaryUpgradeObject = damageUpgradeObject;
        PermanentUpgradeManager.HPUpgrade.temporaryUpgradeObject = HPUpgradeObject;
        PermanentUpgradeManager.cloningUpgrade.temporaryUpgradeObject = cloningUpgradeObject;

        // reset temporary stage to default value (equal to normal stage)
        foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
            upgrade.temporaryStage = upgrade.stage;

        SetUpGUI(GameManager.InfectedCellsCount);
    }

    private void SetUpGUI(int cells)
    {
        foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
        {
            // sets "stage x" text
            upgrade.temporaryUpgradeObject.GetCompomentWithName<Text>("StageText").text = "stage\n" + upgrade.temporaryStage;

            // sets interactibility and text of upgrade button
            Button upgradeButton = upgrade.temporaryUpgradeObject.GetComponentInChildren<Button>();
            upgradeButton.interactable = UpgradeCost(upgrade.temporaryStage) <= cells;
            upgradeButton.GetComponentInChildren<Text>().text = "Upgrade\n<" + UpgradeCost(upgrade.temporaryStage) + '>';
        }

		//Flow manager does this (y)
        //cellsCounter.text = cells.ToString(); // sets cell counter
    }

    public void UpgradeTemporary(GameObject upgradeObject)
    {
        Upgrade upgrade = PermanentUpgradeManager.upgrades.First(x => x.temporaryUpgradeObject == upgradeObject);

        if (GameManager.InfectedCellsCount < UpgradeCost(upgrade.temporaryStage))
            return;

        upgrade.temporaryStage++;

        GameManager.InfectedCellsCount -= UpgradeCost(upgrade.temporaryStage);

        SetUpGUI(GameManager.InfectedCellsCount);

        UpgradeEffectManager.ApplyTemporaryUpgrades(upgrade);
    }

    private const int upgradeCostMutiplier = 1;

    public int UpgradeCost(int stage)
    {
        return stage * upgradeCostMutiplier;
    }
}