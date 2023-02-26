using System.Collections.Generic;
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
            GameData.TripleCount = matchableListToSpawn.Count;

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

            var randomSpawn = new List<MatchableKey>();
            foreach (var matchableKey in matchableListToSpawn)
            {
                for (var i = 0; i < 3; i++)
                    randomSpawn.Add(matchableKey);
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

                await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));
            }
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