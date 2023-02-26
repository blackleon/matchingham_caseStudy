using _Project.Scripts.Runtime.Core.Controller;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    public class MatchableImporter
    {
#if UNITY_EDITOR
        private static string List;

        [MenuItem("intLeon/ImportMatchable")]
        public static void ImportMatchable()
        {
            List = "";

            var prefab = Resources.Load<MatchableController>("MatchablePrefab");
            var selectedList = Selection.gameObjects;

            foreach (var selected in selectedList)
                GenerateMatchable(selected, prefab);

            Debug.Log(List);
        }

        private static void GenerateMatchable(GameObject selected, MatchableController prefab)
        {
            var matchableBase = Object.Instantiate(prefab);
            var matchable = Object.Instantiate(selected, matchableBase.visual);
            var col = matchableBase.col.gameObject.AddComponent<MeshCollider>();
            col.convex = true;
            col.sharedMesh = matchable.GetComponentInChildren<MeshFilter>().sharedMesh;
            col.transform.localScale = matchable.transform.localScale;
            matchableBase.col = col;
            Object.DestroyImmediate(matchableBase.col.gameObject.GetComponent<SphereCollider>());
            List += selected.name + ",\n";
            matchableBase.name = selected.name;
        }
#endif
    }
}