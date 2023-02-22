using System.Collections.Generic;
using _Project.Scripts.Runtime.Core.Enums;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Core.UI;
using _Project.Scripts.Runtime.Utils.Class;
using UnityEngine;

namespace _Project.Scripts.Runtime.Core.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<Pair<UIKey, UIBase>> serializedUIs;
        private Dictionary<UIKey, UIBase> UIs;

        private void Awake()
        {
            UIs = new Dictionary<UIKey, UIBase>();
            foreach (var serializedUI in serializedUIs)
                UIs.Add(serializedUI.Value1, serializedUI.Value2);
        }

        private void Start()
        {
            foreach (var UI in UIs)
                UIEvents.SetUI?.Invoke(UI.Key, UI.Key == UIKey.Splash);
        }

        private void OnEnable()
        {
            UIEvents.SetUI += OnSetUI;
        }

        private void OnDisable()
        {
            UIEvents.SetUI -= OnSetUI;
        }

        private async void OnSetUI(UIKey key, bool state)
        {
            if (!UIs.ContainsKey(key)) return;
            if (state)
            {
                UIs[key].gameObject.SetActive(true);
                await UIs[key].SetUI(true);
            }
            else
            {
                await UIs[key].SetUI(false);
                UIs[key].gameObject.SetActive(false);
            }

            Debug.Log(key + " UI set: " + state);
        }
    }
}