using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Runtime.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            PlayerData.Init();
            GameData.Init();
        }

        private void OnEnable()
        {
            CoreEvents.LoadScene += OnLoadScene;
        }

        private void OnDisable()
        {
            CoreEvents.LoadScene -= OnLoadScene;
        }

        private async void Start()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(2f));

            CoreEvents.LoadScene?.Invoke();
        }

        private async void OnLoadScene()
        {
            await SceneManager.LoadSceneAsync("Game");

            Init();
        }

        private async void Init()
        {
            GameData.Reset();

            await UniTask.Yield();

            UIEvents.SetUI?.Invoke(UIKey.Splash, false);
        }
    }
}