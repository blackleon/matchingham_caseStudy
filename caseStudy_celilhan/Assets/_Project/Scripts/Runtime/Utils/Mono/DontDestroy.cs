using UnityEngine;

namespace _Project.Scripts.Runtime.Utils.Mono
{
    public class DontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}