using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Pathfinding;
using UnityEditor;


namespace Data
{

    public class DataController : MonoBehaviour
    {

        public GameObject obstaclePrefab;
        public GameObject spawnPointPrefab;

        [System.Serializable]
        public struct SpawnPrefabs
        {
            public GameObject neutralPrefab;
            public GameObject hostilePrefab;
            public GameObject sweeperPrefab;
        }

        public SpawnPrefabs spawnPrefabs;
        

        private string _levelDataFilename = "data.json";
        private bool _obstaclesAdded = false;
        private LevelData _currentLevel;
        private Actor[] _actorTypes;
        
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            LoadLevelData();
        }

        void Update()
        {
            if (_obstaclesAdded)
            {
                RegenerateGrid();
                _obstaclesAdded = false;
            }
            
        }

        private void initActor(ActorStats stats, Actor actor)
        {
            stats.Health = actor.health;
            stats.Damage = actor.damage;
            stats.MovementSpeed = actor.movementSpeed;
            stats.AttackSpeed = actor.attackSpeed;
            stats.AttackRange = actor.attackRange;
        }
    
        public void initActorFromType(ActorStats stats, FlowManager.EnemyType enemyType)
        {
            int ix = (int) enemyType;
            if (_actorTypes != null && ix < _actorTypes.Length) initActor(stats, _actorTypes[ix]);
            
        }
        
        public void initActorFromType(ActorStats stats, int ix)
        {
            if (_actorTypes != null && ix < _actorTypes.Length) initActor(stats, _actorTypes[ix]);            
        }
        
        private void RegenerateGrid()
        {
            GridGenerator generator = GridGenerator.GetInstance();
            if (generator != null)
            {
                generator.DetectObstacles();
                generator.RemovePointsNotInBounds();

            }
        }
        
        private void GenerateLevel(LevelData levelData)
        {

            FlowManager flowManager = gameObject.GetComponent<FlowManager>();
            
            
            if (levelData.obstacles != null && obstaclePrefab != null)
            {
                foreach (Obstacle obstacle in levelData.obstacles)
                {
                    GameObject newObstacle = Instantiate(obstaclePrefab, new Vector3(obstacle.x, obstacle.y, 0), Quaternion.identity) as GameObject;
                    newObstacle.transform.localScale = new Vector3(obstacle.width, obstacle.height, 1);
                }


                _obstaclesAdded = true;
            }

            if (levelData.spawnPoints != null && spawnPointPrefab != null)
            {
                foreach (SpawnPoint spawnPoint in levelData.spawnPoints)
                {
                    Instantiate(spawnPointPrefab, new Vector3(spawnPoint.x, spawnPoint.y, 0),
                        Quaternion.identity);
                }
            }
            
            if (levelData.spawns != null && flowManager != null)
            {
                foreach (Spawn spawn in levelData.spawns)
                {

                    GameObject prefab = null;
                    switch (spawn.type)
                    {
                        case EnemyType.Neutral:
                            prefab = spawnPrefabs.neutralPrefab;
                            break;
                        case EnemyType.Hostile:
                            prefab = spawnPrefabs.hostilePrefab;
                            break;
                        case EnemyType.Sweeper:
                            prefab = spawnPrefabs.sweeperPrefab;
                            break;
                        default:
                            break;
                    }

                    if (prefab != null)
                    {
                        FlowManager.SpawnStats stats = new FlowManager.SpawnStats();
                        stats.Prefab = prefab;
                        stats.Type = (FlowManager.EnemyType) spawn.type;
                        stats.BaseSpawnChance = spawn.baseSpawnChance;
                        stats.SpawnIncreasePerDifficultyLevel = spawn.spawnIncreasePerDifficultyLevel;
                        stats.DifficultyLevelToSpawn = spawn.difficultyLevelToSpawn;
                        stats.BaseTimesToTryAndSpawn = spawn.baseTimesToTryAndSpawn;
                        stats.TimesToTrySpawnIncreasePerDifficultyLevel =
                            spawn.timesToTrySpawnIncreasePerDifficultyLevel;
                        stats.OnlySpawnOnce = spawn.onlySpawnOnce;

                        flowManager.EnemyList.Add(stats);
                    }
                }

                
            }
        }
        
        private void LoadLevelData()
        {
           string filePath = Path.Combine(Application.streamingAssetsPath, _levelDataFilename);

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                LevelData loadedData = JsonUtility.FromJson<LevelData>(dataAsJson);
                _currentLevel = loadedData;
                _actorTypes = new Actor[]
                {
                    loadedData.actorNeutral,
                    loadedData.actorHostile,
                    loadedData.actorSweeper,
                    loadedData.actorFriendly,
                    loadedData.actorPlayer
                };
                
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
        
        // Singleton
        private static DataController _instance;
        
        public static DataController GetInstance()
        {
            return _instance;
        }

        void Awake()
        {
            _instance = this;
        }
    }
}