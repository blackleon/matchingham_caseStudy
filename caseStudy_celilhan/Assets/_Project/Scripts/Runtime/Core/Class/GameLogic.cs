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
                await UniTask.Delay(System.TimeSpan.FromSeconds(1f), DelayType.Realtime);
            }
        }

        public async void AddMatchableToSlot(MatchableController matchable)
        {
            GameData.PlacedMatchableList.Add(matchable.Key);
            var placeIndex = GameData.PlacedMatchableList.Count - 1;
            if (GameData.PlacedMatchableList.Contains(matchable.Key))
            {
                for (var i = 0; i < GameData.PlacedMatchableList.Count - 1; i++)
                {
                    if (GameData.PlacedMatchableList[i] == matchable.Key)
                    {
                        for (var j = i + 1; j < GameData.PlacedMatchableList.Count - 1; j++)
                        {
                            if (GameData.PlacedMatchableList[i] == matchable.Key)
                                continue;
                            placeIndex = i;
                            break;
                        }
                    }

                    if (placeIndex != GameData.PlacedMatchableList.Count - 1)
                        break;
                }

                var keyToBePlaced = GameData.PlacedMatchableList[^1];
                for (var i = GameData.PlacedMatchableList.Count - 1; i > placeIndex; i--)
                {
                    GameData.PlacedMatchableList[i] = GameData.PlacedMatchableList[i - 1];
                    CoreEvents.MatchableMoved?.Invoke(matchable.Key, i - 1, i);
                }

                GameData.PlacedMatchableList[placeIndex] = keyToBePlaced;
            }

            CoreEvents.MatchablePlaced?.Invoke(matchable.Key, placeIndex);

            while (CheckMatches())
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
                Debug.Log("FoundMatch!");
            }

            if (GameData.PlacedMatchableList.Count >= GameData.MatchableSlotCount)
                Fail();
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
                    for (var j = i + 1; j < GameData.PlacedMatchableList.Count - 1; j++)
                    {
                        if (GameData.PlacedMatchableList[j] == key)
                        {
                            count++;
                            if (count >= 3)
                            {
                                for (var k = 0; k < 3; k++)
                                {
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