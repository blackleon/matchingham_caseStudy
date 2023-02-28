using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.Core.UI
{
    public class Settings : UIBase
    {
        [Header("Buttons")]
        [SerializeField] private Button audioButton;
        [SerializeField] private Button hapticButton;
        [SerializeField] private Button resume;
        [SerializeField] private Button close;
        [SerializeField] private Button bg;

        [Header("Toggle Boxes")]
        [SerializeField] private Image audioBox;
        [SerializeField] private Image hapticBox;
        
        [Header("Design")]
        [SerializeField] private float toggleOffset;
        [SerializeField] private Color onColor;
        [SerializeField] private Color offColor;

        private void OnEnable()
        {
            audioButton.onClick.AddListener(ToggleAudio);
            hapticButton.onClick.AddListener(ToggleHaptic);
            resume.onClick.AddListener(OnResumeGame);
            close.onClick.AddListener(OnResumeGame);
            bg.onClick.AddListener(OnResumeGame);
        }

        private void OnDisable()
        {
            audioButton.onClick.RemoveListener(ToggleAudio);
            hapticButton.onClick.RemoveListener(ToggleHaptic);
            resume.onClick.RemoveListener(OnResumeGame);
            close.onClick.RemoveListener(OnResumeGame);
            bg.onClick.RemoveListener(OnResumeGame);
        }

        private void ToggleAudio()
        {
            PlayerData.Audio = !PlayerData.Audio;
            UpdateAudio();
        }

        private void UpdateAudio()
        {
            audioBox.color = PlayerData.Audio ? onColor : offColor;
            audioBox.rectTransform.anchoredPosition = new Vector2(PlayerData.Audio ? +toggleOffset : -toggleOffset, 0f);
        }

        private void ToggleHaptic()
        {
            PlayerData.Haptic = !PlayerData.Haptic;
            UpdateHaptic();
        }

        private void UpdateHaptic()
        {
            hapticBox.color = PlayerData.Haptic ? onColor : offColor;
            hapticBox.rectTransform.anchoredPosition =
                new Vector2(PlayerData.Haptic ? +toggleOffset : -toggleOffset, 0f);
        }

        private void OnResumeGame()
        {
            UIEvents.SetUI?.Invoke(UIKey.Settings, false);
            GameData.State = GameState.Play;
        }

        public override async UniTask SetUI(bool state)
        {
            if (state)
            {
                GameData.State = GameState.Pause;
                UpdateAudio();
                UpdateHaptic();
            }

            await UniTask.Yield();
        }
    }
}