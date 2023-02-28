using UnityEngine;

namespace _Project.Scripts.Runtime.Utils.Mono
{
    public class DontDestroy : MonoBehaviour //stop gameObjects from being destroyed on scene load
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}