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
    private static int _upgradesMade = 0;

    private static int _timesIncreasedCostMultiplier = 0;
    private static int _updatesNeededToUpdateCostMod = 3;

    void Awake()
    {
        PermanentUpgradeManager.damageUpgrade.upgradeObject = damageUpgradeObject;
        PermanentUpgradeManager.HPUpgrade.upgradeObject = HPUpgradeObject;
        PermanentUpgradeManager.cloningUpgrade.upgradeObject = cloningUpgradeObject;
    }

    private void Start()
    {
		//Always get at least one cell
	    GameManager.InfectedCellsCount++;

		// No extra cells per round after the first rotation
	    if (ActorStats.MapRotationCount == 1)
	    {
		    GameManager.InfectedCellsCount = Mathf.FloorToInt((float)GameManager.InfectedCellsCount * endOfRoundCellModifier);
	    }
		
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

        if (GameManager.InfectedCellsCount < PermanentUpgradeManager.UpgradeCost(upgrade.stage))
            return;

        GameManager.InfectedCellsCount -= PermanentUpgradeManager.UpgradeCost(upgrade.stage);

        upgrade.stage++;
        _upgradesMade++;

        if (_upgradesMade >= _updatesNeededToUpdateCostMod)
        {
	        _upgradesMade = 0;
	        PermanentUpgradeManager.UpgradeCostMutiplier++;
	        _timesIncreasedCostMultiplier++;

	        if (_timesIncreasedCostMultiplier >= 6)
	        {
		        _updatesNeededToUpdateCostMod--;
	        }
        }

		SetUpGUI(GameManager.InfectedCellsCount - PermanentUpgradeManager.UpgradeCost(upgrade.stage - 1));
    }
}