using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.Core.UI
{
    public class Success : UIBase
    {
        [SerializeField] private Button next;

        private void OnEnable()
        {
            next.onClick.AddListener(OnNextClicked);
        }

        private void OnDisable()
        {
            next.onClick.RemoveListener(OnNextClicked);
        }

        private void OnNextClicked()
        {
            UIEvents.SetUI?.Invoke(UIKey.Success, false);
            PlayerData.Level++;
            CoreEvents.LoadScene?.Invoke();
        }

        public override async UniTask SetUI(bool state)
        {
            await UniTask.Yield();
        }
    }
}