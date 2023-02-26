using _Project.Scripts.Runtime.Core.Controller;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        public static async void AddMatchableToSlot(MatchableController matchable)
        {
            GameData.PlacedMatchableList.Add(matchable.Key);
            var placeIndex = GameData.PlacedMatchableList.Count - 1;
            if (GameData.PlacedMatchableList.Contains(matchable.Key))
            {
                for (var i = 0; i < GameData.PlacedMatchableList.Count - 1; i++)
                {
                    if (GameData.PlacedMatchableList[i] == matchable.Key)
                    {
                        for (var j = i + 1; j < GameData.PlacedMatchableList.Count; j++)
                        {
                            if (GameData.PlacedMatchableList[j] == matchable.Key)
                                continue;
                            placeIndex = j;
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
                    CoreEvents.MatchableMoved?.Invoke(i - 1, i, false);
                }

                GameData.PlacedMatchableList[placeIndex] = keyToBePlaced;
            }

            CoreEvents.MatchablePlaced?.Invoke(matchable, placeIndex);

            while (true)
            {
                var wasMatchFound = CheckMatches();

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
                                    GameData.PlacedMatchableList.RemoveAt(i + k);
                                    var index = i + k;
                                    var targetIndex = i + 1;
                                    DOVirtual.DelayedCall(0.1f,
                                        () => CoreEvents.MatchableRemoved?.Invoke(index, targetIndex));
                                }

                                for (var k = i; k < GameData.PlacedMatchableList.Count; k++)
                                {
                                    var index = k;
                                    DOVirtual.DelayedCall(0.1f,
                                        () => CoreEvents.MatchableMoved?.Invoke(index + 3, index, true));
                                }

                                GameData.SucceededTripleCount++;
                                Debug.Log("Succeeded Count: " + GameData.SucceededTripleCount + ", Total Count: " +
                                          GameData.TripleCount);
                                if (GameData.SucceededTripleCount >= GameData.TripleCount)
                                    Win();

                                return true;
                            }

                            continue;
                        }

                        i = j - 1;
                        key = GameData.PlacedMatchableList[i + 1];
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

        private static void Fail()
        {
            GameData.Result = false;

            Debug.Log("Fail!");
            End();
        }

        private static void Win()
        {
            GameData.Result = true;
            PlayerData.Level++;

            Debug.Log("Win!");
            End();
        }

        private static async void End()
        {
            GameData.State = GameState.End;

            await UniTask.Delay(System.TimeSpan.FromSeconds(1f));

            CoreEvents.LoadScene?.Invoke();
        }
    }
}