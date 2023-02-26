using System.Collections.Generic;
using _Project.Scripts.Runtime.Core.Controller;
using _Project.Scripts.Runtime.Core.Events;
using _Project.Scripts.Runtime.Data.Class;
using _Project.Scripts.Runtime.Utils.Mono;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        private List<Transform> slots;
        private Dictionary<Transform, MatchableController> slotPairs;

        private void Awake()
        {
            var widthPerSlot = (canvas.sizeDelta.x - totalGap) / GameData.MatchableSlotCount;
            var singleWidth = slotPrefab.rectTransform.sizeDelta.x;
            var space = (widthPerSlot - singleWidth) * 0.5f;
            var left = (-canvas.sizeDelta.x + totalGap + singleWidth) * 0.5f + space;

            slots = new List<Transform>();

            for (var i = 0; i < GameData.MatchableSlotCount; i++)
            {
                var slot = Instantiate(slotPrefab, spawnRoot);
                slot.rectTransform.anchoredPosition = new Vector2(left + i * widthPerSlot, 0f);
                slots.Add(slot.transform);
                slot.gameObject.SetActive(true);
            }

            slotPairs = new Dictionary<Transform, MatchableController>();
            foreach (var slot in slots)
                slotPairs.Add(slot, null);
        }

        private void OnEnable()
        {
            CoreEvents.MatchablePlaced += OnMatchablePlaced;
            CoreEvents.MatchableMoved += OnMatchableMoved;
            CoreEvents.MatchableRemoved += OnMatchableRemoved;
        }

        private void OnDisable()
        {
            CoreEvents.MatchablePlaced -= OnMatchablePlaced;
            CoreEvents.MatchableMoved -= OnMatchableMoved;
            CoreEvents.MatchableRemoved -= OnMatchableRemoved;
        }

        private void OnMatchablePlaced(MatchableController matchable, int target)
        {
            var slot = slots[target];

            DOTween.Kill("moveSlot" + matchable.id);

            foreach (var child in matchable.gameObjects)
                child.gameObject.layer = LayerMask.NameToLayer("InSlot");

            matchable.transform.DOMove(slot.position - slot.forward * 0.25f - slot.up * 0.25f, 0.1f).SetId("moveSlot" + matchable.id);
            matchable.visual.DORotate(Quaternion.LookRotation(transform.forward, transform.up).eulerAngles, 0.1f)
                .SetId("moveSlot" + matchable.id);
            slotPairs[slots[target]] = matchable;
        }

        private async void OnMatchableMoved(int source, int target, bool match)
        {
            var sourceSlot = slots[source];
            var targetSlot = slots[target];
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
            var targetSlot = slots[target];
            var moveTargetSlot = slots[moveTarget];
            var matchable = slotPairs[targetSlot];
            matchable.matched = true;

            DOTween.Kill("moveSlot" + matchable.id);

            matchable.transform
                .DOMove(moveTargetSlot.position - targetSlot.forward * 0.25f - targetSlot.up * 0.25f + Random.insideUnitSphere * 0.125f, 0.25f)
                .SetId("matched" + matchable.id);
            matchable.transform.DOScale(2f, 0.05f).SetId("matched" + matchable.id);
            matchable.transform.DOScale(0f, 0.2f).SetId("matched" + matchable.id).SetDelay(0.05f).OnComplete(() =>
            {
                matchable.ReturnToPool();
                Pool.Return(matchable.gameObject);
            });

            slotPairs[targetSlot] = null;
        }
    }
}