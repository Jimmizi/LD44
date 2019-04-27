using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PermanentUpgradeManager : MonoBehaviour
{
    public static Upgrade damageUpgrade;   
    public static Upgrade HPUpgrade;   
    public static Upgrade cloningUpgrade;

    public static Upgrade[] upgrades;

    void Start()
    {
        DontDestroyOnLoad(this);

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

    public static void UpgradePermanent(Upgrade upgrade)
    {        
        upgrade.stage++;      
        //GameManager.cells -= UpgradeCost(upgrades[upgrade]);
    }

    public static int UpgradeCost(int stage)
    {
        return stage * 5;
    }
}