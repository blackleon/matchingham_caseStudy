using System.Collections.Generic;
using _Project.Scripts.Runtime.Core.Controller;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Enums;
using _Project.Scripts.Runtime.Utils.Mono;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.Core.Managers
{
    public class SlotManager : MonoBehaviour
    {
        [SerializeField] private RectTransform canvas;
        [SerializeField] private RectTransform spawnRoot;
        [SerializeField] private float totalGap;
        [SerializeField] private Image slotPrefab;
        [SerializeField] private Image comboFill;
        [SerializeField] private TextMeshProUGUI comboCount;

        public static List<Transform> Slots;
        private Dictionary<Transform, MatchableController> slotPairs;
        private bool destroyed;

        private void Awake()
        {
            var widthPerSlot = (canvas.sizeDelta.x - totalGap) / GameData.MatchableSlotCount;
            var singleWidth = slotPrefab.rectTransform.sizeDelta.x;
            var space = (widthPerSlot - singleWidth) * 0.5f;
            var left = (-canvas.sizeDelta.x + totalGap + singleWidth) * 0.5f + space;

            Slots = new List<Transform>();

            for (var i = 0; i < GameData.MatchableSlotCount; i++)
            {
                var slot = Instantiate(slotPrefab, spawnRoot);
                slot.rectTransform.anchoredPosition = new Vector2(left + i * widthPerSlot, 0f);
                Slots.Add(slot.transform);
                slot.gameObject.SetActive(true);
            }

            slotPairs = new Dictionary<Transform, MatchableController>();
            foreach (var slot in Slots)
                slotPairs.Add(slot, null);


            UIEvents.SetUI += OnSetUI;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            CoreEvents.MatchablePlaced += OnMatchablePlaced;
            CoreEvents.MatchableMoved += OnMatchableMoved;
            CoreEvents.MatchableRemoved += OnMatchableRemoved;
            CoreEvents.ComboChanged += OnComboChanged;
        }

        private void OnDisable()
        {
            CoreEvents.MatchablePlaced -= OnMatchablePlaced;
            CoreEvents.MatchableMoved -= OnMatchableMoved;
            CoreEvents.MatchableRemoved -= OnMatchableRemoved;
            CoreEvents.ComboChanged -= OnComboChanged;
        }

        private void OnDestroy()
        {
            UIEvents.SetUI -= OnSetUI;
            destroyed = true;
        }

        private async void OnSetUI(UIKey key, bool state)
        {
            if (key == UIKey.Main)
            {
                if (state)
                {
                    comboCount.text = GameData.ComboCount + "<size=100>x</size>";
                    gameObject.SetActive(true);
                }
                else
                {
                    DOTween.Kill("comboReset");
                    comboCount.transform.localScale = Vector3.one;
                    comboFill.transform.localScale = Vector3.one;

                    comboCount.text = "";
                    DOTween.To(() => comboFill.fillAmount, x => comboFill.fillAmount = x, 0f, 0.75f).SetEase(Ease.Linear);

                    await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
                    if (destroyed) return;

                    gameObject.SetActive(false);
                }
            }
        }

        private void OnMatchablePlaced(MatchableController matchable, int target)
        {
            var slot = Slots[target];

            DOTween.Kill("moveSlot" + matchable.id);

            foreach (var child in matchable.gameObjects)
                child.gameObject.layer = LayerMask.NameToLayer("InSlot");
            
            matchable.transform.DOMove(slot.position - slot.forward * 0.25f - slot.up * 0.25f, 0.1f)
                .SetId("moveSlot" + matchable.id);
            matchable.visual.DORotate(Quaternion.LookRotation(transform.forward + transform.right, transform.up).eulerAngles, 0.1f)
                .SetId("moveSlot" + matchable.id);
            slotPairs[Slots[target]] = matchable;
        }

        private async void OnMatchableMoved(int source, int target, bool match)
        {
            var sourceSlot = Slots[source];
            var targetSlot = Slots[target];
            var matchable = slotPairs[sourceSlot];
            if (matchable.matched) return;
            slotPairs[targetSlot] = matchable;

            if (match)
                await UniTask.Delay(System.TimeSpan.FromSeconds(0.25f));

            DOTween.Kill("moveSlot" + matchable.id);

            matchable.transform.DOMove(targetSlot.position - targetSlot.forward * 0.25f - targetSlot.up * 0.25f, 0.1f)
                .SetId("moveSlot" + matchable.id);
        }

        private void OnMatchableRemoved(int target, int moveTarget)
        {
            var targetSlot = Slots[target];
            var moveTargetSlot = Slots[moveTarget];
            var matchable = slotPairs[targetSlot];
            matchable.matched = true;

            DOTween.Kill("moveSlot" + matchable.id);

            matchable.transform
                .DOMove(
                    moveTargetSlot.position - targetSlot.forward * 0.25f - targetSlot.up * 0.25f +
                    Random.insideUnitSphere * 0.125f, 0.25f)
                .SetId("matched" + matchable.id);
            matchable.transform.DOScale(2f, 0.05f).SetId("matched" + matchable.id);
            matchable.transform.DOScale(0f, 0.2f).SetId("matched" + matchable.id).SetDelay(0.05f).OnComplete(() =>
            {
                matchable.ReturnToPool();
                Pool.Return(matchable.gameObject);
            });

            slotPairs[targetSlot] = null;
        }

        private void OnComboChanged()
        {
            DOTween.Kill("comboReset");
            comboFill.fillAmount = 1f;
            comboCount.text = GameData.ComboCount + "<size=\"100\">x</size>";

            comboCount.transform.localScale = Vector3.one * 2f;
            comboCount.transform.DOScale(1f, 0.25f).SetId("comboReset");

            comboFill.transform.localScale = Vector3.one * 2f;
            comboFill.transform.DOScale(1f, 0.25f).SetId("comboReset");

            DOTween.To(() => comboFill.fillAmount, x => comboFill.fillAmount = x, 0f, 5f).SetEase(Ease.Linear)
                .SetId("comboReset").OnComplete(() =>
                {
                    GameData.ComboCount = 0;
                    comboCount.text = GameData.ComboCount + "<size=100>x</size>";
                });
        }
    }
}