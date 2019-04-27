using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static int _infectedCellsCount;

    public static int InfectedCellsCount
    {
        get { return _infectedCellsCount; }
        set { DestroyCells(value); _infectedCellsCount = value; }
    }

    public static LevelManager levelManager;

    private static List<GameObject> infectedCells;

    private static Random random;

    void Start()
    {
        DontDestroyOnLoad(this);

        random = new Random();
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
            levelManager.PauseUnpause();

        if (Input.GetButtonDown("UpgradeHP"))
            TemporaryUpgradeManager.singleton.UpgradeTemporary(TemporaryUpgradeManager.singleton.HPUpgradeObject);

        if (Input.GetButtonDown("UpgradeDamage"))
            TemporaryUpgradeManager.singleton.UpgradeTemporary(TemporaryUpgradeManager.singleton.damageUpgradeObject);

        if (Input.GetButtonDown("UpgradeCloning"))
            TemporaryUpgradeManager.singleton.UpgradeTemporary(TemporaryUpgradeManager.singleton.cloningUpgradeObject);
    }

    /// <summary>
    /// Desctroys specific amount of cells (GameObjects in infectedCells array)
    /// </summary>
    /// <param name="amount">Number of cells to destroy</param>
    private static void DestroyCells(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, infectedCells.Count);

            KillCellOnIndex(index);
        }
    }

    public static void KillCell(GameObject cell)
    {
        int index = infectedCells.IndexOf(cell);

        KillCellOnIndex(index);
    }

    private static void KillCellOnIndex(int index)
    {
        Destroy(infectedCells[index]);

        infectedCells.RemoveAt(index);

        if (infectedCells.Count == 0)
            levelManager.GameOver();
    }   

    public static Upgrade UpgradeChooser(string name)
    {
        switch (name)
        {
            case "HP":
                return PermanentUpgradeManager.HPUpgrade;

            case "damage":
                return PermanentUpgradeManager.damageUpgrade;

            case "cloning":
                return PermanentUpgradeManager.cloningUpgrade;

            default:
                return null;
        }
    }
}


internal static class Utitilies
{
    public static T GetCompomentWithName<T>(this GameObject root, string componentName) where T : Component
    {
        foreach (Transform child in root.transform)
        {
            if (child.gameObject.name == componentName)
            {
                return child.GetComponent<T>();
            }
            else
            {
                if (child.childCount == 0)
                {
                    return null;
                }
                else
                {
                    T returnObject = GetCompomentWithName<T>(child.gameObject, componentName);

                    if (returnObject != null)
                        return returnObject;
                }
            }
        }

        return null;
    }
}