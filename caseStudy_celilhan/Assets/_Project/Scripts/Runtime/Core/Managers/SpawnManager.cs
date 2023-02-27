﻿using System.Collections.Generic;
using _Project.Scripts.Runtime.Core.Class;
using _Project.Scripts.Runtime.Core.Controller;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using _Project.Scripts.Runtime.Utils.Mono;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Runtime.Core.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Vector2 size;
        [SerializeField] private List<MatchableKey> matchableListToSpawn;

        private void Start()
        {
            SpawnMatchables();
        }

        private async void SpawnMatchables()
        {
            GameData.TripleCount = Mathf.Min(matchableListToSpawn.Count, PlayerData.Level + 3);

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

            var randomSpawn = new List<MatchableKey>();
            for (var i = 0; i < Mathf.Min(matchableListToSpawn.Count, PlayerData.Level + 3); i++)
            {
                for (var j = 0; j < 3; j++)
                    randomSpawn.Add(matchableListToSpawn[i]);
            }

            randomSpawn = Shuffle(randomSpawn);

            foreach (var matchableKey in randomSpawn)
            {
                var matchable = Pool.Get(GameData.GetMatchablePoolKey(matchableKey), transform)
                    .GetComponent<MatchableController>();
                matchable.ResetMatchable();

                matchable.transform.position = transform.position + new Vector3(
                    Random.Range(-size.x, size.x) * 0.5f, Random.Range(3f, 5f), Random.Range(-size.y, size.y) * 0.5f);
                matchable.transform.localEulerAngles = Random.insideUnitSphere * 90f;

                matchable.gameObject.SetActive(true);
                matchable.rig.AddForce(Vector3.up * -5f, ForceMode.VelocityChange);

                await UniTask.Delay(System.TimeSpan.FromSeconds(0.025f));
            }

            GameLogic.StartTimer(GameData.TripleCount * 10f);
        }

        private List<MatchableKey> Shuffle(List<MatchableKey> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var targetIndex = Random.Range(0, i);
                (list[i], list[targetIndex]) = (list[targetIndex], list[i]);
            }

            return list;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 1f, size.y));
        }
    }
}