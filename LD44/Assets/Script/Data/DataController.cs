using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace Data
{

    public class DataController : MonoBehaviour
    {

        private string levelDataFilename = "data.json";

        public GameObject obstaclePrefab;
        public GameObject spawnPointPrefab;
        public GameObject spawnPrefab;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            LoadLevelData();
        }

        List<GameObject> FindAllPrefabInstances(UnityEngine.Object myPrefab)
        {
            List<GameObject> result = new List<GameObject>();
            GameObject[] allObjects = (GameObject[])FindObjectsOfType(typeof(GameObject));
            foreach(GameObject GO in allObjects)
            {
                if (EditorUtility.GetPrefabType(GO) == PrefabType.PrefabInstance)
                {
                    UnityEngine.Object GO_prefab = EditorUtility.GetPrefabParent(GO);
                    if (myPrefab == GO_prefab)
                        result.Add(GO);
                }
            }
            return result;
        }

        private void GenerateLevel(LevelData levelData)
        {

            FlowManager flowManager = gameObject.GetComponent<FlowManager>();
            
            
            if (levelData.obstacles != null && obstaclePrefab != null)
            {
                foreach (Obstacle obstacle in levelData.obstacles)
                {
                    GameObject newObstacle = Instantiate(obstaclePrefab, new Vector3(obstacle.x, obstacle.y, 0), Quaternion.identity) as GameObject;  // instatiate the object
                    newObstacle.transform.localScale = new Vector3(obstacle.width, obstacle.height, 1);
                }
            }

            if (levelData.spawnPoints != null && spawnPointPrefab != null)
            {
                foreach (SpawnPoint spawnPoint in levelData.spawnPoints)
                {
                    Instantiate(spawnPointPrefab, new Vector3(spawnPoint.x, spawnPoint.y, 0),
                        Quaternion.identity);
                }
            }

            if (levelData.actor != null)
            {
                ActorStats[] actors = GameObject.FindObjectsOfType<ActorStats>() as ActorStats[];
                

                if (actors.Length > 0)
                {
                    for (int i = 0; i < actors.Length; i++)
                    {
                        actors[i].Health = levelData.actor.health;
                        actors[i].Damage = levelData.actor.damage;
                        actors[i].MovementSpeed = levelData.actor.movementSpeed;
                        actors[i].AttackSpeed = levelData.actor.attackSpeed;
                        actors[i].AttackRange = levelData.actor.attackRange;
                    }
                }

            }

            if (levelData.spawns != null && flowManager != null && spawnPrefab != null)
            {
                foreach (Spawn spawn in levelData.spawns)
                {

                    FlowManager.SpawnStats stats = new FlowManager.SpawnStats();
                    stats.Prefab = spawnPrefab;
                    stats.Type = (FlowManager.EnemyType) spawn.type;
                    stats.BaseSpawnChance = spawn.baseSpawnChance;
                    stats.SpawnIncreasePerDifficultyLevel = spawn.spawnIncreasePerDifficultyLevel;
                    stats.DifficultyLevelToSpawn = spawn.difficultyLevelToSpawn;
                    stats.BaseTimesToTryAndSpawn = spawn.baseTimesToTryAndSpawn;
                    stats.TimesToTrySpawnIncreasePerDifficultyLevel = spawn.timesToTrySpawnIncreasePerDifficultyLevel;
                    stats.OnlySpawnOnce = spawn.onlySpawnOnce;
                    
                    flowManager.EnemyList.Add(stats);
                }

                
            }
        }
        
        private void LoadLevelData()
        {
           string filePath = Path.Combine(Application.streamingAssetsPath, levelDataFilename);

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                LevelData loadedData = JsonUtility.FromJson<LevelData>(dataAsJson);            
                GenerateLevel(loadedData);
            }
            else
            {
                
                // TODO: Remove
                /*
                LevelData ld = new LevelData();
                ld.obstacles = new Obstacle[] { new Obstacle() };
                ld.difficulty = 2;

                Spawn s = new Spawn();
                s.type = EnemyType.Hostile;
                ld.spawns = new Spawn[] { s };

                string dataAsJson = JsonUtility.ToJson(ld);
                File.WriteAllText(filePath, dataAsJson);
                */
                
                Debug.LogError("Cannot load game data!");
            }
        }
    }
}