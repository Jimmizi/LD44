using UnityEngine;

public class UpgradeEffectManager : MonoBehaviour
{
    private const int HPUpgradeMultiplier = 15;
    private const int damageUpgradeMultiplier = 4;
    private const int cloningUpgradeMultiplier = 3;

    public static void ApplyPermanentUpgrades(ActorStats actorStats)
    {
        actorStats.Health += PermanentUpgradeManager.HPUpgrade.CurrentStage * HPUpgradeMultiplier;
        actorStats.Damage += PermanentUpgradeManager.damageUpgrade.CurrentStage * damageUpgradeMultiplier;
        actorStats.Damage += PermanentUpgradeManager.cloningUpgrade.CurrentStage * cloningUpgradeMultiplier;
    }
}