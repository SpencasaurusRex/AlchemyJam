using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class StageController : MonoBehaviour
    {
        public StageInfo StageInfo;
        public List<SpawnPedestal> SpawnLocations;
        public List<PlacementPedestal> PlacementLocations;

        Queue<SpawnEntry> spawnQueue;

        List<int> progresses = new List<int>();

        public void Progress(IngredientType type)
        {
            int goalIndex = StageInfo.Goals.FindIndex(x => x.Type == type);
            int progress = ++progresses[goalIndex];
            // Compare to goal
            if (progress >= StageInfo.Goals[goalIndex].Amount)
            {
                // We did it
                print("We did it!");
            }
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            spawnQueue = new Queue<SpawnEntry>(StageInfo.Spawns.Count);
            var orderedSpawns = StageInfo.Spawns.OrderBy(x => x.Time).ToList();
            foreach (var spawn in orderedSpawns)
            {
                spawnQueue.Enqueue(spawn);
            }

            var goalTypes = StageInfo.Goals.Select(x => x.Type).Distinct().ToList();
            foreach (var placement in PlacementLocations)
            {
                placement.GoalTypes = goalTypes;
                placement.StageController = this;
            }

            for (int i = 0; i < goalTypes.Count; i++)
            {
                progresses.Add(0);
            }
        }

        float currentTime;
        void Update()
        {
            currentTime += Time.deltaTime;
            while (spawnQueue.Any() && spawnQueue.Peek().Time <= currentTime)
            {
                var spawn = spawnQueue.Dequeue();

                var pedestal = SpawnLocations[spawn.SpawnIndex];
                pedestal.Spawn(spawn.Type);
            }
        }
    }
}
