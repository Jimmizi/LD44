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

    public void SetUpGUI(int cells)
    {
	    if (PermanentUpgradeManager.damageUpgrade.temporaryUpgradeObject == null)
	    {
		    PermanentUpgradeManager.damageUpgrade.temporaryUpgradeObject = GameObject.Find("DamageUpgrade");
	    }

	    if (PermanentUpgradeManager.HPUpgrade.temporaryUpgradeObject == null)
	    {
		    PermanentUpgradeManager.HPUpgrade.temporaryUpgradeObject = GameObject.Find("HPUpgrade");
	    }

	    if (PermanentUpgradeManager.cloningUpgrade.temporaryUpgradeObject == null)
	    {
		    PermanentUpgradeManager.cloningUpgrade.temporaryUpgradeObject = GameObject.Find("CloningUpgrade");
	    }

	    if (PermanentUpgradeManager.cloningUpgrade.temporaryUpgradeObject == null ||
	        PermanentUpgradeManager.HPUpgrade.temporaryUpgradeObject ||
	        PermanentUpgradeManager.damageUpgrade.temporaryUpgradeObject)
	    {
		    return;
	    }

	    foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
        {
	        if (upgrade == null)
	        {
		        continue;
	        }

            // sets "stage x" text
            Text tempText = upgrade.temporaryUpgradeObject.GetCompomentWithName<Text>("LevelText");
            if (tempText)
			{
				tempText.text = upgrade.temporaryStage.ToString();
			}

            // sets interactibility and text of upgrade button
            Button upgradeButton = null;

            if (upgrade.temporaryUpgradeObject)
            {
	            upgradeButton = upgrade.temporaryUpgradeObject.GetComponentInChildren<Button>();
	        }
			
			if (upgradeButton)
            {
	            upgradeButton.interactable = UpgradeCost(upgrade.temporaryStage) <= cells;
            }

            upgrade.temporaryUpgradeObject.GetCompomentWithName<Text>("CostText").text = "x" + UpgradeCost(upgrade.temporaryStage);
        }

		//Flow manager does this (y)
        //cellsCounter.text = cells.ToString(); // sets cell counter
    }

    public void UpgradeTemporary(GameObject upgradeObject)
    {
        Upgrade upgrade = PermanentUpgradeManager.upgrades.First(x => x.temporaryUpgradeObject == upgradeObject);

        if (GameManager.InfectedCellsCount < UpgradeCost(upgrade.temporaryStage))
            return;      

        GameManager.InfectedCellsCount -= UpgradeCost(upgrade.temporaryStage);

        upgrade.temporaryStage++;

        SetUpGUI(GameManager.InfectedCellsCount);

        UpgradeEffectManager.ApplyTemporaryUpgrades(upgrade);
    }

    private const int upgradeCostMutiplier = 1;

    public int UpgradeCost(int stage)
    {
        return stage * upgradeCostMutiplier;
    }
}