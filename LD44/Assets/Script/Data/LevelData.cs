using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    
    [System.Serializable]
    public struct Obstacle
    {
        public bool enabled;
        public float x;
        public float y;
        public float width;
        public float height;
    }

    [System.Serializable]
    public struct SpawnPoint
    {
        public float x;
        public float y;
    }
    
    [System.Serializable]
    public class Actor
    {
        /// <summary>
        /// How much health does this character have left?
        /// </summary>
        public int health = 100;

        /// <summary>
        /// How much damage does this character do to another?
        /// </summary>
        public int damage = 20;

        /// <summary>
        /// How fast does the character move?
        /// </summary>
        public float movementSpeed = 0.5f;

        /// <summary>
        /// How fast does this character attack? How little of a delay between attack input and the attack happening is there?
        /// </summary>
        public float attackSpeed = 0.25f;

        /// <summary>
        /// How far away can this character attack from?
        /// </summary>
        public float attackRange = 0.425f; //a good default melee range
    }
    
    [System.Serializable]
    public enum EnemyType
    {
        Neutral,
        Hostile,
        Sweeper
    }

    [System.Serializable]
    public class Spawn
    {
        public EnemyType type = EnemyType.Neutral;

        /// <summary>
        /// The base chance to spawn this in
        /// </summary>
        public int baseSpawnChance = 100;

        /// <summary>
        /// Extra difficulty levels above the DifficultyLevelToSpawn level add onto the chance to spawn
        /// </summary>
        public int spawnIncreasePerDifficultyLevel = 0;

        /// <summary>
        /// The difficulty level of the stage needed before we start trying to spawn things in
        /// </summary>
        public int difficultyLevelToSpawn = 1;

        /// <summary>
        /// How many times to try and spawn
        /// </summary>
        public int baseTimesToTryAndSpawn = 2;

        /// <summary>
        /// How many times to try and spawn
        /// </summary>
        public float timesToTrySpawnIncreasePerDifficultyLevel = 0.4f;

        /// <summary>
        /// If a chance spawns one, don't try to spawn any more
        /// </summary>
        public bool onlySpawnOnce = false;
    }

    
    [System.Serializable]
    public class LevelData
    {
        public Obstacle[] obstacles;
        public SpawnPoint[] spawnPoints;
        
        public Actor actorNeutral;
        public Actor actorHostile;
        public Actor actorSweeper;
        public Actor actorFriendly;
        public Actor actorPlayer;
        
        public Spawn[] spawns;
        
        public int difficulty = 1;

        public float neutralEnemiesModifier = 1.0f;
        public float hostileEnemiesModifier = 0.5f;
        public int roundTimer = 60;
    }
}
