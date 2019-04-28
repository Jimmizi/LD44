using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeEffectManager : MonoBehaviour
{
    private const int HPUpgradeMultiplier = 15;
    private const int damageUpgradeMultiplier = 4;
    private const float cloningUpgradeMultiplier = 5.0f;

    public static void ApplyPermanentUpgrades(ActorStats actorStats)
    {
        actorStats.Health += PermanentUpgradeManager.HPUpgrade.stage * HPUpgradeMultiplier;
        actorStats.Damage += PermanentUpgradeManager.damageUpgrade.stage * damageUpgradeMultiplier;
        actorStats.CloningChance += PermanentUpgradeManager.cloningUpgrade.stage * cloningUpgradeMultiplier;
    }

    public static void ApplyTemporaryUpgrades(Upgrade upgrade)
    {
        List<ActorStats> infectedCells = FindObjectsOfType<ActorStats>().
                                         Where(x => x.Infected).
                                         ToList();

        foreach (ActorStats infectedCell in infectedCells)
        {
            if (upgrade == PermanentUpgradeManager.HPUpgrade)
                infectedCell.Health += PermanentUpgradeManager.HPUpgrade.CurrentStage * HPUpgradeMultiplier;

            if (upgrade == PermanentUpgradeManager.damageUpgrade)
                infectedCell.Damage += PermanentUpgradeManager.damageUpgrade.CurrentStage * damageUpgradeMultiplier;

            if (upgrade == PermanentUpgradeManager.cloningUpgrade)
                infectedCell.CloningChance += PermanentUpgradeManager.cloningUpgrade.CurrentStage * cloningUpgradeMultiplier;
        }
    }
}