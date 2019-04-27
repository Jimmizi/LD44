using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class PermanentUpgradeManager : MonoBehaviour
{
    public Upgrade damageUpgrade;
    public GameObject damageUpgradeObject;

    public Upgrade HPUpgrade;
    public GameObject HPUpgradeObject;

    public Upgrade cloningUpgrade;
    public GameObject cloningUpgradeObject;

    public Text cellsCounter;

    private Upgrade[] upgrades;

    void Start()
    {
        damageUpgrade = new Upgrade(damageUpgradeObject, 0);
        HPUpgrade = new Upgrade(HPUpgradeObject, 0);
        cloningUpgrade = new Upgrade(cloningUpgradeObject, 0);

        upgrades = new Upgrade[]
        {
            damageUpgrade,
            HPUpgrade,
            cloningUpgrade
        };

        DontDestroyOnLoad(this);
    }

    void SetUpGUI(int cells)
    {
        foreach (Upgrade upgrade in upgrades)
        {
            upgrade.upgradeObject.transform.Find("StageText").GetComponent<Text>().text = "stage\n" + upgrade.stage; // sets "stage x" text

            upgrade.upgradeObject.GetComponent<Button>().interactable = UpgradeCost(upgrade.stage) <= cells; // sets interactibility of upgrade button
        }

        cellsCounter.text = cells.ToString(); // sets cell counter
    }

    void Upgrade(GameObject upgradeObject)
    {
        Upgrade upgrade = upgrades.First(x => x.upgradeObject == upgradeObject);

        upgrade.stage++;
        upgrade.upgradeObject.transform.Find("StageText").GetComponent<Text>().text = upgrade.stage.ToString();
        //GameManager.cells -= UpgradeCost(upgrades[upgrade]);
        //SetUpGUI(GameManager.cells);
    }

    int UpgradeCost(int stage)
    {
        return stage * 5;
    }
}