using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static int _infectedCellsCount;

    public static int InfectedCellsCount
    {
        get { return _infectedCellsCount; }
        set { DestroyCells(value); _infectedCellsCount = value; }
    }

    private static GameObject[] infectedCells;

    private static Random random;

    void Start()
    {
        DontDestroyOnLoad(this);

        random = new Random();
    }

    /// <summary>
    /// Desctroys specific amount of cells (GameObjects in infectedCells array)
    /// </summary>
    /// <param name="amount">Number of cells to destroy</param>
    private static void DestroyCells(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, infectedCells.Length);

            Destroy(infectedCells[index]);
        }
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


static class Utitilies
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