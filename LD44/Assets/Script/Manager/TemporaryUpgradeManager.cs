using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryUpgradeManager : MonoBehaviour
{
    void Start()
    {
        foreach (Upgrade upgrade in PermanentUpgradeManager.upgrades)
        {
            upgrade.temporaryStage = upgrade.stage;
        }
    }

    public void UpgradeTemporary(Upgrade upgrade)
    {
        upgrade.temporaryStage++;
        //GameManager.cells -= UpgradeCost(upgrades[upgrade]);
    }

    public int UpgradeCost(int stage)
    {
        return stage * 2;
    }
}