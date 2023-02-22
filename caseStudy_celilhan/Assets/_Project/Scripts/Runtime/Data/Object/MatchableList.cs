using System.Collections.Generic;
using _Project.Scripts.Runtime.Enums;
using _Project.Scripts.Runtime.Utils.Class;
using UnityEngine;

namespace _Project.Scripts.Runtime.Data.Object
{
    [CreateAssetMenu(menuName = "intLeon/matchableList", fileName = "matchableList")]
    public class MatchableList : ScriptableObject
    {
        [SerializeField] private List<Pair<MatchableKey, string>> serializedMatchableList;

        public Dictionary<MatchableKey, string> MatchablePrefabs;

        public void Init()
        {
            MatchablePrefabs = new Dictionary<MatchableKey, string>();
            foreach (var serializedMatchable in serializedMatchableList)
                MatchablePrefabs.Add(serializedMatchable.Value1, serializedMatchable.Value2);
        }
    }
}