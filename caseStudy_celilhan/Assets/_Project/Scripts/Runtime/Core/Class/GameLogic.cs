using System;
using _Project.Scripts.Runtime.Core.Controller;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Runtime.Core.Class
{
    public class GameLogic
    {
        public static async void StartTimer(float timeLimit)
        {
            GameData.TimeLimit = timeLimit;
            GameData.StartTimer = Time.timeSinceLevelLoad;

            var remaining = float.MaxValue;
            while (remaining > 0f)
            {
                remaining = GameData.TimeLimit - (Time.timeSinceLevelLoad - GameData.StartTimer);
                if (remaining <= 0)
                {
                    Fail();
                    break;
                }

                UIEvents.UpdateTimer?.Invoke((int)remaining);
                await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.Realtime);
            }
        }

        public static async void AddMatchableToSlot(MatchableController matchable)
        {
            GameData.PlacedMatchableList.Add(matchable.Key);
            var placeIndex = GameData.PlacedMatchableList.Count - 1;
            Debug.Log("Contains: " + GameData.PlacedMatchableList.Contains(matchable.Key));
            if (GameData.PlacedMatchableList.Contains(matchable.Key))
            {
                for (var i = 0; i < GameData.PlacedMatchableList.Count - 1; i++)
                {
                    Debug.Log("Equals: " + (GameData.PlacedMatchableList[i] == matchable.Key));
                    if (GameData.PlacedMatchableList[i] == matchable.Key)
                    {
                        for (var j = i + 1; j < GameData.PlacedMatchableList.Count; j++)
                        {
                            Debug.Log("Equals: " + (GameData.PlacedMatchableList[j] == matchable.Key));
                            if (GameData.PlacedMatchableList[j] == matchable.Key)
                                continue;
                            placeIndex = i;
                            break;
                        }
                    }

                    Debug.Log("Chaned: " + (placeIndex != GameData.PlacedMatchableList.Count - 1));
                    if (placeIndex != GameData.PlacedMatchableList.Count - 1)
                        break;
                }

                Debug.Log("Swapping");
                var keyToBePlaced = GameData.PlacedMatchableList[^1];
                for (var i = GameData.PlacedMatchableList.Count - 1; i > placeIndex; i--)
                {
                    GameData.PlacedMatchableList[i] = GameData.PlacedMatchableList[i - 1];
                    CoreEvents.MatchableMoved?.Invoke(matchable.Key, i - 1, i);
                }

                GameData.PlacedMatchableList[placeIndex] = keyToBePlaced;
            }

            CoreEvents.MatchablePlaced?.Invoke(matchable.Key, placeIndex);

            while (true)
            {
                var wasMatchFound = CheckMatches();
                Debug.Log("wasMatchFound: " + wasMatchFound);

                if (wasMatchFound)
                {
                    await UniTask.Yield();
                }
                else
                {
                    break;
                }
            }

            if (GameData.PlacedMatchableList.Count >= GameData.MatchableSlotCount)
                Fail();

            foreach (var placedMatchable in GameData.PlacedMatchableList)
                Debug.Log(placedMatchable);
        }

        private static bool CheckMatches()
        {
            if (GameData.PlacedMatchableList.Count <= 0) return false;
            var key = GameData.PlacedMatchableList[0];
            for (var i = 0; i < GameData.PlacedMatchableList.Count - 1; i++)
            {
                if (GameData.PlacedMatchableList[i] == key)
                {
                    var count = 1;
                    for (var j = i + 1; j < GameData.PlacedMatchableList.Count; j++)
                    {
                        if (GameData.PlacedMatchableList[j] == key)
                        {
                            count++;
                            if (count >= 3)
                            {
                                for (var k = 2; k > -1; k--)
                                {
                                    Debug.Log(i + " " + k);
                                    GameData.PlacedMatchableList.RemoveAt(i + k);
                                    CoreEvents.MatchableRemoved?.Invoke(key, i + k);
                                }

                                for (var k = i; i < GameData.PlacedMatchableList.Count; i++)
                                    CoreEvents.MatchableMoved?.Invoke(key, k + 3, k);

                                GameData.SucceededTripleCount++;
                                if (GameData.SucceededTripleCount >= GameData.TripleCount)
                                    Win();

                                return true;
                            }

                            continue;
                        }

                        i = j;
                        key = GameData.PlacedMatchableList[i];
                        break;
                    }
                }
                else
                {
                    key = GameData.PlacedMatchableList[i];
                }
            }

            return false;
        }

        public static void ClearSlots()
        {
            for (var i = GameData.PlacedMatchableList.Count - 1; i > -1; i--)
            {
                CoreEvents.MatchableRemoved?.Invoke(GameData.PlacedMatchableList[i], i);
                GameData.PlacedMatchableList.RemoveAt(i);
            }
        }

        private static void Fail()
        {
            GameData.Result = false;
            Debug.Log("Fail!");
            End();
        }

        private static void Win()
        {
            GameData.Result = true;
            Debug.Log("Win!");
            End();
        }

        private static void End()
        {
            GameData.State = GameState.End;
        }
    }
}