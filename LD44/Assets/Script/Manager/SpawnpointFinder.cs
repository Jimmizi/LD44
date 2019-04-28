using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Poll this class to get a suitable spawn point for something to spawn at
/// </summary>

public class SpawnpointFinder : MonoBehaviour
{
	private GameObject[] _spawnPointsEnemy;
	private GameObject[] _spawnPointsFriendly;

	private void Start()
	{
		_spawnPointsEnemy = GameObject.FindGameObjectsWithTag("EnemySpawn");
		_spawnPointsFriendly = GameObject.FindGameObjectsWithTag("FriendlySpawn");
	}

	public Vector2 GetSpawnPoint(bool bIsFriendlySearch = false)
	{
		var vReturnValue = new Vector2(0, 0);
		var pointsToUse = bIsFriendlySearch ? _spawnPointsFriendly : _spawnPointsEnemy;
		
		if (pointsToUse.Length > 0)
		{
			vReturnValue = pointsToUse[Random.Range(0, pointsToUse.Length)].transform.position;
		}

		return vReturnValue;
	}
  
}
