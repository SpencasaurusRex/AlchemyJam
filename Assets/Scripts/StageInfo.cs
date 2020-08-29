using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu]
    public class StageInfo : ScriptableObject
    {
        public List<SpawnEntry> Spawns;
        public List<GoalEntry> Goals;
    }

    [Serializable]
    public struct SpawnEntry
    {
        public float Time;
        public IngredientType Type;
        public int SpawnIndex;
    }

    [Serializable]
    public struct GoalEntry
    {
        public IngredientType Type;
        public int Amount;
    }
}
