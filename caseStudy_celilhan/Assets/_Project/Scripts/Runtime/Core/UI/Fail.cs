using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.Core.UI
{
    public class Fail : UIBase
    {
        [SerializeField] private Button retry;

        private void OnEnable()
        {
            retry.onClick.AddListener(OnRetryClicked);
        }

        private void OnDisable()
        {
            retry.onClick.RemoveListener(OnRetryClicked);
        }

        private void OnRetryClicked()
        {
            UIEvents.SetUI?.Invoke(UIKey.Fail, false);
            CoreEvents.LoadScene?.Invoke();
        }

        public override async UniTask SetUI(bool state)
        {
            await UniTask.Yield();
        }
    }
}