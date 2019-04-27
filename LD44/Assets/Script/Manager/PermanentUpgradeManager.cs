using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PermanentUpgradeManager : MonoBehaviour
{
    public static Upgrade damageUpgrade;   
    public static Upgrade HPUpgrade;   
    public static Upgrade cloningUpgrade;

    public static Upgrade[] upgrades;

    void Awake()
    {
        if (damageUpgrade != null)
            return;

        damageUpgrade = new Upgrade();
        HPUpgrade = new Upgrade();
        cloningUpgrade = new Upgrade();

        upgrades = new Upgrade[]
        {
            damageUpgrade,
            HPUpgrade,
            cloningUpgrade
        };
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    private const int upgradeCostMutiplier = 2;

    public static int UpgradeCost(int stage)
    {
        return stage * upgradeCostMutiplier;
    }
}