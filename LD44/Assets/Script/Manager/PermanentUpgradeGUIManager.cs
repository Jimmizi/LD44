using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PermanentUpgradeGUIManager : MonoBehaviour
{
    public GameObject damageUpgradeObject;
    public GameObject HPUpgradeObject;
    public GameObject cloningUpgradeObject;

    public Text cellsCounter;

    void Awake()
    {
        PermanentUpgradeManager.damageUpgrade.upgradeObject = damageUpgradeObject;
        PermanentUpgradeManager.HPUpgrade.upgradeObject = HPUpgradeObject;
        PermanentUpgradeManager.cloningUpgrade.upgradeObject = cloningUpgradeObject;
    }

    void SetUpGUI(int cells)
    {
        foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
        {
            // sets "stage x" text
            upgrade.upgradeObject.transform.Find("StageText").GetComponent<Text>().text = "stage\n" + upgrade.stage;

            // sets interactibility of upgrade button
            upgrade.upgradeObject.GetComponent<Button>().interactable = PermanentUpgradeManager.UpgradeCost(upgrade.stage) <= cells;
        }

        cellsCounter.text = cells.ToString(); // sets cell counter
    }

    public void UpgradePermanent(GameObject upgradeObject)
    {
        Upgrade upgrade = PermanentUpgradeManager.upgrades.First(x => x.upgradeObject == upgradeObject);
        upgrade.upgradeObject.transform.Find("StageText").GetComponent<Text>().text = upgrade.stage.ToString();

        PermanentUpgradeManager.UpgradePermanent(upgrade);

        //SetUpGUI(GameManager.cells);
    }
}