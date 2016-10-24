using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Enemies
{
    public class EnemyManager : MonoBehaviour, EnemyDeathObserver
    {
        public float MinRespawnTime = 1;
        public float MaxRespawnTime = 5;
        public float RespawnTimeDecrement = 0.25f;
        private float _respawnTime;
        private float _timeSinceSpawn;

        public float StartSpawningAfter = 5;

        public int MinSpawnCount = 1; // How many enemies we want to spawn at a time.
        public int MaxSpawnCount = 8;
        public int AmountEnemiesKilledToIncreaseSpawnCount = 5;
        [SerializeField] private int _spawnCount;

        public int HealthPoints = 1;

        private Spawner[] _enemySpawners; //the enemy spawners.
        public int EnemiesKilled { get; private set; }
        [SerializeField] private int EnemiesSpawned;

        private const string EnemyTag = "Enemy";
        private bool _isSpawning;
        
        // Use this for initialization
        void Awake()
        {
            //Find all enemy spawners in the scene.
            _enemySpawners = GetSpawnersWithTag(EnemyTag);
        }

        void Update()
        {
            _timeSinceSpawn += Time.deltaTime;
        }

        public void Restart()
        {
            //remove all enemies from the field.
            Clear();

            //set values back to original state.
            EnemiesKilled = 0;
            _respawnTime = MaxRespawnTime;
            _spawnCount = MinSpawnCount;

            _isSpawning = true;
            //start spawning.
            Invoke("ScheduledSpawn", StartSpawningAfter);
        }

        #region Spawning
        private void ScheduledSpawn()
        {
            if (!_isSpawning) return;
            RandomSpawn();
            //schedule next.
            Invoke("ScheduledSpawn", _respawnTime);
        }

        /// <summary>
        /// Returns all spawners that spawn objects with the given tag.
        /// </summary>
        /// <param name="objectTag">tag of objects spawned by the spawners you're looking for</param>
        /// <returns>all spawners that spawn objects with the given tag.</returns>
        public Spawner[] GetSpawnersWithTag(string objectTag)
        {
            Spawner[] spawners = GetComponentsInChildren<Spawner>();
            return spawners.Where(spawner => spawner.tag == objectTag).ToArray();
        }


        /// <summary>
        /// Spawns this SpawnAtATime mount of objects,starting with the spawner at spawnerIndex in the Spawnerlist 
        /// </summary>
        /// <param name="spawnerIndex">index of spawner to be used in first spawning.</param>
        /// <returns></returns>
        public GameObject SpecificSpawn(int spawnerIndex)
        {
            //Spawner should be free.
            if(!_enemySpawners[spawnerIndex].IsFree)
                throw new Exception("Trying to spawn an enemy on a not-free location");

            //spawn
            GameObject obj = _enemySpawners[spawnerIndex].Spawn();

            //register as observer(we want to know when the enemy dies).
            HitBoxController hbController = obj.GetComponent<HitBoxController>();
            hbController.RegisterObserver(this);

            //data tracking.
            EnemiesSpawned++;
            _timeSinceSpawn = 0;

            return obj;
        }

        /// <summary>
        /// Uses a randomly selected spawner to spawn an object
        /// </summary>
        /// <returns>The spawned object.</returns>
        public GameObject[] RandomSpawn()
        {
            return RandomSpawn(_spawnCount);
        }

        /// <summary>
        /// Uses count amount of randomly selected spawners to spawn.
        /// </summary>
        /// <param name="count">amount of spawners to be used</param>
        /// <returns>the spawned objects</returns>
        public GameObject[] RandomSpawn(int count)
        {
            GameObject[] objects = new GameObject[count];

            //spawn objects
            for (int i = 0; i < count; i++)
            {
                int index = GetFreeSpawnerIndex();
                //when we can spawn no more enemies.
                if (index == -1)
                    return objects;

                objects[i] = SpecificSpawn(index);
            }
            return objects;
        }

        /// <summary>
        /// gets a random index of a free spawner.
        /// </summary>
        /// <returns>random index of a free spawner.</returns>
        private int GetFreeSpawnerIndex()
        {
            List<int> freeIndices = new List<int> {-1};
            //find all free spawners.
            for (int index = 0; index < _enemySpawners.Length; index++)
            {
                if(_enemySpawners[index].IsFree)
                    freeIndices.Add(index);
            }

            //choose one.
            int choice = Random.Range(0, freeIndices.Count - 1);
            return freeIndices[choice];
        }

        #endregion

        public void Clear()
        {
            CancelInvoke();
            if (_enemySpawners == null) return;

            foreach (Spawner enemySpawn in _enemySpawners)
                enemySpawn.Clear();

            _isSpawning = false;
        }
        /// <summary>
        /// Enemy was defeated.
        /// </summary>
        public void Notify()
        {
            throw new NotImplementedException();
        }

        public void EnemyDestroyNotify(bool killed)
        {
            EnemiesSpawned--;
            if(killed)
                IncreaseEnemyKilled();
        }

        /// <summary>
        /// Increase enemieskilled var by amount.
        /// </summary>
        /// <param name="amount">amount of enemies that have been killed. default 1.</param>
        private void IncreaseEnemyKilled(int amount = 1)
        {
            EnemiesKilled += amount;
            _respawnTime -= RespawnTimeDecrement;
            _respawnTime = Math.Max(_respawnTime, MinRespawnTime);

            if (EnemiesKilled % AmountEnemiesKilledToIncreaseSpawnCount != 0) return;
            _spawnCount++;
            _spawnCount = Math.Min(_spawnCount, MaxSpawnCount);
        }
    }
}