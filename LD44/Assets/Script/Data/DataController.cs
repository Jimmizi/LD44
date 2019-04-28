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
        public GameObject spawnPrefab;

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

        /*List<GameObject> FindAllPrefabInstances(UnityEngine.Object myPrefab)
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
        }*/

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

            /*if (levelData.actor != null)
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

            }*/

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