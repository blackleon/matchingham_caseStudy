using System.Collections.Generic;
using _Project.Scripts.Runtime.Data.Object;
using _Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace _Project.Scripts.Runtime.Data.Class
{
    public class GameData
    {
        public static bool InputEnabled;
        public static bool Result;
        public static GameState state;

        public static GameState State
        {
            get => state;
            set
            {
                state = value;
                InputEnabled = state == GameState.Play;
            }
        }

        public static int TripleCount;
        public static int SucceededTripleCount;
        public static int MatchableSlotCount = 7;
        public static List<MatchableKey> PlacedMatchableList;
        public static float StartTimer;
        public static float TimeLimit;

        private static MatchableList MatchableList;

        public string GetMatchablePrefab(MatchableKey key)
        {
            var poolKey = "";
            if (MatchableList.MatchablePrefabs.ContainsKey(key))
                poolKey = MatchableList.MatchablePrefabs[key];

            return poolKey;
        }

        public static void Init()
        {
            MatchableList = Resources.Load<MatchableList>("Data/matchableList");
            MatchableList.Init();
            PlacedMatchableList = new List<MatchableKey>();
            
            Reset();
        }

        public static void Reset()
        {
            State = GameState.Stop;
            Result = false;
            TripleCount = int.MaxValue;
            SucceededTripleCount = 0;
            PlacedMatchableList.Clear();
        }
    }
}