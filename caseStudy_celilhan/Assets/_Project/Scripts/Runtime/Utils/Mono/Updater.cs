using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Utils.Mono
{
    public class Updater : MonoBehaviour //generic update class to handle all the update operations
    {
        public static UnityAction OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}