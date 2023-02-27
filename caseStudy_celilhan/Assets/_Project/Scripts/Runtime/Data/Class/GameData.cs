using System.Collections.Generic;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Object;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Runtime.Data.Class
{
    public class GameData
    {
        public static Camera Cam;

        public static bool InputEnabled;
        private static GameState state;

        public static GameState State
        {
            get => state;
            set
            {
                state = value;
                InputEnabled = state == GameState.Play;
                Time.timeScale = state == GameState.Pause ? 0.0001f : 1f;
            }
        }

        public static int TripleCount;
        public static int SucceededTripleCount;
        public static int MatchableSlotCount = 7;
        public static List<MatchableKey> PlacedMatchableList;
        public static float StartTimer;
        public static float TimeLimit;

        private static int comboCount;

        public static int ComboCount
        {
            get { return comboCount; }
            set
            {
                if (comboCount.Equals(value)) return;
                comboCount = value;
                if (comboCount > 0)
                    CoreEvents.ComboChanged?.Invoke();
            }
        }

        private static MatchableList MatchableList;

        public static string GetMatchablePoolKey(MatchableKey key)
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

        public async static void Reset()
        {
            State = GameState.Stop;
            SucceededTripleCount = 0;
            comboCount = 0;
            PlacedMatchableList.Clear();

            await UniTask.Yield();

            Cam = Camera.main;
        }
    }
}