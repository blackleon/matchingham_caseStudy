using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.Core.UI
{
    public class Main : UIBase
    {
        [SerializeField] private Button settings;
        [SerializeField] private TextMeshProUGUI money;
        [SerializeField] private TextMeshProUGUI timer;
        [SerializeField] private TextMeshProUGUI level;

        [SerializeField] private Image moneyPrefab;

        private int bufferedMoney;

        private void OnEnable()
        {
            settings.onClick.AddListener(OnSettingsClicked);
            CoreEvents.MoneyChanged += OnMoneyChanged;
            UIEvents.UpdateTimer += OnUpdateTimer;
        }

        private void OnDisable()
        {
            settings.onClick.RemoveListener(OnSettingsClicked);
            CoreEvents.MoneyChanged -= OnMoneyChanged;
            UIEvents.UpdateTimer -= OnUpdateTimer;
        }

        private void OnSettingsClicked()
        {
            UIEvents.SetUI?.Invoke(UIKey.Settings, true);
        }

        private async void OnMoneyChanged(int change, Vector3 source) //generate coin object and move to related position in UI
        {
            if (change > 0)
            {
                for (var i = 0; i < change; i++)
                {
                    var moneyImg = Instantiate(moneyPrefab, transform);
                    moneyImg.transform.position = source;
                    moneyImg.gameObject.SetActive(true);
                    moneyImg.transform.DOMove(source + (Vector3)(Random.insideUnitCircle * 250f), 0.125f)
                        .SetUpdate(true);
                    moneyImg.transform.DOMove(moneyPrefab.transform.position, 0.25f).OnComplete(() =>
                    {
                        moneyImg.transform.DOScale(3f, 0.1f).SetUpdate(true);
                        moneyImg.transform.DOScale(0f, 0.25f).SetUpdate(true).SetDelay(0.1f).OnComplete(() =>
                        {
                            bufferedMoney++;
                            UpdateMoney();
                            Destroy(moneyImg);
                        });
                    }).SetUpdate(true).SetDelay(0.25f);

                    await UniTask.Delay(System.TimeSpan.FromSeconds(0.05f));
                }
            }
            else
            {
                bufferedMoney = PlayerData.GetMoney();
                UpdateMoney();
            }
        }

        private void UpdateMoney()
        {
            money.text = bufferedMoney.ToString();
        }

        private void OnUpdateTimer(int remaining)
        {
            timer.text = (remaining / 60) + ":" + (remaining % 60).ToString("00");
        }

        public override async UniTask SetUI(bool state)
        {
            if (state)
            {
                bufferedMoney = PlayerData.GetMoney();
                UpdateMoney();
                level.text = "LEVEL " + (PlayerData.Level + 1);

                await UniTask.Yield();
            }
            else
            {
                timer.text = "";
            }
        }
    }
}