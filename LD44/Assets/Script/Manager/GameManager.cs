using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	//TODO Need to populate friendly count from somewhere
	private static int _infectedCellsCount;

	private static int _difficulty = 1;


    public static int InfectedCellsCount
    {
        get { return _infectedCellsCount; }
        set { DestroyCells(value); }
    }
	
	public static int Difficulty
    {
	    get { return _difficulty; }
	    set { _difficulty = value; }
    }

	public static LevelManager levelManager;

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
    /// Destroys specific amount of infected cells
    /// </summary>
    /// <param name="newCellsCount">New number of infected cells</param>
    private static void DestroyCells(int newCellsCount)
    {
        TemporaryUpgradeManager.singleton?.SetUpGUI(newCellsCount);

        if (newCellsCount > _infectedCellsCount)
        {
            _infectedCellsCount = newCellsCount;
            return;
        }

        int cellsToKill = _infectedCellsCount - newCellsCount;

        // get list of all infected cells except of one controlled by a player
        List<GameObject> infectedCells = FindObjectsOfType<ActorStats>().
                                         Where(x => x.Infected && x.gameObject.GetComponent<PlayerController>() == null).
                                         Select(x => x.gameObject).ToList();

        if (infectedCells.Count == 0) // we are buying permanent upgrade; no actual cell game objects to kill
            return;

        for (int i = 0; i < cellsToKill; i++)
        {
            int index = Random.Range(0, infectedCells.Count);

            infectedCells [index].AddComponent<KillActor>();
            infectedCells.RemoveAt(index);
        }
    }

    public static void InfectedCellDies()
    {
        _infectedCellsCount--;
        TemporaryUpgradeManager.singleton.SetUpGUI(_infectedCellsCount);
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
	    if (root && root.transform)
	    {
		    foreach (Transform child in root.transform)
		    {
			    if (child.gameObject == null)
			    {
				    continue;
			    }

			    if (child.gameObject.name == componentName)
			    {
				    return child.GetComponent<T>();
			    }
			    else
			    {
				    if (child.childCount == 0)
				    {
					    continue;
                    }
				    else
				    {
					    T returnObject = GetCompomentWithName<T>(child.gameObject, componentName);

					    if (returnObject != null)
						    return returnObject;
				    }
			    }
		    }
	    }

	    return null;
    }
}