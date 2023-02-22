using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime.Utils.Mono
{
    public class Updater : MonoBehaviour
    {
        public static UnityAction OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}