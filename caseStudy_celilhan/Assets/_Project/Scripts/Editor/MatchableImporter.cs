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
            col.transform.position = matchable.transform.position;
            col.convex = true;
            col.sharedMesh = matchable.GetComponentInChildren<MeshFilter>().sharedMesh;
            col.transform.localScale = matchable.transform.localScale;
            col.gameObject.layer = LayerMask.NameToLayer("Default");
            matchableBase.col = col;
            Object.DestroyImmediate(matchableBase.col.gameObject.GetComponent<SphereCollider>());
            var trig = Object.Instantiate(col, matchableBase.transform);
            trig.name = "Trigger";
            trig.transform.localScale *= 1.25f;
            matchableBase.trig = trig; //trigger will require position tweak
            List += selected.name + ",\n";
            matchableBase.name = selected.name;
        }
#endif
    }
}