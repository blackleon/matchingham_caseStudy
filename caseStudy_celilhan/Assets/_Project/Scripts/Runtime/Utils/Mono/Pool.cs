using System.Collections.Generic;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Utils.Class;
using UnityEngine;

namespace _Project.Scripts.Runtime.Utils.Mono
{
    public class Pool : MonoBehaviour //pool to get and return objects just by their gameObject names
    {
        [SerializeField] private List<Pair<string, Pair<GameObject, int>>> serializedPool;
        private Dictionary<string, GameObject> poolObjects;
        private Dictionary<string, Queue<GameObject>> pool;
        private List<GameObject> givenObjects;

        private static Pool instance;

        private void Awake()
        {
            instance = this;

            poolObjects = new Dictionary<string, GameObject>();
            pool = new Dictionary<string, Queue<GameObject>>();
            foreach (var serializedPair in serializedPool)
            {
                poolObjects.Add(serializedPair.Value1, serializedPair.Value2.Value1);
                pool.Add(serializedPair.Value1, new Queue<GameObject>());

                for (var i = 0; i < serializedPair.Value2.Value2; i++)
                    SpawnObject(serializedPair.Value1);
            }

            givenObjects = new List<GameObject>();
        }

        private void OnEnable()
        {
            CoreEvents.LoadScene += OnLoadScene;
        }

        private void OnDisable()
        {
            CoreEvents.LoadScene -= OnLoadScene;
        }

        private void OnLoadScene()
        {
            for(var i = givenObjects.Count - 1; i > -1; i--)
                Return(givenObjects[i]);
        }

        public static GameObject Get(string objectKey, Transform parent = null, bool resetTransform = true)
        {
            if (!instance.pool.ContainsKey(objectKey)) return null;

            if (instance.pool[objectKey].Count <= 0)
                SpawnObject(objectKey);

            var obj = instance.pool[objectKey].Dequeue();
            obj.transform.parent = parent;

            if (resetTransform)
            {
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
            }

            instance.givenObjects.Add(obj);

            return obj;
        }

        public static void Return(GameObject obj, bool destroyIfKeyIsNotPresent = true)
        {
            obj.SetActive(false);
            if (instance.pool.ContainsKey(obj.name))
            {
                instance.pool[obj.name].Enqueue(obj);
                obj.transform.parent = instance.transform;

                if (instance.givenObjects.Contains(obj))
                    instance.givenObjects.Remove(obj);
            }
            else if (destroyIfKeyIsNotPresent)
            {
                Destroy(obj);
            }
        }

        private static void SpawnObject(string objectKey)
        {
            if (!instance.poolObjects.ContainsKey(objectKey)) return;

            var obj = Instantiate(instance.poolObjects[objectKey], instance.transform);
            obj.name = objectKey;
            obj.SetActive(false);

            instance.pool[objectKey].Enqueue(obj);
        }
    }
}