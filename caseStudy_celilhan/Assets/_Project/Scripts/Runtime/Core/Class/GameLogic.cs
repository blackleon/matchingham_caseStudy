using _Project.Scripts.Runtime.Core.Controller;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Core.Managers;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Core.Class
{
    public class GameLogic //non player game mechanics
    {
        public static async void StartTimer(float timeLimit) //start countdown timer
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
                await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f), DelayType.Realtime);

                if (GameData.State == GameState.Pause)
                {
                    await UniTask.WaitUntil(() => GameData.State == GameState.Play);
                }
                else if (GameData.State is GameState.Stop or GameState.End)
                {
                    break;
                }
            }
        }

        public static async void
            AddMatchableToSlot(MatchableController matchable) //add matchable to a slot after its selected
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

            GameData.InputEnabled = false;
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

            GameData.InputEnabled = true;

            if (GameData.PlacedMatchableList.Count >= GameData.MatchableSlotCount)
                Fail();
        }

        private static bool CheckMatches() //check if any matchable objects have matched
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
                                    CoreEvents.MatchableRemoved?.Invoke(index, targetIndex);
                                }

                                for (var k = i; k < GameData.PlacedMatchableList.Count; k++)
                                {
                                    var index = k;
                                    CoreEvents.MatchableMoved?.Invoke(index + 3, index, true);
                                }

                                GameData.SucceededTripleCount++;

                                var source = GameData.Cam.WorldToScreenPoint(SlotManager.Slots[i + 1].position);
                                PlayerData.SetMoney(PlayerData.GetMoney() + (1 + GameData.ComboCount), source);
                                GameData.ComboCount++;
                                if (GameData.SucceededTripleCount >= GameData.TripleCount)
                                    Success();
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

        private static async void Fail() //fail method
        {
            Debug.Log("Fail!");
            await End();

            UIEvents.SetUI?.Invoke(UIKey.Fail, true);
        }

        private static async void Success() //win method
        {
            Debug.Log("Success!");
            await End();

            UIEvents.SetUI?.Invoke(UIKey.Success, true);
        }

        private static async UniTask End() //generic end method
        {
            GameData.State = GameState.End;
            Time.timeScale = 1f;

            await UniTask.Delay(System.TimeSpan.FromSeconds(1f + GameData.ComboCount * 0.05f));

            UIEvents.SetUI?.Invoke(UIKey.Main, false);
            UIEvents.SetUI?.Invoke(UIKey.Settings, false);

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.25f));
        }
    }
}