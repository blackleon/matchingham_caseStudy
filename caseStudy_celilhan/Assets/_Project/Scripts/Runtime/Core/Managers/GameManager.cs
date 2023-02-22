using _Project.Scripts.Runtime.Core.Enums;
using _Project.Scripts.Runtime.Core.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Runtime.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
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

        private void Init()
        {
            UIEvents.SetUI?.Invoke(UIKey.Splash, false);
        }
    }
}