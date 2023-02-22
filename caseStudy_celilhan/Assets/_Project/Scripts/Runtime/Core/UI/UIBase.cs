using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Runtime.Core.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        public abstract UniTask SetUI(bool state);
    }
}