using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Runtime.Core.Managers
{
    public class GameManager : MonoBehaviour //Game Core Starts Here
    {
        private void Awake()
        {
            //Init Data
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
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));
            UIEvents.SetUI?.Invoke(UIKey.Splash, false);

            CoreEvents.LoadScene?.Invoke();
        }

        private async void OnLoadScene() //Load Game Scene
        {
            await SceneManager.LoadSceneAsync("Game");

            Init();
        }

        private void Init() //Init data and game states
        {
            GameData.Reset();
            GameData.State = GameState.Stop;
        }
    }
}