using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TemporaryUpgradeManager : MonoBehaviour
{
    public GameObject damageUpgradeObject;
    public GameObject HPUpgradeObject;
    public GameObject cloningUpgradeObject;

    public Text cellsCounter;

    void Awake()
    {
        PermanentUpgradeManager.damageUpgrade.temporaryUpgradeObject = damageUpgradeObject;
        PermanentUpgradeManager.HPUpgrade.temporaryUpgradeObject = HPUpgradeObject;
        PermanentUpgradeManager.cloningUpgrade.temporaryUpgradeObject = cloningUpgradeObject;
    }

    void Start()
    {
        // reset temporary stage to default value (equal to normal stage)
        foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
            upgrade.temporaryStage = upgrade.stage;

        SetUpGUI(GameManager.cells);
    }

    void SetUpGUI(int cells)
    {
        foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
        {
            // sets "stage x" text
            upgrade.temporaryUpgradeObject.transform.Find("StageText").GetComponent<Text>().text = "stage\n" + upgrade.stage;

            // sets interactibility of upgrade button
            upgrade.temporaryUpgradeObject.GetComponent<Button>().interactable = UpgradeCost(upgrade.stage) <= cells;
        }

        cellsCounter.text = cells.ToString(); // sets cell counter
    }

    public void UpgradeTemporary(Upgrade upgrade)
    {
        upgrade.temporaryStage++;
        GameManager.cells -= UpgradeCost(upgrade.temporaryStage);

        SetUpGUI(GameManager.cells);
    }

    private const int upgradeCostMutiplier = 1;

    public int UpgradeCost(int stage)
    {
        return stage * upgradeCostMutiplier;
    }
}